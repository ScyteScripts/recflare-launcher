"""Create a small PatchKit distribution for isolated native updater tests."""
import hashlib
import json
import random
import zipfile
from pathlib import Path
import xxhash

root = Path(__file__).resolve().parent.parent / 'validation/distribution'
root.mkdir(parents=True, exist_ok=True)
catalog = {'latest': 171, 'releases': []}
payload = random.Random(26).randbytes(3 * 1024 * 1024)
for version in [171, 172]:
    members = {'dummy.exe': b'', 'hello.txt': f'fixture-v{version}\n'.encode(),
               'payload.bin': payload, 'RecRoomStudio/ProjectTemplate/ProjectSettings/ProjectVersion.txt':
               b'm_EditorVersion: 6000.0.27f1\nm_EditorVersionWithRevision: 6000.0.27f1 (b8bf2c881068)\n'}
    members['RecRoomStudio/ProjectTemplate/ProjectSettings/RecRoomStudioPatch.ini'] = b'[Network]\nServerUrl=https://ns.recflare.net\n'
    members['RecRoomStudio/ProjectTemplate/RecRoomStudioPackages/sdk.tgz'] = f'fixture-sdk-{version}'.encode()
    archive = root / f'v{version}.zip'
    with zipfile.ZipFile(archive, 'w', zipfile.ZIP_STORED) as z:
        for name, data in sorted(members.items()):
            z.writestr(zipfile.ZipInfo(name, date_time=(2026, 1, 1, 0, 0, 0)), data)
    data = archive.read_bytes()
    catalog['releases'].append({'version': version, 'label': f'fixture-v{version}', 'partSize': 32*1024*1024,
      'summary': {'version': '1.0', 'size': len(data), 'uncompressed_size': sum(map(len, members.values())),
        'compression_method': 'zip', 'encryption_method': 'none', 'hash_code': hashlib.sha256(data).hexdigest(),
        'chunks': {'size': 32*1024*1024, 'hashes': [xxhash.xxh32(data, seed=42).hexdigest()]},
        'files': [{'path': name, 'size': len(data), 'flags': '', 'hash': xxhash.xxh32(data, seed=42).hexdigest()} for name, data in sorted(members.items())]}})
(root / 'catalog.json').write_text(json.dumps(catalog), encoding='utf-8')
print('Created isolated v171/v172 native-updater fixtures.')
