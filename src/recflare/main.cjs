'use strict';
const { app, BrowserWindow, ipcMain, shell, safeStorage } = require('electron');
const fs = require('node:fs');
const path = require('node:path');

function handle(name, fn) {
  ipcMain.handle(`RecFlareLauncher.${name}`, async (event, args) => {
    try {
      return { execResult: await fn(event, args) };
    } catch (error) {
      return {
        execErrorInfo: { typeFullname: 'RecFlareLauncher.Error', message: String(error?.message ?? error) },
      };
    }
  });
}

handle('windowControl', (event, action) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  if (!win) return;
  if (action === 'minimize') win.minimize();
  else if (action === 'maximize') win.isMaximized() ? win.unmaximize() : win.maximize();
  else if (action === 'close') win.close();
});

const OPENABLE = new Set([
  'www.recflare.net',
  'recflare.net',
  'join.recflare.net',
  'github.com',
  'unity.com',
]);
handle('openExternal', (_event, url) => {
  try {
    const u = new URL(String(url));
    if (u.protocol === 'https:' && OPENABLE.has(u.hostname)) return shell.openExternal(u.href);
  } catch {
  }
  return undefined;
});

const AUTH = 'https://auth.recflare.net';
const ACCOUNTS = 'https://accounts.recflare.net';
const CLIENT = { client_id: 'recroom.studio', client_secret: '54oc5C4BH7jpqp42KhChkNZOLP0U3yTx' };
const sessionFile = () => path.join(app.getPath('userData'), 'session.bin');

let accessToken = null;
let pending = null;

function saveRefresh(token) {
  if (!token) return;
  if (!safeStorage.isEncryptionAvailable()) return;
  fs.writeFileSync(sessionFile(), safeStorage.encryptString(token));
}
function loadRefresh() {
  try {
    return safeStorage.decryptString(fs.readFileSync(sessionFile()));
  } catch {
    return null;
  }
}
function forget() {
  accessToken = null;
  fs.rmSync(sessionFile(), { force: true });
}

async function form(url, fields) {
  const res = await fetch(url, {
    method: 'POST',
    headers: { 'content-type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams(fields),
  });
  const body = await res.json().catch(() => ({}));
  return { ok: res.ok, body };
}

function tokenRoles() {
  try {
    const claims = JSON.parse(Buffer.from(accessToken.split('.')[1], 'base64url').toString('utf8'));
    const role = claims.role ?? claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    return Array.isArray(role) ? role.map(String) : role ? [String(role)] : [];
  } catch {
    return [];
  }
}

async function me() {
  const res = await fetch(`${ACCOUNTS}/account/me`, { headers: { authorization: `Bearer ${accessToken}` } });
  if (!res.ok) throw new Error(`account/me ${res.status}`);
  const a = await res.json();
  return {
    studio: tokenRoles().includes('betastudio'),
    accountId: a.accountId,
    username: a.username ?? '',
    displayName: a.displayName || a.username || 'Player',
    profileImage: a.profileImage || null,
  };
}

function takeTokens(body) {
  accessToken = body.access_token;
  saveRefresh(body.refresh_token);
}

function send(channel, payload) {
  for (const w of BrowserWindow.getAllWindows())
    if (!w.isDestroyed()) w.webContents.send('RecFlareLauncher.event', { channel, payload });
}

handle('authRestore', async () => {
  const refresh = loadRefresh();
  if (!refresh) return null;
  try {
    const { ok, body } = await form(`${AUTH}/connect/token`, { grant_type: 'refresh_token', refresh_token: refresh });
    if (!ok || !body.access_token) {
      if (body.error === 'invalid_grant') forget();
      return null;
    }
    takeTokens(body);
    return await me();
  } catch {
    return null;
  }
});

handle('authStart', async () => {
  if (pending) pending.cancelled = true;
  const { ok, body } = await form(`${AUTH}/connect/deviceauthorization`, CLIENT);
  if (!ok || !body.device_code) throw new Error(body.error_description || 'could not start sign in');
  const job = { cancelled: false };
  pending = job;
  const url = new URL(String(body.verification_uri_complete));
  if (url.protocol === 'https:' && url.hostname === 'www.recflare.net') shell.openExternal(url.href);

  (async () => {
    let interval = Math.max(5, Number(body.interval) || 5) * 1000;
    const until = Date.now() + (Number(body.expires_in) || 600) * 1000;
    while (!job.cancelled && Date.now() < until) {
      await new Promise((r) => setTimeout(r, interval));
      if (job.cancelled) return;
      let res;
      try {
        res = await form(`${AUTH}/connect/token`, {
          ...CLIENT,
          grant_type: 'urn:ietf:params:oauth:grant-type:device_code',
          device_code: body.device_code,
        });
      } catch {
        continue;
      }
      if (job.cancelled) return;
      if (res.ok && res.body.access_token) {
        takeTokens(res.body);
        pending = null;
        try {
          send('auth:changed', { account: await me() });
        } catch {
          send('auth:changed', { error: 'Signed in, but your account could not be loaded.' });
        }
        return;
      }
      const err = res.body.error;
      if (err === 'authorization_pending') continue;
      if (err === 'slow_down') {
        interval += 5000;
        continue;
      }
      pending = null;
      send('auth:changed', {
        error: err === 'access_denied' ? 'Sign in was denied.' : 'The sign in code expired. Try again.',
      });
      return;
    }
    if (!job.cancelled) {
      pending = null;
      send('auth:changed', { error: 'The sign in code expired. Try again.' });
    }
  })();

  return { userCode: body.user_code, url: url.href };
});

const ROOMS = 'https://rooms.recflare.net';
async function authed(url) {
  const res = await fetch(url, { headers: { authorization: `Bearer ${accessToken}` } });
  if (!res.ok) return null;
  return res.json();
}
handle('roomsMine', async () => {
  if (!accessToken) return null;
  const [favorites, visited, featured] = await Promise.all([
    authed(`${ROOMS}/rooms/favoritedby/me?skip=0&take=24`).catch(() => null),
    authed(`${ROOMS}/rooms/visitedby/me?skip=0&take=40`).catch(() => null),
    authed(`${ROOMS}/featuredrooms/current`).catch(() => null),
  ]);
  const favs = Array.isArray(favorites) ? favorites : [];
  const seen = new Map();
  for (const r of [...favs, ...(Array.isArray(visited) ? visited : [])])
    if (r && typeof r.RoomId === 'number' && !seen.has(r.RoomId)) seen.set(r.RoomId, r);
  const candidates = [...seen.values()];
  const cheered = [];
  for (let i = 0; i < candidates.length; i += 8) {
    const batch = candidates.slice(i, i + 8);
    const states = await Promise.all(
      batch.map((r) => authed(`${ROOMS}/rooms/${r.RoomId}/interactionby/me`).catch(() => null)),
    );
    batch.forEach((r, j) => states[j]?.Cheered === true && cheered.push(r));
  }
  return {
    favorites: favs,
    cheered,
    featured: Array.isArray(featured?.Rooms) ? featured.Rooms : [],
  };
});

handle('authCancel', () => {
  if (pending) pending.cancelled = true;
  pending = null;
});

handle('authSignout', () => {
  if (pending) pending.cancelled = true;
  pending = null;
  forget();
});

handle('openWindowsSecurity', () => shell.openExternal('windowsdefender://threat'));

handle('openFolder', async (_event, dirPath) => {
  const target = path.resolve(String(dirPath ?? ''));
  const stat = await fs.promises.stat(target).catch(() => null);
  if (!stat?.isDirectory()) throw new Error('That folder no longer exists.');
  const error = await shell.openPath(target);
  if (error) throw new Error(error);
});

const os = require('node:os');
const { execFile } = require('node:child_process');
const discovery = require('../platform/studio-project-discovery.cjs');
const game = require('./game.cjs');
game.register(handle);
require('./update.cjs').register(handle);

const STUDIO_KEY = 'HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\RecRoomStudioSourceLauncher';
const REG = path.join(process.env.SystemRoot || 'C:\\Windows', 'System32', 'reg.exe');

function regValue(key, name) {
  return new Promise((resolve) =>
    execFile(REG, ['query', key, '/v', name], { windowsHide: true }, (error, out) => {
      if (error) return resolve(null);
      const m = new RegExp(name + '\\s+REG_\\w+\\s+(.+)').exec(String(out));
      resolve(m ? m[1].trim() : null);
    }),
  );
}

async function oldStudioLauncher() {
  const local = process.env.LOCALAPPDATA || path.join(os.homedir(), 'AppData', 'Local');
  const dir = ((await regValue(STUDIO_KEY, 'InstallLocation')) ?? path.join(local, 'Programs', 'recroom-studio-launcher')).replace(/[\\/]+$/, '');
  const here = path.dirname(app.getPath('exe')).toLowerCase();
  if (dir.toLowerCase() === here) return null;
  const exe = ['Rec Room Studio Launcher.exe', 'RecFlare Studio Launcher.exe'].find((name) => fs.existsSync(path.join(dir, name)));
  const template = oldStudioTemplate(dir);
  if (!exe && !template) return null;
  const uninstaller = exe
    ? ['Uninstall Source Launcher.exe', 'Uninstall Rec Room Studio Launcher.exe']
        .map((name) => path.join(dir, name))
        .find((file) => fs.existsSync(file))
    : undefined;
  return { dir, uninstaller: uninstaller ?? null, template };
}

function oldStudioTemplate(dir) {
  const apps = path.join(dir, 'Apps');
  let names = [];
  try {
    names = fs.readdirSync(apps, { withFileTypes: true }).filter((e) => e.isDirectory()).map((e) => e.name);
  } catch {
    return null;
  }
  const found = names
    .map((name) => path.join(apps, name))
    .find((folder) => fs.existsSync(path.join(folder, 'app_data.json')) && fs.existsSync(path.join(folder, 'RecRoomStudio')));
  return found ?? null;
}

async function oldStudioProjects() {
  const candidates = new Set(discovery.hubProjects());
  try {
    for (const dir of await require('../platform/routing-repair.cjs').windowsProjects('Rec Room Studio Launcher'))
      candidates.add(dir);
  } catch {
  }
  try {
    const recent = JSON.parse(
      fs.readFileSync(path.join(app.getPath('appData'), 'recroom-studio-launcher', 'recent-studio-projects.json'), 'utf8'),
    );
    for (const dir of Object.values(recent ?? {})) if (typeof dir === 'string') candidates.add(dir);
  } catch {
  }
  const found = [];
  for (const dir of candidates)
    if (path.isAbsolute(dir) && (await discovery.isStudioProject(dir).catch(() => false))) found.push(dir);
  return [...new Set(found)].sort();
}

handle('findOldStudioTemplate', async () => (await oldStudioLauncher())?.template ?? null);

handle('adoptStudioFolder', async (_event, { appId, appBranchId, dir } = {}) => {
  const folder = path.resolve(String(dir ?? ''));
  let data;
  try {
    data = JSON.parse(fs.readFileSync(path.join(folder, 'app_data.json'), 'utf8'));
  } catch {
    throw new Error('That folder does not have a RecFlare Studio install in it.');
  }
  if (data.is_installed !== true) throw new Error('The Studio install in that folder did not finish. Install a fresh copy instead.');
  const P = require('../launcher/$').__.PatchKitLauncher2;
  await P.retrieveAppBranchInfo({ appId, appBranchId });
  const settings = await P.fetchSettings();
  const apps = { ...(settings.apps ?? {}) };
  const app_ = { ...(apps[appId] ?? {}) };
  app_.branches = { ...(app_.branches ?? {}), [appBranchId]: { ...(app_.branches?.[appBranchId] ?? {}), rootDirPath: folder } };
  apps[appId] = app_;
  await P.saveSettingsFileStore({ settings: { ...settings, apps } });
  await P.fetchAppBranchInternalState({ appId, appBranchId }).catch(() => {});
  return { installedVersionId: data.installed_version_id ?? null };
});

handle('findOldInstalls', async () => ({
  game: await game.oldInstall(),
  studioLauncher: await oldStudioLauncher(),
  projects: await oldStudioProjects(),
}));

handle('retireOldStudioLauncher', async () => {
  const old = await oldStudioLauncher();
  if (!old?.uninstaller) throw new Error('The old Studio launcher was not found.');
  await new Promise((resolve, reject) =>
    execFile(old.uninstaller, ['/S'], { cwd: old.dir }, (error) => (error ? reject(error) : resolve())),
  );
});
