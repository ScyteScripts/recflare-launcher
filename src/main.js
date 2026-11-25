const { app, BrowserWindow, ipcMain, shell } = require('electron');
const path = require('node:path');

app.commandLine.appendSwitch('disable-features', 'ElasticOverscroll,OverscrollHistoryNavigation');

let win = null;

function createWindow() {
  win = new BrowserWindow({
    width: 1240,
    height: 780,
    minWidth: 1000,
    minHeight: 640,
    frame: false,
    backgroundColor: '#0c0d10',
    show: false,
    title: 'RecFlare Launcher',
    icon: path.join(__dirname, 'renderer', 'img', 'icon.png'),
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      sandbox: true,
      nodeIntegration: false,
    },
  });
  win.removeMenu();
  win.loadFile(path.join(__dirname, 'renderer', 'index.html'));
  win.once('ready-to-show', () => win.show());
  win.webContents.on('will-navigate', (e) => e.preventDefault());
  win.webContents.setWindowOpenHandler(() => ({ action: 'deny' }));
  win.on('closed', () => {
    win = null;
  });
}

ipcMain.handle('window:control', (_e, action) => {
  if (!win) return;
  if (action === 'minimize') win.minimize();
  else if (action === 'maximize') win.isMaximized() ? win.unmaximize() : win.maximize();
  else if (action === 'close') win.close();
});

const OPENABLE = new Set(['www.recflare.net', 'join.recflare.net', 'github.com']);
ipcMain.handle('open:external', (_e, url) => {
  try {
    const u = new URL(String(url));
    if (u.protocol === 'https:' && OPENABLE.has(u.hostname)) return shell.openExternal(u.href);
  } catch {
  }
  return undefined;
});

if (!app.requestSingleInstanceLock()) app.quit();
else {
  app.on('second-instance', () => {
    if (!win) return;
    if (win.isMinimized()) win.restore();
    win.focus();
  });
  app.whenReady().then(createWindow);
  app.on('window-all-closed', () => app.quit());
}
