'use strict';
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

function readSetting(section, key) {
  const config = process.env.RECROOM_STUDIO_PATCH_CONFIG || path.join(process.env.LOCALAPPDATA, 'RecRoomStudioPatch', 'services.ini');
  if (!fs.existsSync(config)) return undefined;
  let current = ''; let value;
  for (const raw of fs.readFileSync(config, 'utf8').split(/\r?\n/)) {
    const line = raw.trim();
    if (!line || /^[;#]/.test(line)) continue;
    if (line.startsWith('[') && line.endsWith(']')) { current = line.slice(1, -1).trim().toLowerCase(); continue; }
    const equals = line.indexOf('=');
    if (equals > 0 && current === section.toLowerCase() && line.slice(0, equals).trim().toLowerCase() === key.toLowerCase()) value = line.slice(equals + 1).trim();
  }
  return value;
}

function manifest() {
  return JSON.parse(fs.readFileSync(path.join(process.resourcesPath, 'studio-patch-manifest.json'), 'utf8'));
}

function resourceUrl(resource, originalOrigin, setting) {
  const match = /^1\/apps\/([^/]+)(?:\/.*)?$/.exec(resource);
  const origin = readSetting('Network', setting) || (['PatchKitApiUrl','PatchKitCacheUrl'].includes(setting) ? 'https://studio.recflare.net' : undefined);
  if (!match || !origin) return originalOrigin + '/' + resource;
  if (crypto.createHash('sha256').update(match[1]).digest('hex') !== manifest().installedAppSecretSha256) return originalOrigin + '/' + resource;
  const url = new URL(origin);
  if (!['https:', 'http:'].includes(url.protocol) || url.username || url.password || url.pathname !== '/' || url.search || url.hash) throw new Error('Studio distribution service must be an HTTP(S) origin.');
  return url.origin + '/' + resource;
}

function installedResource(resource) {
  if (readSetting('Launcher', 'UseInstalledTemplate') !== '1') return undefined;
  const match = /^1\/apps\/([^/]+)(?:\/(.*))?$/.exec(resource);
  if (!match) return undefined;
  const info = manifest();
  if (crypto.createHash('sha256').update(match[1]).digest('hex') !== info.installedAppSecretSha256) return undefined;
  const root = path.join(process.env.STUDIO_LAUNCHER_PORTABLE_ROOT || path.resolve(process.resourcesPath, '..'), 'Apps', 'Rec Room Studio');
  const metadata = JSON.parse(fs.readFileSync(path.join(root, 'app_data.json'), 'utf8'));
  if (!metadata.is_installed || ![info.templateVersion, info.distributionVersion].includes(metadata.installed_version_id)) throw new Error('Studio installed-template version does not match this patch.');
  const version = metadata.installed_version_id;
  const suffix = match[2] || '';
  if (suffix === 'versions/latest/id') return JSON.stringify({ id: version });
  if (suffix === 'versions/' + version) return JSON.stringify({ label: 'Installed template ' + version, changelog: 'Use the locally installed, patched Studio template. Remote updates are disabled in installed-template mode.' });
  if (suffix === 'changelog') return JSON.stringify({ versions: [{ vid: version, label: 'Installed template ' + version }] });
  if (suffix === '') return JSON.stringify({ display_name: 'RecFlare Studio', patcher_whitelabel: false });
  return undefined;
}

function validateTemplate(source, version) {
  const info=manifest();
  const release=info.templateReleases?.find(r=>r.version===version);
  const files=release ? release.files : info.templateFiles;
  for (const target of files) {
    const file = path.resolve(source, target.path);
    if (!file.startsWith(path.resolve(source) + path.sep)) throw new Error('Invalid template manifest path');
    const hash = crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
    if (hash !== target.sha256) throw new Error('RecFlare Studio package changed after patching: ' + target.path + '. Install or repair the matching Studio template before creating a project.');
  }
}

module.exports = { readSetting, resourceUrl, installedResource, validateTemplate };
