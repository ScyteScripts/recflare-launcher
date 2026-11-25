import importlib.util
import hashlib
import json
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parent.parent
spec = importlib.util.spec_from_file_location('build_installer', ROOT / 'scripts/build-installer.py')
builder = importlib.util.module_from_spec(spec)
spec.loader.exec_module(builder)


class InstallerManifestTests(unittest.TestCase):
    def prepare(self, entry):
        with tempfile.TemporaryDirectory(prefix='studio-installer-manifest-') as directory:
            root = Path(directory)
            (root / 'payload').mkdir()
            (root / 'payload/build-manifest.json').write_text(json.dumps({'files': [entry], 'electron': '45.0.0', 'updater': 'source-built-dotnet-rpc'}))
            return builder.prepare(root / 'payload', root / 'generated')

    def test_parent_and_windows_paths_rejected(self):
        for path in ['../file', 'C:/file', 'resources\\file', '/absolute/file']:
            with self.subTest(path=path), self.assertRaisesRegex(RuntimeError, 'Unsafe'):
                self.prepare({'path': path, 'sha256': '0'*64})

    def test_user_data_cannot_be_payload(self):
        for path in ['data/profile', 'services.ini', 'source-launcher-install.ini']:
            with self.subTest(path=path), self.assertRaisesRegex(RuntimeError, 'installation-owned'):
                self.prepare({'path': path, 'sha256': '0'*64})

    def test_tampered_payload_is_rejected(self):
        with tempfile.TemporaryDirectory(prefix='studio-installer-tamper-') as directory:
            root = Path(directory)
            (root / 'payload').mkdir()
            (root / 'payload/app.exe').write_bytes(b'changed')
            (root / 'payload/build-manifest.json').write_text(json.dumps({'files': [{'path': 'app.exe', 'sha256': '0'*64}]}))
            with self.assertRaisesRegex(RuntimeError, 'hash mismatch'):
                builder.prepare(root / 'payload', root / 'generated')

    def test_generated_uninstaller_is_nonrecursive(self):
        script = (ROOT / 'installer/setup.nsi').read_text()
        self.assertNotIn('RMDir /r', script)
        self.assertNotIn('SETUP_BACKUP', script)
        self.assertNotIn('launcher-backups', script)
        self.assertIn('StrCmp $INSTDIR "${DEFAULT_INSTALL_DIR}" standard_studio_directory', script)
        self.assertIn('StrCmp $1 $2 0 keep_legacy_registration', script)
        self.assertIn('SETUP_RECOGNIZER', script)
        self.assertNotIn('Adopt only the marked', script)
        self.assertIn('RequestExecutionLevel user', script)

    def test_one_click_installer_has_guarded_success_launch(self):
        script = (ROOT / 'installer/setup.nsi').read_text()
        self.assertNotIn('!insertmacro MUI_PAGE_DIRECTORY', script)
        self.assertIn('AutoCloseWindow true', script)
        callback = script.split('Function .onInstSuccess', 1)[1].split('FunctionEnd', 1)[0]
        self.assertIn('IfSilent no_launch', callback)
        self.assertIn('StrCmp $ExtractOnly 1 no_launch', callback)
        self.assertIn('StrCmp $NoIntegration 1 no_launch', callback)
        self.assertIn('RecFlare Launcher.exe', callback)


if __name__ == '__main__':
    unittest.main()
