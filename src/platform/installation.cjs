'use strict';
const fs = require('fs');
const path = require('path');
function isIntegrated(directory) {
  const file = path.join(directory,'source-launcher-install.ini');
  if (!fs.existsSync(file)) return false;
  const bytes = fs.readFileSync(file);
  const text = bytes.toString(bytes[0]===0xff && bytes[1]===0xfe ? 'utf16le' : 'utf8');
  return /^Product=RecRoomStudioSourceLauncher\s*$/im.test(text) && /^NoIntegration=0\s*$/im.test(text);
}
module.exports = {isIntegrated};
