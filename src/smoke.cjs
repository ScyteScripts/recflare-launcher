'use strict';
// Opt-in validation instrumentation, inactive during ordinary launcher use.
exports.attach = () => {
  const fs = require('fs');
  const { app, ipcMain } = require('electron');
  const report = {status: 'starting', runtime: {electron:process.versions.electron,node:process.versions.node}, windows: [], failures: []};
  const handlers = [];
  const handle = ipcMain.handle.bind(ipcMain);
  ipcMain.handle = (name, listener) => {handlers.push(name); return handle(name,listener);};
  const write = () => fs.writeFileSync(process.env.STUDIO_LAUNCHER_SMOKE_REPORT, JSON.stringify(report, null, 2));
  write();
  process.on('uncaughtException', error => { report.failures.push(error.stack); write(); app.exit(1); });
  process.on('unhandledRejection', error => { report.failures.push(String(error.stack || error)); write(); });
  app.on('browser-window-created', (_event, window) => {
    window.webContents.on('did-fail-load', (_e, code, message) => { report.failures.push({code, message}); write(); });
    window.webContents.on('did-finish-load', async () => {
      try {
        await new Promise(resolve => setTimeout(resolve, 5000));
        report.windows.push({url: window.webContents.getURL(), title: window.getTitle(),
          content: await window.webContents.executeJavaScript('document.body.innerText'),
          bridge: await window.webContents.executeJavaScript('({id:window.patchKitLauncher2Id,version:window.patchKitLauncher2VersionLabel,api:typeof window.sendPatchKitLauncher2ApiFuncRequest})')});
        const shot = await window.webContents.capturePage();
        fs.writeFileSync(process.env.STUDIO_LAUNCHER_SMOKE_REPORT + '.png', shot.toPNG());
        report.ipcFunctions = handlers.sort();
        if (process.env.STUDIO_LAUNCHER_FIXTURE_ORIGIN) report.nativeWorkflows = await require('./native-validation.cjs').run();
        report.status = report.failures.length ? 'failed' : 'loaded'; write(); app.exit(report.failures.length ? 1 : 0);
      } catch (e) { report.status='failed'; report.failures.push(e.stack); write(); app.exit(1); }
    });
  });
  setTimeout(() => { report.status = 'timeout'; write(); app.exit(1); }, 45000);
};
