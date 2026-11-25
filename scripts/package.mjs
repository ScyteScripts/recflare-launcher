import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import {createRequire} from 'node:module';
import { createAsar } from './asar.mjs';
import { build, root, sha, verifyInputs, appOut } from './build.mjs';

import {buildUpdater} from './build-updater.mjs';
const isDebugArtifact = file => /\.(?:map|pdb|mdb)$/i.test(file);

function removeStaleDebugFiles(directory) {
  const boundary=fs.realpathSync(directory)+path.sep;
  function walk(dir) {
    for(const entry of fs.readdirSync(dir,{withFileTypes:true})) {
      const file=path.join(dir,entry.name);
      if(entry.isSymbolicLink())throw new Error('Release output must not contain links');
      if(entry.isDirectory())walk(file);
      else if(isDebugArtifact(file)) {
        if(!fs.realpathSync(file).startsWith(boundary))throw new Error('Debug file escaped release directory');
        fs.unlinkSync(file);
      }
    }
  }
  walk(directory);
}
export async function pack() {
  buildUpdater();
  await build();
  verifyInputs();
  const dest = path.join(root, 'dist/RecFlare Launcher');
  fs.mkdirSync(path.join(dest, 'resources'), {recursive: true});
  const runtimeConfig=JSON.parse(fs.readFileSync(path.join(root,'config/runtime.json')));
  const runtime=path.join(root,'build/runtimes',runtimeConfig.version+'-win32-x64');
  const runtimeManifest=JSON.parse(fs.readFileSync(path.join(runtime,'runtime-manifest.json')));
  for(const f of runtimeManifest.files) {
    if(sha(fs.readFileSync(path.join(runtime,f.path)))!==f.sha256)throw new Error('Runtime hash mismatch: '+f.path);
    if(f.path==='resources/default_app.asar')continue;
    const rel=f.path==='electron.exe'?'RecFlare Launcher.exe':f.path;
    const target=path.join(dest,rel);fs.mkdirSync(path.dirname(target),{recursive:true});fs.copyFileSync(path.join(runtime,f.path),target);
  }
  fs.cpSync(path.join(root,'assets/launcher'),path.join(dest,'resources/assets'),{recursive:true});
  fs.copyFileSync(path.join(root,'config/launcher.json'),path.join(dest,'resources/config.json'));
  fs.copyFileSync(path.join(root,'config/template-manifest.json'),path.join(dest,'resources/studio-patch-manifest.json'));
  const generatedConfig = JSON.parse(fs.readFileSync(path.join(dest,'resources/config.json')));
  generatedConfig.version = JSON.parse(fs.readFileSync(path.join(root,'package.json'))).version;
  fs.writeFileSync(path.join(dest,'resources/config.json'),JSON.stringify(generatedConfig));
  const templateManifest=JSON.parse(fs.readFileSync(path.join(dest,'resources/studio-patch-manifest.json')));
  templateManifest.templateReleases=JSON.parse(fs.readFileSync(path.join(root,'config/template-releases.json'))).releases;
  fs.writeFileSync(path.join(dest,'resources/studio-patch-manifest.json'),JSON.stringify(templateManifest,null,2)+'\n');
  const icons=createRequire(import.meta.url)('png2icons');
  const iconSource=fs.readFileSync(path.join(root,'assets/recflare-icon.png'));
  fs.writeFileSync(path.join(dest,'resources/assets/icon'),iconSource);
  const launcherIcon=icons.createICO(iconSource,icons.BICUBIC2,0,false,true);
  if(!launcherIcon)throw new Error('Could not build the Studio shortcut icon');
  fs.writeFileSync(path.join(dest,'resources/launcher.ico'),launcherIcon);
  const updaterDest=path.join(dest,'resources/updater');
  fs.cpSync(path.join(root,'build/updater'),updaterDest,{recursive:true,filter:file=>!isDebugArtifact(file)});
  // Incremental packaging must also remove symbols left by an earlier build.
  removeStaleDebugFiles(updaterDest);
  createAsar(appOut, path.join(dest, 'resources/app.asar'),{exclude:isDebugArtifact});
  fs.copyFileSync(path.join(root, 'config/services.ini.example'), path.join(dest, 'services.ini.example'));
  fs.writeFileSync(path.join(dest, 'Launch.cmd'), '@echo off\r\nsetlocal\r\nif exist "%~dp0services.ini" set "RECROOM_STUDIO_PATCH_CONFIG=%~dp0services.ini"\r\nstart "" "%~dp0RecFlare Launcher.exe" %*\r\n');
  fs.copyFileSync(path.join(root,'THIRD_PARTY_NOTICES.md'),path.join(dest,'THIRD_PARTY_NOTICES.md'));
  const files = [];
  function walk(dir) {
    for (const name of fs.readdirSync(dir).sort()) {
      const file = path.join(dir, name), rel = path.relative(dest, file).replaceAll('\\', '/');
      if (rel === 'data' || rel === 'debug.log' || rel === 'services.ini') continue;
      if (fs.statSync(file).isDirectory()) walk(file);
      else if (rel !== 'build-manifest.json') files.push({path: rel, size: fs.statSync(file).size, sha256: sha(fs.readFileSync(file))});
    }
  }
  walk(dest);
  if(files.some(f=>isDebugArtifact(f.path)))throw new Error('Debug artifact leaked into release payload');
  fs.writeFileSync(path.join(dest, 'build-manifest.json'), JSON.stringify({version: '3.0.0', releaseDebugFiles:false, electron:runtimeConfig.version, updater:'source-built-dotnet-rpc', files}, null, 2) + '\n');
  console.log('Packaged portable launcher: ' + dest);
  return dest;
}
if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) await pack();
