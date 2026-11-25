import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import {fileURLToPath} from 'node:url';
import {createRequire} from 'node:module';
const require=createRequire(import.meta.url);
const {downloadArtifact}=require('@electron/get');
const extract=require('extract-zip');
const root=path.resolve(path.dirname(fileURLToPath(import.meta.url)),'..');
const config=JSON.parse(fs.readFileSync(path.join(root,'config/runtime.json')));
if(!/^\d+\.\d+\.\d+$/.test(config.version)||config.platform!=='win32'||config.arch!=='x64')throw new Error('Unsupported runtime configuration');
const destination=path.join(root,'build/runtimes',config.version+'-win32-x64');
if(!fs.existsSync(path.join(destination,'runtime-manifest.json'))) {
  // @electron/get verifies the archive against Electron's published SHASUMS256.txt.
  const archive=await downloadArtifact({...config,artifactName:'electron'});
  await extract(archive,{dir:destination});
  const files=[];
  function walk(dir){for(const name of fs.readdirSync(dir).sort()){const p=path.join(dir,name);if(fs.statSync(p).isDirectory())walk(p);else files.push({path:path.relative(destination,p).replaceAll('\\','/'),sha256:crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex')});}}
  walk(destination);
  fs.writeFileSync(path.join(destination,'runtime-manifest.json'),JSON.stringify({config,archiveSha256:crypto.createHash('sha256').update(fs.readFileSync(archive)).digest('hex'),files},null,2)+'\n');
}
console.log('Verified upstream runtime: '+destination);
