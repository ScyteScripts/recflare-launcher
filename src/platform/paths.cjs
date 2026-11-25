'use strict';
const path = require('path');
const os = require('os');

function profilePath(platform = process.platform, env = process.env, home = os.homedir()) {
  if (env.RECROOM_STUDIO_PATCH_CONFIG) return env.RECROOM_STUDIO_PATCH_CONFIG;
  // Match .NET SpecialFolder.LocalApplicationData used by the Studio SDK.
  const base = platform === 'win32' ? (env.LOCALAPPDATA || path.join(home, 'AppData', 'Local'))
    : platform === 'darwin' ? path.join(home, 'Library', 'Application Support')
    : (env.XDG_DATA_HOME || path.join(home, '.local', 'share'));
  return path.join(base, 'RecRoomStudioPatch', 'services.ini');
}
module.exports = {profilePath};
