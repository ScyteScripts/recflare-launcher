'use strict';
const { app } = require('electron');
// Keep the package ID and userData directory stable across the visible rename.
const existingUserData = app.getPath('userData');
app.setName('RecFlare Launcher');
// Smooth wheel scrolling stays; only the rubber-band bounce at the ends is turned off.
app.commandLine.appendSwitch('disable-features', 'ElasticOverscroll,OverscrollHistoryNavigation');
app.setPath('userData', existingUserData);
const fs = require('fs');
const path = require('path');
require('./network.cjs').attach(require('electron'));
// Portable is the default. OS integration is an explicit operator action.
const integrated = require('./platform/installation.cjs').isIntegrated(path.dirname(app.getPath('exe')));
if (process.env.STUDIO_LAUNCHER_VALIDATION_ROOT || (!integrated && !process.argv.includes('--register-integration'))) {
  const root = path.resolve(process.env.STUDIO_LAUNCHER_VALIDATION_ROOT || path.join(path.dirname(app.getPath('exe')), 'data'));
  process.env.STUDIO_LAUNCHER_PORTABLE_ROOT = root;
  const userData = path.join(root, 'electron-user-data');
  fs.mkdirSync(userData, { recursive: true });
  app.setPath('userData', userData);
}
process.on('unhandledRejection', error => console.error('Launcher asynchronous failure:', error));
if (process.env.STUDIO_LAUNCHER_SMOKE_REPORT) require('./smoke.cjs').attach();
try {require('./platform/routing-repair.cjs').repairDevelopmentProfiles();}
catch (error) {
  require('electron').dialog.showErrorBox('Studio routing could not be fixed', 'Close Unity and check that your Studio config files are writable, then restart the launcher.');
  app.exit(1);
  throw error;
}
// RecFlare's own handlers (sign in, rooms, links, window buttons), then the Studio engine.
require('./recflare/main.cjs');
require('./launcher/main.js');
