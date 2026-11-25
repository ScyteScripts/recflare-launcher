'use strict';
const { app, BrowserWindow, shell } = require('electron');
const crypto = require('node:crypto');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { execFile, spawn } = require('node:child_process');
const { Readable } = require('node:stream');
const { pipeline } = require('node:stream/promises');

const CLIENT = {
  url: 'https://s3.g.megas4.com/2koayuyiwxv4groxzwdbbxg43cwustavrkvfb/recflare/2026/client.zip',
  md5: '96512e70cfa1c2d0da5745740cba1472',
};
const STEAM = { app: '471710', depot: '471711', manifest: '8867414017562658247' };
const DEPOT_DOWNLOADER =
  'https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_3.4.0/DepotDownloader-windows-x64.zip';
const PATCH_REPO = 'recflare/patch-2026';
const PATCH_FALLBACK = 'v0.0.3';
const PATCH_INI = {
  ApiHost: 'https://ns.recflare.net',
  ServerDomain: 'recflare.net',
  VoiceKeyXml:
    '<RSAKeyValue><Modulus>nu7lFZqLX+siEI2U8/JihESmm4JrBVLsCHdS8NoAipq2NdJqVouOHQoxtFfJTG6u/+kx4wocnsQ1Ydk5AISyy0GQeweQOTLKlkXsR6on/DL7LqKWxWp3M+MG4kkyZROWh2OELCAudC902p8VSaXmEyH8i1Sid9YiKe0FaNsyzWE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>',
};
const OLD_INSTALLER_KEY =
  'HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{8E1D2A7B-6C43-4F0A-9B7E-3D5F80C21A96}_is1';
const MARKER = '.recflare-launcher.json';
const SYSTEM = process.env.SystemRoot || 'C:\\Windows';
const TAR = path.join(SYSTEM, 'System32', 'tar.exe');
const REG = path.join(SYSTEM, 'System32', 'reg.exe');

const defaultDir = () => path.join(path.parse(os.homedir()).root, 'Games', 'RecFlare');
const settingsFile = () => path.join(app.getPath('userData'), 'recflare-game.json');

function readSettings() {
  try {
    return JSON.parse(fs.readFileSync(settingsFile(), 'utf8'));
  } catch {
    return {};
  }
}
function writeSettings(patch) {
  const next = { ...readSettings(), ...patch };
  fs.mkdirSync(path.dirname(settingsFile()), { recursive: true });
  fs.writeFileSync(settingsFile(), JSON.stringify(next, null, 2));
  return next;
}

function isInstall(dir) {
  if (!dir) return false;
  return ['RecRoom.exe', 'GameAssembly.dll', '2026patch.dll', 'RecRoomScreen.bat'].every((f) =>
    fs.existsSync(path.join(dir, f)),
  );
}
function readMarker(dir) {
  try {
    return JSON.parse(fs.readFileSync(path.join(dir, MARKER), 'utf8'));
  } catch {
    return {};
  }
}
function writeMarker(dir, patch) {
  fs.writeFileSync(path.join(dir, MARKER), JSON.stringify({ ...readMarker(dir), ...patch }, null, 2));
}

function run(file, args, options = {}) {
  return new Promise((resolve, reject) =>
    execFile(file, args, { windowsHide: true, maxBuffer: 4 * 1024 * 1024, ...options }, (error, stdout) =>
      error ? reject(error) : resolve(String(stdout)),
    ),
  );
}

async function oldInstall() {
  let location = null;
  try {
    const out = await run(REG, ['query', OLD_INSTALLER_KEY, '/v', 'InstallLocation']);
    const m = /InstallLocation\s+REG_SZ\s+(.+)/.exec(out);
    if (m) location = m[1].trim().replace(/[\\/]+$/, '');
  } catch {
  }
  if (location && isInstall(location)) return { dir: location, registered: true };
  if (isInstall(defaultDir()) && fs.existsSync(path.join(defaultDir(), 'unins000.exe')))
    return { dir: defaultDir(), registered: false };
  return null;
}

let task = null;
function send(payload) {
  for (const w of BrowserWindow.getAllWindows())
    if (!w.isDestroyed()) w.webContents.send('RecFlareLauncher.event', { channel: 'game:progress', payload });
}
function progress(text, done, total) {
  send({ running: true, text, pct: total ? Math.min(100, (done / total) * 100) : undefined });
}

async function download(url, file, label) {
  const res = await fetch(url, { redirect: 'follow' });
  if (!res.ok || !res.body) throw new Error(`${label}: download failed (HTTP ${res.status}).`);
  const total = Number(res.headers.get('content-length')) || 0;
  let done = 0;
  let last = 0;
  const body = Readable.fromWeb(res.body);
  body.on('data', (chunk) => {
    done += chunk.length;
    if (Date.now() - last > 250) {
      last = Date.now();
      progress(label, done, total);
    }
    if (task?.cancelled) body.destroy(new Error('Cancelled.'));
  });
  await pipeline(body, fs.createWriteStream(file));
}

async function md5(file) {
  const hash = crypto.createHash('md5');
  await pipeline(fs.createReadStream(file), hash);
  return hash.digest('hex');
}

async function extract(zip, dir) {
  fs.mkdirSync(dir, { recursive: true });
  await run(TAR, ['-xf', zip, '-C', dir], { cwd: dir });
}

async function latestPatch() {
  try {
    const res = await fetch(`https://api.github.com/repos/${PATCH_REPO}/releases/latest`, {
      headers: { accept: 'application/vnd.github+json' },
    });
    const tag = res.ok ? (await res.json()).tag_name : null;
    return /^v\d+\.\d+\.\d+$/.test(tag ?? '') ? tag : PATCH_FALLBACK;
  } catch {
    return PATCH_FALLBACK;
  }
}

function writePatchIni(dir) {
  const file = path.join(dir, '2026patch.ini');
  const lines = fs.existsSync(file) ? fs.readFileSync(file, 'utf8').split(/\r?\n/) : ['[config]'];
  for (const [key, value] of Object.entries(PATCH_INI)) {
    const at = lines.findIndex((l) => l.includes('=') && l.split('=')[0].trim().toLowerCase() === key.toLowerCase());
    if (at >= 0) lines[at] = `${key}=${value}`;
    else lines.push(`${key}=${value}`);
  }
  while (lines.length && lines.at(-1) === '') lines.pop();
  fs.writeFileSync(file, lines.join('\r\n') + '\r\n', 'ascii');
}

async function installPatch(dir) {
  const version = await latestPatch();
  const zip = path.join(dir, `RecFlare-2026Patch-${version}.zip`);
  await download(
    `https://github.com/${PATCH_REPO}/releases/download/${version}/2026Patch-${version}-x64.zip`,
    zip,
    'Downloading the RecFlare patch',
  );
  progress('Installing the RecFlare patch');
  try {
    if (!fs.existsSync(zip)) throw Object.assign(new Error('patch file removed'), { blocked: true });
    await extract(zip, dir);
  } catch (error) {
    const text = `${error?.message ?? ''} ${error?.stderr ?? ''}`;
    if (error?.blocked || /virus|potentially unwanted|ENOENT|EPERM|EACCES|Operation did not complete|Error opening archive|Failed to open|Cannot open|cannot be opened|Truncated/i.test(text))
      throw Object.assign(
        new Error(
          'Windows Security blocked the RecFlare patch, so it was not updated. Windows flags the patch\'s Injector.exe. ' +
            'The patch comes from github.com/recflare/patch-2026.',
        ),
        { code: 'AV_BLOCKED' },
      );
    throw error;
  } finally {
    fs.rmSync(zip, { force: true });
  }
  fs.writeFileSync(path.join(dir, 'steam_appid.txt'), '480');
  writePatchIni(dir);
  writeMarker(dir, { patch: version, patchedAt: new Date().toISOString() });
  return version;
}

async function fromMirror(dir) {
  const zip = path.join(dir, 'client.zip');
  if (!(fs.existsSync(zip) && (await md5(zip)) === CLIENT.md5)) {
    await download(CLIENT.url, zip, 'Downloading RecFlare');
    progress('Checking the download');
    if ((await md5(zip)) !== CLIENT.md5) {
      fs.rmSync(zip, { force: true });
      throw new Error('The downloaded game did not match the expected checksum. Try again.');
    }
  }
  progress('Unpacking RecFlare (this takes a few minutes)');
  await extract(zip, dir);
  fs.rmSync(zip, { force: true });
}

async function fromSteam(dir, username) {
  const tools = path.join(app.getPath('userData'), 'tools', 'DepotDownloader-3.4.0');
  const exe = path.join(tools, 'DepotDownloader.exe');
  if (!fs.existsSync(exe)) {
    const zip = path.join(app.getPath('temp'), 'DepotDownloader-3.4.0.zip');
    await download(DEPOT_DOWNLOADER, zip, 'Downloading the Steam downloader');
    await extract(zip, tools);
    fs.rmSync(zip, { force: true });
  }
  const args = ['-app', STEAM.app, '-depot', STEAM.depot, '-manifest', STEAM.manifest, '-remember-password', '-dir', dir];
  if (username) args.push('-username', username);
  else args.push('-qr');
  progress('Downloading from Steam: finish signing in in the window that opened');
  await new Promise((resolve, reject) => {
    const child = spawn(exe, args, { cwd: tools, detached: true, windowsHide: false, stdio: 'ignore' });
    task.child = child;
    child.on('error', reject);
    child.on('exit', (code) =>
      code === 0
        ? resolve()
        : reject(
            new Error(
              `The Steam download did not finish (exit code ${code}). Check that your sign-in worked and that Rec Room is in your Steam library.`,
            ),
          ),
    );
  });
}

async function job(kind, fn) {
  if (task) throw new Error('Something is already running.');
  task = { kind, cancelled: false };
  try {
    const result = await fn();
    send({ running: false, done: kind });
    return result;
  } catch (error) {
    send({ running: false, error: error.message, code: error.code ?? null });
    throw error;
  } finally {
    task = null;
  }
}

async function status() {
  const s = readSettings();
  const dir = s.folder && isInstall(s.folder) ? s.folder : null;
  return {
    installed: !!dir,
    folder: dir ?? s.folder ?? defaultDir(),
    patch: dir ? readMarker(dir).patch ?? null : null,
    running: task?.kind ?? null,
  };
}

const av = require('./av.cjs');
function avFolder() {
  const s = readSettings();
  return s.folder && isInstall(s.folder) ? s.folder : s.folder || defaultDir();
}

function register(handle) {
  handle('gameStatus', () => status());
  handle('gameOldInstall', () => oldInstall());
  handle('gameDefaultFolder', () => defaultDir());

  handle('avStatus', async () => {
    const folder = avFolder();
    const remembered = (readSettings().avExcluded ?? []).some(
      (x) => folder.toLowerCase() === x || folder.toLowerCase().startsWith(x.replace(/\\+$/, '') + '\\'),
    );
    const live = remembered ? true : await av.isExcluded(folder);
    return { folder, excluded: remembered || live === true };
  });
  handle('avAddExclusion', async () => {
    const folder = avFolder();
    const ok = await av.addExclusion(folder);
    if (ok) {
      const list = new Set((readSettings().avExcluded ?? []).map((x) => x.toLowerCase()));
      list.add(folder.toLowerCase());
      writeSettings({ avExcluded: [...list] });
    }
    return ok;
  });

  handle('gameAdopt', (_e, dir) => {
    const folder = path.resolve(String(dir ?? ''));
    if (!isInstall(folder)) throw new Error('That folder does not have RecFlare in it.');
    writeSettings({ folder });
    return status();
  });

  handle('gameInstall', (_e, { folder, source, username } = {}) =>
    job('install', async () => {
      const dir = path.resolve(String(folder || defaultDir()));
      fs.mkdirSync(dir, { recursive: true });
      if (source === 'steam') await fromSteam(dir, String(username ?? '').trim());
      else await fromMirror(dir);
      await installPatch(dir);
      writeSettings({ folder: dir });
      return status();
    }),
  );

  handle('gameRepair', () =>
    job('repair', async () => {
      const { folder } = readSettings();
      if (!folder || !fs.existsSync(path.join(folder, 'RecRoom.exe')))
        throw new Error('RecFlare is not installed. Install it first.');
      await installPatch(folder);
      return status();
    }),
  );

  handle('gamePatchInfo', async () => {
    const { folder } = readSettings();
    if (!folder || !isInstall(folder)) return null;
    const installed = readMarker(folder).patch ?? null;
    const latest = await latestPatch();
    return { installed, latest, updateAvailable: installed !== latest };
  });

  handle('gameNeedsPatch', async () => {
    const { folder } = readSettings();
    if (!folder || !isInstall(folder)) return false;
    return readMarker(folder).patch !== (await latestPatch());
  });

  handle('gamePlay', (_e, { vr } = {}) => {
    const { folder } = readSettings();
    if (!isInstall(folder)) throw new Error('RecFlare is not installed.');
    const bat = vr ? 'RecRoomVR.bat' : 'RecRoomScreen.bat';
    const child = spawn(path.join(SYSTEM, 'System32', 'cmd.exe'), ['/d', '/c', bat], {
      cwd: folder,
      windowsHide: true,
      stdio: ['ignore', 'pipe', 'pipe'],
    });
    let log = '';
    const grab = (d) => (log = (log + d.toString()).slice(-4000));
    child.stdout.on('data', grab);
    child.stderr.on('data', grab);
    child.on('error', (error) =>
      send({ running: false, error: `The game could not start: ${error.message}` }),
    );
    child.on('close', (code) => {
      if (!code) return;
      const line = log
        .split(/\r?\n/)
        .map((l) => l.trim())
        .filter(Boolean)
        .at(-1);
      send({ running: false, error: line || `The game failed to start (exit code ${code}).` });
    });
  });

  handle('gameCancel', () => {
    if (!task) return;
    task.cancelled = true;
    try {
      task.child?.kill();
    } catch {
    }
  });

  handle('gameOpenFolder', async () => {
    const { folder } = readSettings();
    if (folder && fs.existsSync(folder)) await shell.openPath(folder);
  });

  handle('gameUninstall', async () => {
    const { folder } = readSettings();
    if (!folder || !isInstall(folder)) throw new Error('RecFlare is not installed.');
    await fs.promises.rm(folder, { recursive: true, force: true });
    writeSettings({ folder: null });
    return status();
  });

  handle('gameRetireOldInstaller', async () => {
    const old = await oldInstall();
    const removed = [];
    const links = [
      path.join(app.getPath('appData'), 'Microsoft', 'Windows', 'Start Menu', 'Programs'),
      app.getPath('desktop'),
    ].flatMap((d) => ['RecFlare (Desktop mode).lnk', 'RecFlare (VR).lnk'].map((n) => path.join(d, n)));
    for (const link of links)
      if (fs.existsSync(link)) {
        fs.rmSync(link, { force: true });
        removed.push(link);
      }
    if (old) {
      for (const f of ['unins000.exe', 'unins000.dat'])
        if (fs.existsSync(path.join(old.dir, f))) fs.rmSync(path.join(old.dir, f), { force: true });
      if (old.registered) await run(REG, ['delete', OLD_INSTALLER_KEY, '/f']).catch(() => {});
    }
    return { removed: removed.length, uninstallEntry: !!old?.registered };
  });
}

module.exports = { register, oldInstall, isInstall };
