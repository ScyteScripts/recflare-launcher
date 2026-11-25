'use strict';
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const {TextDecoder} = require('util');
const {profilePath} = require('./paths.cjs');
const product = require('../../config/product.json');
const {hubProjects, discoverProjects, isStudioProject} = require('./studio-project-discovery.cjs');
const os = require('os');
const STUDIO_APP_ID = '8ec38942-fea4-44bd-b5bb-cc75c77935f0';

// Reset every routing override, including malformed URLs and duplicate sections.
// Non-routing sections and user-authored project files are preserved.
function repairText(text) {
  let routing = false;
  const lines = [];
  for (const line of text.replace(/^\uFEFF/, '').split(/\r?\n/)) {
    const section = /^\[([^\]]+)\]$/.exec(line.trim());
    if (section) routing = ['network', 'services'].includes(section[1].trim().toLowerCase());
    if (!routing) lines.push(line);
  }
  const prefix = lines.join('\n').replace(/\n+$/, '');
  return (prefix ? prefix + '\n\n' : '') + '[Network]\n' +
    `ServerUrl=${product.serverOrigin}\n` +
    'StudioCdnUrl=\nWebUrl=\nPatchKitApiUrl=\nPatchKitCacheUrl=\nPatchKitKeysUrl=\n' +
    'PatchKitAppSecretSha256=f31e174f7cd5991d0e45db176e6283bfe04a74d91a95929c465e2edace7770e6\n\n[Services]\n';
}
function decode(bytes) {
  const utf16 = bytes[0] === 0xff && bytes[1] === 0xfe ? 'utf-16le' : bytes[0] === 0xfe && bytes[1] === 0xff ? 'utf-16be' : 'utf-8';
  return new TextDecoder(utf16, {fatal:true}).decode(bytes);
}
function read(file) {
  try {return fs.readFileSync(file);} catch (error) {if (error.code === 'ENOENT') return null; throw error;}
}
function atomicWrite(file, bytes) {
  fs.mkdirSync(path.dirname(file), {recursive:true});
  const temporary = file + '.routing-write-' + crypto.randomUUID();
  try {
    fs.writeFileSync(temporary, bytes, {flag:'wx', mode:0o600});
    fs.renameSync(temporary, file);
  } finally {if (fs.existsSync(temporary)) fs.unlinkSync(temporary);}
}
function containsDevelopmentRouting(text) {
  let section = '';
  for (const raw of text.replace(/^\uFEFF/, '').split(/\r?\n/)) {
    const line = raw.trim(); if (!line || /^[;#]/.test(line)) continue;
    const heading = /^\[([^\]]+)\]$/.exec(line);
    if (heading) {section=heading[1].trim().toLowerCase();continue;}
    if (['network','services'].includes(section) && /^[^=]+=.*https?:\/\/(?:[a-z0-9-]+\.)*dev\.proot\.app(?=[:/\s]|$)/i.test(line)) return true;
  }
  return false;
}
function repairFiles(files, {developmentOnly = false} = {}) {
  const unique = [...new Map(files.map(file => {
    const resolved = path.resolve(file);
    return [process.platform === 'win32' ? resolved.toLowerCase() : resolved, resolved];
  })).values()];
  const plans = unique.map(file => {
    const before = read(file);
    return {file, before, after:Buffer.from(repairText(before === null ? '' : decode(before)), 'utf8')};
  }).filter(p => (!developmentOnly || (p.before !== null && containsDevelopmentRouting(decode(p.before)))) && (p.before === null || !p.before.equals(p.after)));
  const written = [];
  try {
    for (const p of plans) {
      const current = read(p.file);
      if ((current === null) !== (p.before === null) || (current !== null && !current.equals(p.before)))
        throw new Error('Studio config changed during repair. Retry Repair.');
      atomicWrite(p.file, p.after);
      written.push(p);
    }
  } catch (error) {
    let restoreFailed = false;
    for (const p of written.reverse()) {
      try {if (p.before === null) fs.unlinkSync(p.file); else atomicWrite(p.file, p.before);}
      catch {restoreFailed = true;}
    }
    throw new Error(restoreFailed ? 'Studio routing repair failed and could not restore every config. Run Repair again after checking file permissions.' : 'Studio routing repair failed; no config changes retained. Check file permissions and retry.', {cause:error});
  }
  return plans.map(p => p.file);
}
function projectConfig(project) {
  if (typeof project !== 'string' || !path.isAbsolute(project)) return undefined;
  if (['projecttemplate','projecttemplates'].includes(path.basename(project).toLowerCase())) return undefined;
  if (!fs.existsSync(path.join(project, 'ProjectSettings'))) return undefined;
  if (fs.existsSync(path.join(project, 'Temp/UnityLockfile')))
    throw Object.assign(new Error('Close Unity before repairing Studio routing.'), {code:'PROJECT_OPEN'});
  return path.join(project, 'ProjectSettings/RecRoomStudioPatch.ini');
}
function developmentTemplatePath(appRootDirPath) {
  if (!appRootDirPath || !path.isAbsolute(appRootDirPath)) throw new Error('Invalid Studio install path.');
  const root = path.resolve(appRootDirPath);
  const template = path.resolve(root, 'RecRoomStudio', 'ProjectTemplate');
  // Validate the resolved absolute target immediately before any recursive deletion.
  if (path.relative(root, template) !== path.join('RecRoomStudio','ProjectTemplate')) throw new Error('Invalid Studio template path.');
  if (!fs.existsSync(template)) return false;
  const actualRoot = fs.realpathSync(root), actualTemplate = fs.realpathSync(template);
  if (path.relative(actualRoot, actualTemplate) !== path.join('RecRoomStudio','ProjectTemplate') || fs.lstatSync(template).isSymbolicLink())
    throw new Error('Studio template is linked outside its install. Refusing to delete it.');
  const bytes = read(path.join(template,'ProjectSettings/RecRoomStudioPatch.ini'));
  if (bytes === null) return false;
  return containsDevelopmentRouting(decode(bytes)) ? template : false;
}
const reinstallMarker = root => path.join(root,'.recflare-template-reinstall');
function needsDevelopmentTemplateReinstall(root) {
  return !!developmentTemplatePath(root) || fs.existsSync(reinstallMarker(root));
}
function removeDevelopmentTemplate(appRootDirPath) {
  const template = developmentTemplatePath(appRootDirPath);
  if (!template) return false;
  fs.writeFileSync(reinstallMarker(appRootDirPath), 'Reinstall template from RecFlare packaging.\n', {mode:0o600});
  fs.rmSync(template,{recursive:true,force:false});
  return true;
}
function completeTemplateReinstall(root) {
  if (!fs.existsSync(reinstallMarker(root))) return;
  if (developmentTemplatePath(root) || !fs.existsSync(path.join(root,'RecRoomStudio/ProjectTemplate/ProjectSettings/RecRoomStudioPatch.ini')))
    throw new Error('RecFlare template reinstall did not complete.');
  fs.unlinkSync(reinstallMarker(root));
}
function repairDevelopmentProfiles({env = process.env, platform = process.platform} = {}) {
  return repairFiles([profilePath(platform,env),profilePath(platform,{...env,RECROOM_STUDIO_PATCH_CONFIG:undefined})],{developmentOnly:true});
}
function repairDevelopmentProject(project) {
  const file=path.join(project,'ProjectSettings/RecRoomStudioPatch.ini'), bytes=read(file);
  if (bytes === null || !containsDevelopmentRouting(decode(bytes))) return [];
  const config=projectConfig(project);
  return config ? repairFiles([config],{developmentOnly:true}) : [];
}
async function windowsProjects(configName) {
  const WinReg = require('winreg');
  const registry = new WinReg({hive:WinReg.HKCU, key:'\\Software\\Rec Room Inc.\\' + configName});
  const exists = await new Promise((resolve,reject) => registry.keyExists((error,value) => error ? reject(error) : resolve(value)));
  if (!exists) return [];
  const values = await new Promise((resolve,reject) => registry.values((error,value) => error ? reject(error) : resolve(value)));
  return values.filter(v => /^recroom_branch_.+_last_studio_unity_project_dir_path$/.test(v.name) && v.type === WinReg.REG_SZ).map(v => v.value);
}
async function repairLauncherRouting({appId, configName, userData, env = process.env, platform = process.platform, registeredProjects, searchRoots, discoverUnknown = true, developmentOnly = false, onWarning = () => {}}) {
  if (appId !== STUDIO_APP_ID) return [];
  userData ??= require('electron').app.getPath('userData');
  let projects = [], fromHub = [], changed = [];
  const report={utc:new Date().toISOString(),completed:false,repairedFiles:changed,warnings:[]};
  const warn=(stage,file,error)=>{
    let cause=error;while(cause.cause)cause=cause.cause;
    const warning={stage,path:file,code:cause.code || error.code || 'CHECK_FAILED',type:cause.name || 'Error'};
    report.warnings.push(warning);onWarning(warning);
  };
  try {
    // Critical service profiles must be corrected even if an unrelated project
    // cannot be inspected. Their write failures still abort the repair.
    changed.push(...repairFiles([profilePath(platform,env),profilePath(platform,{...env,RECROOM_STUDIO_PATCH_CONFIG:undefined})],{developmentOnly}));
    try {
      const recent=read(path.join(userData,'recent-studio-projects.json'));
      if(recent!==null) {
        const value=JSON.parse(decode(recent));
        if(!value || typeof value!=='object' || Array.isArray(value))throw Error('Invalid recent projects');
        projects=Object.values(value);
      }
    } catch(error) {warn('recent-projects',path.join(userData,'recent-studio-projects.json'),error);}
    try {projects.push(...(registeredProjects ?? (platform==='win32' ? await windowsProjects(configName) : [])));}
    catch(error) {warn('project-registry','Windows Studio project registry',error);}
    if(discoverUnknown && searchRoots===undefined) {
      try {fromHub=hubProjects(env,platform);}catch(error){warn('hub-projects','Unity Hub project list',error);}
    }
    // Only projects the launcher already knows about and Unity Hub's own list: no crawl of the
    // whole user folder.
    const roots=discoverUnknown ? searchRoots ?? [...fromHub] : [];
    projects.push(...await discoverProjects(roots,{onWarning:(file,error)=>warn('project-search',file,error)}));
    for(const project of new Set(projects)) {
      if(typeof project!=='string' || !path.isAbsolute(project))continue;
      try {
        if(!await isStudioProject(project))continue;
        if(developmentOnly) {
          const bytes=read(path.join(project,'ProjectSettings/RecRoomStudioPatch.ini'));
          if(bytes===null || !containsDevelopmentRouting(decode(bytes)))continue;
        }
        const config=projectConfig(project);
        if(config)changed.push(...repairFiles([config],{developmentOnly}));
      } catch(error) {warn('project-routing',project,error);}
    }
    report.completed=true;
    return changed;
  } catch(error) {warn('global-routing','Studio global profiles',error);throw error;}
  finally {
    // Fixed-format diagnostics: never serialize INI contents, tokens, or raw exceptions.
    try {atomicWrite(path.join(userData,'logs',developmentOnly?'routing-auto-repair.json':'routing-repair.json'),Buffer.from(JSON.stringify(report,null,2)+'\n'));}
    catch {console.warn('Could not save Studio routing repair diagnostics.');}
  }
}
async function repairInstalledDevelopmentTemplates({registeredApps, startRepair}) {
  for (const [appId,app] of Object.entries(registeredApps)) {
    if (appId !== STUDIO_APP_ID) continue;
    for (const [appBranchId,branch] of Object.entries(app.registeredBranches || {})) {
      if (branch.ongoingTaskId !== undefined || branch.installedVersionId === undefined) continue;
      if (needsDevelopmentTemplateReinstall(branch.rootDir.path)) await startRepair({appId,appBranchId});
    }
  }
}
module.exports = {windowsProjects, repairText, repairFiles, repairLauncherRouting, removeDevelopmentTemplate, completeTemplateReinstall,
  needsDevelopmentTemplateReinstall, repairDevelopmentProfiles, repairDevelopmentProject, repairInstalledDevelopmentTemplates, STUDIO_APP_ID};
