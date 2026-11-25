import {spawn} from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import {fileURLToPath} from 'node:url';

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const dir = path.join(root, 'dist', 'RecFlare Launcher');
const exe = path.join(dir, 'RecFlare Launcher.exe');
if (!fs.existsSync(exe)) throw new Error('Package the launcher first: npm run package');
spawn(exe, process.argv.slice(2), {cwd: dir, detached: true, stdio: 'ignore'}).unref();
console.log('Started ' + exe);
