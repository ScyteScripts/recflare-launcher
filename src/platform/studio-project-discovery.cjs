'use strict';
const fs = require('fs');
const path = require('path');
const os = require('os');
const skip = new Set(['library','temp','logs','obj','node_modules','appdata','windows','program files','program files (x86)','programdata','$recycle.bin','system volume information','recroomstudioofflinecache','recroomstudiopackages','projectsettings','packages','projecttemplate','projecttemplates','build','builds']);
async function isStudioProject(directory) {
  for (const marker of ['ProjectSettings/Packages/com.recroom.studio/BranchId.txt','RecRoomStudioPackages/VersionInfo.json']) {
    try {if ((await fs.promises.stat(path.join(directory,marker))).isFile()) return true;}
    catch (error) {if (!['ENOENT','ENOTDIR'].includes(error.code)) throw error;}
  }
  try {
    const manifest = JSON.parse(await fs.promises.readFile(path.join(directory,'Packages/manifest.json'),'utf8'));
    return Object.keys(manifest.dependencies || {}).some(name => name === 'com.recroom.studio' || name.startsWith('com.recroom.studio.'));
  } catch (error) {if (['ENOENT','ENOTDIR'].includes(error.code) || error instanceof SyntaxError) return false; throw error;}
}
function hubProjects(env = process.env, platform = process.platform) {
  const home = env.USERPROFILE || env.HOME || os.homedir();
  const appData = platform === 'win32' ? env.APPDATA || path.join(home,'AppData/Roaming') : platform === 'darwin' ? path.join(home,'Library/Application Support') : env.XDG_CONFIG_HOME || path.join(home,'.config');
  const results = new Set();
  for (const filename of ['projects-v1.json','projects.json']) {
    const file = path.join(appData,'UnityHub',filename);
    let data;
    try {data = JSON.parse(fs.readFileSync(file,'utf8'));}
    catch (error) {if (error.code === 'ENOENT' || error instanceof SyntaxError) continue; throw error;}
    function visit(value) {
      if (!value || typeof value !== 'object') return;
      for (const [key,item] of Object.entries(value)) {
        if (path.isAbsolute(key)) results.add(key);
        if (['path','projectPath','projectDir'].includes(key) && typeof item === 'string' && path.isAbsolute(item)) results.add(item);
        if (item && typeof item === 'object') visit(item);
      }
    }
    visit(data);
  }
  return [...results];
}
async function discoverProjects(roots, {maxDirectories = 100000, onWarning} = {}) {
  const found = [], pending = [...roots], seen = new Set();
  while (pending.length) {
    const directory = path.resolve(pending.pop());
    if (['projecttemplate','projecttemplates'].includes(path.basename(directory).toLowerCase())) continue;
    const key = process.platform === 'win32' ? directory.toLowerCase() : directory;
    if (seen.has(key)) continue;
    seen.add(key);
    if (seen.size > maxDirectories) {
      const error=new Error('Studio project search exceeded its directory limit.'); error.code='SEARCH_LIMIT';
      if (!onWarning) throw error;
      onWarning(directory,error);break;
    }
    try {
      // Never traverse symlinks/junctions, including the selected roots themselves.
      const info = await fs.promises.lstat(directory);
      if (!info.isDirectory() || info.isSymbolicLink()) continue;
      if (await isStudioProject(directory)) {found.push(directory); continue;}
      const entries = await fs.promises.readdir(directory,{withFileTypes:true});
      for (const entry of entries) {
        if (entry.isDirectory() && !entry.isSymbolicLink() && !entry.name.startsWith('.') && !skip.has(entry.name.toLowerCase()))
          pending.push(path.join(directory,entry.name));
      }
    } catch (error) {
      if (['ENOENT','ENOTDIR'].includes(error.code)) continue;
      if (onWarning) {onWarning(directory,error);continue;}
      throw new Error('Cannot search Studio project location: ' + directory, {cause:error});
    }
  }
  return found;
}
module.exports = {isStudioProject, hubProjects, discoverProjects};
