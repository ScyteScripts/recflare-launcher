"""Compile the current launcher setup from source, without the original setup EXE."""
import argparse
import hashlib
import json
import os
import re
import shutil
from pathlib import Path, PurePosixPath
import subprocess
import uuid

ROOT = Path(__file__).resolve().parent.parent
PRODUCT = 'RecRoomStudioSourceLauncher'


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def nsis(value):
    value = str(value)
    if '\r' in value or '\n' in value:
        raise ValueError('Newline in NSIS path')
    return value.replace('$', '$$').replace('"', '$\\"')


def prepare(payload, destination):
    manifest_file = payload / 'build-manifest.json'
    manifest = json.loads(manifest_file.read_text(encoding='utf-8'))
    records = manifest['files'] + [{'path': 'build-manifest.json', 'sha256': sha(manifest_file)}]
    seen, directories = set(), set()
    for item in records:
        path = PurePosixPath(item['path'])
        # Windows path syntax, case-folded collisions and reserved data paths
        # must be rejected before any compiler can embed files.
        if not path.parts or path.is_absolute() or '..' in path.parts or re.search(r'[:\\<>"|?*]', item['path']):
            raise RuntimeError('Unsafe payload path: ' + item['path'])
        if any(p in ('', '.') or p.endswith((' ', '.')) for p in path.parts):
            raise RuntimeError('Invalid Windows payload path')
        if any(re.fullmatch(r'(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(?:\..*)?', p, re.I) for p in path.parts):
            raise RuntimeError('Reserved Windows payload path')
        if path.parts[0].lower() in ['data', 'services.ini', 'source-launcher-install.ini', 'uninstall source launcher.exe']:
            raise RuntimeError('Payload includes installation-owned data')
        if path.as_posix().casefold() in seen:
            raise RuntimeError('Duplicate payload path')
        seen.add(path.as_posix().casefold())
        file = payload.joinpath(*path.parts)
        if any(p.is_symlink() or p.is_junction() for p in [file, *file.parents] if p != payload.parent):
            raise RuntimeError('Linked payload path')
        if sha(file) != item['sha256']:
            raise RuntimeError('Payload hash mismatch: ' + item['path'])
        directories.update(p.as_posix() for p in path.parents if p.as_posix() != '.')
    destination.mkdir(parents=True, exist_ok=True)
    install, preflight, uninstall_preflight, uninstall = [], [], [], []
    for rel in sorted(directories, key=lambda p: (p.count('/'), p)):
        target = '$INSTDIR\\' + nsis(rel.replace('/', '\\'))
        preflight += [f'Push "{target}"', 'Call CheckPath']
        uninstall_preflight += [f'Push "{target}"', 'Call un.CheckPath']
    last = None
    for item in records:
        rel = item['path'].replace('/', '\\')
        target = '$INSTDIR\\' + nsis(rel)
        for commands, prefix in [(preflight, ''), (uninstall_preflight, 'un.')]:
            commands += [f'Push "{target}"', f'Call {prefix}CheckPath', f'Push "{target}"', f'Call {prefix}CheckUnlocked']
        parent = str(PurePosixPath(item['path']).parent)
        if parent != last:
            install.append('SetOutPath "$INSTDIR' + ('\\' + nsis(parent.replace('/', '\\')) if parent != '.' else '') + '"')
            last = parent
        install.append('File "' + nsis(payload / item['path']) + '"')
        install += ['IfErrors 0 +3', 'SetErrorLevel 5', 'Abort']
        uninstall.append(f'Delete "{target}"')
    for rel in sorted(directories, key=lambda p: (p.count('/'), p), reverse=True):
        uninstall.append('RMDir "$INSTDIR\\' + nsis(rel.replace('/', '\\')) + '"')
    for name, lines in [('payload', install), ('preflight', preflight), ('uninstall-preflight', uninstall_preflight), ('uninstall', uninstall)]:
        (destination / (name + '.nsh')).write_text('\n'.join(lines) + '\n', encoding='utf-8-sig')
    # Only path metadata from the old capture is used for preflight/obsolete-file
    # cleanup. No old executable or DLL is embedded in the setup payload.
    legacy = set(json.loads((ROOT / 'config/legacy-files.json').read_text()))
    legacy.update(['Uninstall Rec Room Studio Launcher.exe', 'studio-setup-payload.json', 'resources/updater/Recovered.Updater.pdb',
                   'resources/updater/Recovered.Updater.exe', 'resources/updater/Recovered.Updater.dll',
                   'resources/updater/Recovered.Updater.deps.json', 'resources/updater/Recovered.Updater.runtimeconfig.json',
                   'resources/updater/RecFlare.Updater.pdb'])
    legacy_preflight = []
    for rel in sorted(legacy):
        posix = PurePosixPath(rel)
        if posix.is_absolute() or '..' in posix.parts or re.search(r'[:\\<>"|?*]', rel):
            raise RuntimeError('Unsafe legacy payload path')
        target = '$INSTDIR\\' + nsis(rel.replace('/', '\\'))
        legacy_preflight += [f'Push "{target}"', 'Call CheckPath',
                             f'Push "{target}"', 'Call CheckUnlocked']
    obsolete = [f'Delete "$INSTDIR\\{nsis(rel.replace(chr(47), chr(92)))}"' for rel in sorted(legacy - {f['path'] for f in records})]
    (destination / 'legacy-preflight.nsh').write_text('\n'.join(legacy_preflight)+'\n',encoding='utf-8-sig')
    (destination / 'obsolete.nsh').write_text('\n'.join(obsolete)+'\n',encoding='utf-8-sig')
    return {'product': PRODUCT, 'payloadManifestSha256': sha(manifest_file), 'files': records,
            'electron': manifest['electron'], 'updater': manifest['updater']}


def compressed_payload(payload, work, records, sevenzip):
    """Original-style non-solid 7z, 1 MiB dictionary and maximum BCJ2 filters.

    The independently installed upstream CLI extracts it; no plugin or executable
    from the original Rec Room installer is embedded or executed.
    """
    archive = work / ('payload-'+uuid.uuid4().hex+'.7z')
    listing = work / 'archive-files.txt'
    listing.write_text('\n'.join(item['path'] for item in records)+'\n', encoding='utf-8')
    options = ['-t7z', '-mx=9', '-md=1m', '-ms=off', '-mmt=2', '-scsUTF-8']
    result = subprocess.run([str(sevenzip), 'a', str(archive), '@'+str(listing), *options],
                            cwd=payload, capture_output=True)
    (work / '7z-compress.log').write_bytes(result.stdout+result.stderr)
    if result.returncode:
        raise RuntimeError('7z compression failed; see 7z-compress.log')
    test = subprocess.run([str(sevenzip), 't', str(archive)], capture_output=True)
    (work / '7z-test.log').write_bytes(test.stdout+test.stderr)
    if test.returncode:
        raise RuntimeError('7z archive verification failed')
    listing_result = subprocess.run([str(sevenzip), 'l', '-slt', str(archive)], capture_output=True, text=True, encoding='utf-8')
    method = next(line.split(' = ',1)[1] for line in listing_result.stdout.splitlines() if line.startswith('Method = '))
    # The helper is an ordinary upstream 7-Zip installation, kept temporary by
    # NSIS. It never changes the launcher payload or requires 7-Zip on the PC.
    for name in ['7z.dll','License.txt']:
        if not (sevenzip.parent / name).is_file():
            raise RuntimeError('7-Zip helper is missing '+name)
    commands = ['InitPluginsDir', 'SetOutPath "$PLUGINSDIR"', 'SetCompress off',
                'File /oname=payload.7z "'+nsis(archive)+'"', 'SetCompress auto',
                'File /oname=7z.exe "'+nsis(sevenzip)+'"',
                'File /oname=7z.dll "'+nsis(sevenzip.parent / '7z.dll')+'"',
                'File /oname=7zip-LICENSE.txt "'+nsis(sevenzip.parent / 'License.txt')+'"',
                'nsExec::ExecToStack /TIMEOUT=120000 \'"$PLUGINSDIR\\7z.exe" x "$PLUGINSDIR\\payload.7z" "-o$INSTDIR" -y -aoa -bd -bso0 -bsp0\'',
                'Pop $0', 'Pop $1', 'StrCmp $0 "0" +2', 'Goto install_failed', 'SetOutPath "$INSTDIR"']
    (work / 'payload.nsh').write_text('\n'.join(commands)+'\n', encoding='utf-8-sig')
    return {'style':'original-7z','options':options,'method':method,'solid':False,
            'archiveSha256':sha(archive),'archiveBytes':archive.stat().st_size,
            'extractorSha256':sha(sevenzip),'extractorLibrarySha256':sha(sevenzip.parent/'7z.dll')}


def build(makensis, payload, output, compression='original-7z', sevenzip=None):
    if sevenzip is None:
        sevenzip = Path(os.environ.get('ProgramFiles', r'C:\Program Files')) / '7-Zip/7z.exe'
    work = ROOT / 'build/installer-source'
    record = prepare(payload, work)
    record['compression'] = compressed_payload(payload, work, record['files'], sevenzip) if compression == 'original-7z' else {'style':'zlib'}
    compile_setup(payload, work, output, record, makensis)


def compile_setup(payload, work, output, record, makensis, extra_defines=None):
    defines = {'SETUP_OUTPUT': output, 'SETUP_ICON': payload / 'resources/launcher.ico',
               'SETUP_RECOGNIZER': ROOT / 'installer/recognize-install.ps1',
               'SETUP_PAYLOAD': work / 'payload.nsh', 'SETUP_PREFLIGHT': work / 'preflight.nsh',
               'SETUP_UNINSTALL_PREFLIGHT': work / 'uninstall-preflight.nsh',
               'SETUP_LEGACY_PREFLIGHT': work / 'legacy-preflight.nsh', 'SETUP_OBSOLETE': work / 'obsolete.nsh',
               'SETUP_UNINSTALL': work / 'uninstall.nsh', 'SETUP_PAYLOAD_HASH': record['payloadManifestSha256']}
    defines.update(extra_defines or {})
    output.parent.mkdir(parents=True, exist_ok=True)
    result = subprocess.run([str(makensis), '/V2'] + ['/D'+k+'='+str(v) for k, v in defines.items()] +
                            [str(ROOT / 'installer/setup.nsi')], capture_output=True)
    (work / 'compile.log').write_bytes(result.stdout + result.stderr)
    if result.returncode:
        raise RuntimeError('NSIS compile failed: ' + (result.stdout + result.stderr).decode('utf-8', errors='replace'))
    record['installerSha256'] = sha(output)
    record['compilerSha256'] = sha(makensis)
    record['sourceSha256'] = sha(ROOT / 'installer/setup.nsi')
    output.with_suffix('.exe.sha256').write_text(record['installerSha256']+'  '+output.name+'\n')
    output.with_suffix('.payload.json').write_text(json.dumps(record, indent=2)+'\n', encoding='utf-8')
    print('Built source installer:', output, '; files:', len(record['files']))


def find_makensis():
    configured = os.environ.get('MAKENSIS') or shutil.which('makensis')
    if configured:
        return Path(configured)
    return Path(os.environ.get('ProgramFiles(x86)', r'C:\Program Files (x86)')) / 'NSIS/makensis.exe'


if __name__ == '__main__':
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--makensis', type=Path, default=find_makensis())
    p.add_argument('--payload', type=Path, default=ROOT / 'dist/RecFlare Launcher')
    p.add_argument('--output', type=Path, default=ROOT / 'dist/RecFlareLauncher-Setup.exe')
    p.add_argument('--compression', choices=['original-7z','zlib'], default='original-7z')
    p.add_argument('--sevenzip', type=Path, default=Path(os.environ.get('ProgramFiles', r'C:\Program Files')) / '7-Zip/7z.exe')
    a = p.parse_args()
    build(a.makensis.resolve(), a.payload.resolve(), a.output.resolve(), a.compression, a.sevenzip.resolve())
