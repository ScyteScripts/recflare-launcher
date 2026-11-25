'use strict';
const { app, BrowserWindow } = require('electron');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const { spawn } = require('node:child_process');
const { Readable } = require('node:stream');
const { pipeline } = require('node:stream/promises');
const { isIntegrated } = require('../platform/installation.cjs');

const REPO = 'ScyteScripts/recflare-launcher';
const SETUP = 'RecFlareLauncher-Setup.exe';

function currentVersion() {
  try {
    return JSON.parse(fs.readFileSync(path.join(__dirname, 'recflare-version.json'), 'utf8')).version;
  } catch {
    return '0.0.0';
  }
}
function newer(a, b) {
  const pa = String(a).replace(/^v/, '').split('.').map(Number);
  const pb = String(b).replace(/^v/, '').split('.').map(Number);
  for (let i = 0; i < 3; i++) if ((pa[i] || 0) !== (pb[i] || 0)) return (pa[i] || 0) > (pb[i] || 0);
  return false;
}
const installDir = () => path.dirname(app.getPath('exe'));
const canSelfUpdate = () => process.platform === 'win32' && isIntegrated(installDir());

function send(payload) {
  for (const w of BrowserWindow.getAllWindows())
    if (!w.isDestroyed()) w.webContents.send('RecFlareLauncher.event', { channel: 'launcher:update', payload });
}

async function latestRelease() {
  const res = await fetch(`https://api.github.com/repos/${REPO}/releases/latest`, {
    headers: { accept: 'application/vnd.github+json' },
  });
  if (!res.ok) throw new Error(`Could not check for launcher updates (HTTP ${res.status}).`);
  const release = await res.json();
  const asset = (name) => (release.assets ?? []).find((a) => a.name === name)?.browser_download_url;
  return {
    version: String(release.tag_name ?? '').replace(/^v/, ''),
    notes: String(release.body ?? ''),
    url: release.html_url,
    setup: asset(SETUP),
    sha: asset(`${SETUP}.sha256`),
  };
}

let busy = false;

function register(handle) {
  handle('launcherVersion', () => ({ version: currentVersion(), canSelfUpdate: canSelfUpdate() }));

  handle('launcherCheckUpdate', async () => {
    const current = currentVersion();
    const latest = await latestRelease();
    return {
      current,
      latest: latest.version,
      available: !!latest.version && newer(latest.version, current) && !!latest.setup && !!latest.sha,
      notes: latest.notes,
      url: latest.url,
      canSelfUpdate: canSelfUpdate(),
    };
  });

  handle('launcherInstallUpdate', async () => {
    if (busy) throw new Error('The update is already downloading.');
    if (!canSelfUpdate()) throw new Error('This copy of the launcher is not installed, so it cannot update itself.');
    busy = true;
    try {
      const latest = await latestRelease();
      if (!latest.setup || !latest.sha || !newer(latest.version, currentVersion()))
        throw new Error('There is no newer launcher to install.');
      const expected = (await (await fetch(latest.sha)).text()).trim().split(/\s+/)[0].toLowerCase();
      if (!/^[0-9a-f]{64}$/.test(expected)) throw new Error('The update has no valid checksum.');

      const file = path.join(app.getPath('temp'), `RecFlareLauncher-Setup-${latest.version}.exe`);
      const res = await fetch(latest.setup, { redirect: 'follow' });
      if (!res.ok || !res.body) throw new Error(`The update did not download (HTTP ${res.status}).`);
      const total = Number(res.headers.get('content-length')) || 0;
      let done = 0;
      let last = 0;
      const hash = crypto.createHash('sha256');
      const body = Readable.fromWeb(res.body);
      body.on('data', (chunk) => {
        hash.update(chunk);
        done += chunk.length;
        if (Date.now() - last > 250) {
          last = Date.now();
          send({ downloading: true, pct: total ? (done / total) * 100 : undefined });
        }
      });
      await pipeline(body, fs.createWriteStream(file));
      if (hash.digest('hex') !== expected) {
        fs.rmSync(file, { force: true });
        throw new Error('The downloaded update did not match its checksum, so it was not installed.');
      }
      send({ downloading: false, installing: true });

      const exe = app.getPath('exe');
      const cmd = path.join(process.env.SystemRoot || 'C:\\Windows', 'System32', 'cmd.exe');
      const line = `start "" /wait "${file}" /S /D=${installDir()} & start "" "${exe}"`;
      spawn(cmd, ['/d', '/s', '/c', line], { detached: true, stdio: 'ignore', windowsHide: true, windowsVerbatimArguments: true }).unref();
      setTimeout(() => app.quit(), 500);
    } finally {
      busy = false;
    }
  });
}

module.exports = { register, currentVersion, newer };
