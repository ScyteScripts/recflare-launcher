"""Exercise ONLY the newly compiled setup in explicit, unregistered fixtures."""
import ctypes
import importlib.util
import uuid
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
import winreg

ROOT = Path(__file__).resolve().parent.parent
EXE = ROOT / 'dist/RecFlareLauncher-Setup.exe'
RUN = Path(tempfile.mkdtemp(prefix='installer-run-', dir=ROOT / 'validation')).resolve()
assert RUN.is_relative_to((ROOT / 'validation').resolve())
TARGET = RUN / 'installation with spaces'


def digest(file):
    return hashlib.sha256(file.read_bytes()).hexdigest()


def registry_snapshot():
    records = []
    for name in ['Software\\b2ae587a-b913-5ae0-a9fa-017e52aabe01',
                 'Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\b2ae587a-b913-5ae0-a9fa-017e52aabe01',
                 'Software\\RecRoomStudioSourceLauncher',
                 'Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\RecRoomStudioSourceLauncher']:
        try:
            with winreg.OpenKey(winreg.HKEY_CURRENT_USER, name, 0, winreg.KEY_READ | winreg.KEY_WOW64_64KEY) as key:
                values = []
                index = 0
                while True:
                    try:
                        values.append(winreg.EnumValue(key, index))
                        index += 1
                    except OSError:
                        break
                records.append((name, values))
        except FileNotFoundError:
            records.append((name, None))
    return records


def launch(exe, flags, destination, uninstall=False):
    # NSIS requires its unquoted /D= or _?= path to be the LAST command-line item.
    assert '"' not in str(destination)
    command = '"' + str(exe) + '" ' + flags + (' _?=' if uninstall else ' /D=') + str(destination)
    result = subprocess.run(command, cwd=RUN, creationflags=subprocess.CREATE_NO_WINDOW, timeout=90)
    return result.returncode


def main():
    before_registry = registry_snapshot()
    payload = json.loads(EXE.with_suffix('.payload.json').read_text(encoding='utf-8'))
    results = []
    assert launch(EXE, '/S /EXTRACTONLY', TARGET) == 0
    for item in payload['files']:
        assert digest(TARGET / item['path']) == item['sha256'], item['path']
    assert (TARGET / 'Uninstall Source Launcher.exe').is_file()
    results.append('extract-only-full-payload-hashes')

    (TARGET / 'data').mkdir()
    (TARGET / 'data/keep.txt').write_text('local user data')
    (TARGET / 'services.ini').write_text('[Launcher]\nUseInstalledTemplate=0\n')
    (TARGET / 'unowned.txt').write_text('keep unknown files')
    old_symbols=TARGET / 'resources/updater/Recovered.Updater.pdb'
    old_symbols.write_bytes(b'old debug symbols')
    assert launch(EXE, '/S /EXTRACTONLY', TARGET) == 0
    assert not old_symbols.exists()
    assert not (TARGET / 'launcher-backups').exists()
    assert (TARGET / 'data/keep.txt').read_text() == 'local user data'
    assert (TARGET / 'unowned.txt').read_text() == 'keep unknown files'
    results.append('upgrade-preserves-data-profile-and-unknown-files')
    results.append('upgrade-retires-old-debug-symbols-without-backup')

    # Hold a real Windows file lock; setup must stop before changing payload.
    kernel = ctypes.WinDLL('kernel32', use_last_error=True)
    kernel.CreateFileW.restype = ctypes.c_void_p
    kernel.CreateFileW.argtypes = [ctypes.c_wchar_p, ctypes.c_uint32, ctypes.c_uint32, ctypes.c_void_p,
                                   ctypes.c_uint32, ctypes.c_uint32, ctypes.c_void_p]
    kernel.CloseHandle.argtypes = [ctypes.c_void_p]
    handle = kernel.CreateFileW(str(TARGET / 'RecFlare Launcher.exe'), 0x80000000, 1, None, 3, 0x80, None)
    assert handle not in (None, ctypes.c_void_p(-1).value)
    try:
        assert launch(EXE, '/S /EXTRACTONLY', TARGET) == 32
    finally:
        kernel.CloseHandle(ctypes.c_void_p(handle))
    results.append('locked-launcher-aborts-preflight')

    foreign = RUN / 'foreign'
    foreign.mkdir()
    (foreign / 'keep.txt').write_text('foreign data')
    assert launch(EXE, '/S /EXTRACTONLY', foreign) == 87
    assert sorted(p.name for p in foreign.iterdir()) == ['keep.txt']
    results.append('foreign-directory-refused')

    patched = RUN / 'patched-2.0.1'
    (patched / 'resources/libpkapps').mkdir(parents=True)
    (patched / 'resources/config.json').write_text(json.dumps({'id':'recroom-studio-launcher','name':'Rec Room Studio Launcher','version':'2.0.1'}, indent=2))
    (patched / 'resources/app.asar').write_bytes(b'patched old asar')
    (patched / 'Rec Room Studio Launcher.exe').write_bytes(b'old runtime')
    (patched / 'resources/libpkapps/PatchKit.Api.dll').write_bytes(b'legacy dependency')
    (patched / 'Uninstall Rec Room Studio Launcher.exe').write_bytes(b'old uninstaller')
    (patched / 'studio-setup-payload.json').write_text('{}')
    (patched / 'Run-Recovered.cmd').write_text('old launch wrapper')
    (patched / 'BUILD-NOTICE.txt').write_text('obsolete recovery notes')
    (patched / 'settings-store.json').write_text('preserve launcher settings')
    template = patched / 'Apps/Rec Room Studio/RecRoomStudio/ProjectTemplate'
    template.mkdir(parents=True)
    (template / 'keep.txt').write_text('preserve existing template')
    assert launch(EXE, '/S /EXTRACTONLY', patched) == 0
    assert not (patched / 'launcher-backups').exists()
    assert not (patched / 'resources/libpkapps/PatchKit.Api.dll').exists()
    assert not (patched / 'Uninstall Rec Room Studio Launcher.exe').exists()
    assert not (patched / 'Run-Recovered.cmd').exists()
    assert not (patched / 'BUILD-NOTICE.txt').exists()
    assert (patched / 'settings-store.json').read_text() == 'preserve launcher settings'
    assert (template / 'keep.txt').read_text() == 'preserve existing template'
    assert json.loads((patched / 'resources/config.json').read_text())['version'] == '3.0.0'
    results.append('patched-launcher-upgrade-preserves-template-settings-without-backup')

    # Users may have the public original, an unmarked patch, or a portable build.
    for version in ['2.0.0', '2.0.1', '3.0.0']:
        previous = RUN / ('unmarked-' + version)
        (previous / 'resources').mkdir(parents=True)
        (previous / 'resources/config.json').write_text(json.dumps({'id':'recroom-studio-launcher','name':'Rec Room Studio Launcher','version':version}, indent=2))
        (previous / 'resources/app.asar').write_bytes(b'previous launcher asar')
        (previous / 'Rec Room Studio Launcher.exe').write_bytes(b'previous launcher runtime')
        if version == '3.0.0':
            (previous / 'build-manifest.json').write_text(json.dumps({'version':'3.0.0','updater':'source-built-dotnet-rpc'}))
        (previous / 'settings-store.json').write_text('keep settings')
        (previous / 'Apps/ProjectTemplate').mkdir(parents=True)
        (previous / 'Apps/ProjectTemplate/keep.txt').write_text('keep template')
        (previous / 'authored-project').mkdir()
        (previous / 'authored-project/keep.txt').write_text('keep project')
        assert launch(EXE, '/S /EXTRACTONLY', previous) == 0, version
        assert not (previous / 'launcher-backups').exists()
        assert (previous / 'settings-store.json').read_text() == 'keep settings'
        assert (previous / 'Apps/ProjectTemplate/keep.txt').read_text() == 'keep template'
        assert (previous / 'authored-project/keep.txt').read_text() == 'keep project'
        results.append('unmarked-' + version + '-upgrade-with-data-preservation-without-backup')

    for label, config in [('unrelated', {'id':'another-app','name':'Another App','version':'2.0.0'}),
                          ('unknown-version', {'id':'recroom-studio-launcher','name':'Rec Room Studio Launcher','version':'99.0.0'}),
                          ('malformed', None)]:
        unknown = RUN / label
        (unknown / 'resources').mkdir(parents=True)
        (unknown / 'resources/config.json').write_text(json.dumps(config) if config else '{broken json')
        (unknown / 'resources/app.asar').write_bytes(b'keep asar')
        (unknown / 'Rec Room Studio Launcher.exe').write_bytes(b'keep executable')
        assert launch(EXE, '/S /EXTRACTONLY', unknown) == 87, label
        assert (unknown / 'resources/app.asar').read_bytes() == b'keep asar'
        assert (unknown / 'Rec Room Studio Launcher.exe').read_bytes() == b'keep executable'
        assert not (unknown / 'source-launcher-install.ini').exists()
        results.append(label + '-installation-refused-without-writes')

    # Compile the exact same setup/payload with an isolated default directory
    # and an absent registry namespace, then install WITHOUT /D=.
    spec = importlib.util.spec_from_file_location('installer_builder', ROOT / 'scripts/build-installer.py')
    builder = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(builder)
    first_default = RUN / "new user's default Studio location"
    fixture_setup = RUN / 'fresh-default-setup.exe'
    builder.compile_setup(ROOT / 'dist/RecFlare Studio Launcher', ROOT / 'build/installer-source',
                          fixture_setup, dict(payload), builder.find_makensis(),
                          {'DEFAULT_INSTALL_DIR':first_default, 'INSTALL_KEY':'Software\\RecFlareStudioValidation'+uuid.uuid4().hex})
    assert not first_default.exists()
    assert subprocess.run('"'+str(fixture_setup)+'" /S /EXTRACTONLY', cwd=RUN, timeout=90).returncode == 0
    assert (first_default / 'source-launcher-install.ini').is_file()
    assert not (first_default / 'launcher-backups').exists()
    results.append('first-install-absent-default-and-registry-without-D-override')
    # Leave only unrelated data in that default path after an owned removal.
    (first_default / 'unrelated.txt').write_text('must survive collision')
    (first_default / 'settings-store.json').write_text('keep leftover settings')
    (first_default / 'Apps/Template').mkdir(parents=True)
    (first_default / 'Apps/Template/keep.txt').write_text('keep leftover template')
    fresh_runner = RUN / 'fresh-default-uninstaller.exe'
    shutil.copyfile(first_default / 'Uninstall Source Launcher.exe', fresh_runner)
    assert launch(fresh_runner, '/S', first_default, uninstall=True) == 0
    assert (first_default / 'unrelated.txt').read_text() == 'must survive collision'
    assert subprocess.run('"'+str(fixture_setup)+'" /S /EXTRACTONLY', cwd=RUN, timeout=90).returncode == 0
    assert (first_default / 'source-launcher-install.ini').is_file()
    assert (first_default / 'resources/app.asar').exists()
    assert (first_default / 'unrelated.txt').read_text() == 'must survive collision'
    assert (first_default / 'settings-store.json').read_text() == 'keep leftover settings'
    assert (first_default / 'Apps/Template/keep.txt').read_text() == 'keep leftover template'
    results.append('first-install-standard-directory-preserves-leftover-data-without-relocation')
    assert launch(fresh_runner, '/S', first_default, uninstall=True) == 0
    (first_default / 'Rec Room Studio Launcher.exe').write_bytes(b'unrecognized program')
    (first_default / 'resources').mkdir(exist_ok=True)
    (first_default / 'resources/config.json').write_text('{old unrecognized launcher metadata')
    assert subprocess.run('"'+str(fixture_setup)+'" /S /EXTRACTONLY', cwd=RUN, timeout=90).returncode == 0
    expected_exe = next(f['sha256'] for f in payload['files'] if f['path']=='RecFlare Launcher.exe')
    assert digest(first_default / 'RecFlare Launcher.exe') == expected_exe
    assert (first_default / 'source-launcher-install.ini').is_file()
    assert not (first_default / 'launcher-backups').exists()
    assert (first_default / 'settings-store.json').read_text() == 'keep leftover settings'
    assert (first_default / 'Apps/Template/keep.txt').read_text() == 'keep leftover template'
    results.append('standard-directory-upgrades-unrecognized-older-launcher-without-blocking')

    runner = RUN / 'uninstaller-fixture.exe'
    shutil.copyfile(TARGET / 'Uninstall Source Launcher.exe', runner)
    assert launch(runner, '/S', foreign, uninstall=True) == 87
    assert (foreign / 'keep.txt').read_text() == 'foreign data'
    results.append('uninstaller-refuses-unowned-directory')
    assert launch(runner, '/S', TARGET, uninstall=True) == 0
    assert not (TARGET / 'RecFlare Launcher.exe').exists()
    assert not (TARGET / 'resources/app.asar').exists()
    assert (TARGET / 'data/keep.txt').read_text() == 'local user data'
    assert (TARGET / 'services.ini').is_file()
    assert (TARGET / 'unowned.txt').read_text() == 'keep unknown files'
    results.append('uninstall-removes-owned-payload-preserves-user-data')
    assert registry_snapshot() == before_registry
    results += ['existing-registry-keys-unchanged']
    report = {'status': 'passed', 'installerSha256': digest(EXE), 'fixture': str(RUN), 'checks': results}
    (ROOT / 'validation/installer-summary.json').write_text(json.dumps(report, indent=2)+'\n')
    print(json.dumps(report, indent=2))


if __name__ == '__main__':
    main()
