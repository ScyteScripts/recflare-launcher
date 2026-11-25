"""Package only the measured portable release, excluding user data and profiles."""
import hashlib
import json
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
folder = ROOT / 'dist/RecFlare Launcher'
manifest = json.loads((folder / 'build-manifest.json').read_text())
archive = ROOT / 'dist/RecFlareLauncher-win64.zip'
with zipfile.ZipFile(archive, 'w', zipfile.ZIP_DEFLATED, compresslevel=6) as output:
    for record in manifest['files'] + [{'path': 'build-manifest.json'}]:
        data = (folder / record['path']).read_bytes()
        if 'sha256' in record and hashlib.sha256(data).hexdigest() != record['sha256']:
            raise RuntimeError('Release changed after packaging: ' + record['path'])
        item = zipfile.ZipInfo('RecFlare Launcher/' + record['path'], date_time=(2026, 1, 1, 0, 0, 0))
        item.compress_type = zipfile.ZIP_DEFLATED
        item.external_attr = 0o100644 << 16
        output.writestr(item, data)
digest = hashlib.sha256(archive.read_bytes()).hexdigest()
archive.with_suffix('.zip.sha256').write_text(digest + '  ' + archive.name + '\n')
print(f'Archived {archive.name}: {archive.stat().st_size:,} bytes; SHA-256 {digest}')
