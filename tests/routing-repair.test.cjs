'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const {root, framework, load} = require('./harness.cjs');
const {repairText, repairFiles, repairLauncherRouting, removeDevelopmentTemplate, completeTemplateReinstall,
  repairDevelopmentProfiles, repairDevelopmentProject, repairInstalledDevelopmentTemplates, needsDevelopmentTemplateReinstall, STUDIO_APP_ID} = require('../src/platform/routing-repair.cjs');
const {discoverProjects,hubProjects} = require('../src/platform/studio-project-discovery.cjs');
function fixture(t) {
  const boundary = path.join(root,'validation/unit'); fs.mkdirSync(boundary,{recursive:true});
  const dir = fs.mkdtempSync(path.join(boundary,'routing-'));
  t.after(() => {if (!path.resolve(dir).startsWith(path.resolve(boundary)+path.sep)) throw Error('Invalid fixture'); fs.rmSync(dir,{recursive:true,force:true});});
  return dir;
}
const broken = '[Launcher]\nUseInstalledTemplate=1\n[Network]\nServerUrl=not a URL\nStudioCdnUrl=https://cdn.dev.proot.app\n[Services]\nAuth=http://wrong.test/path\n[NETWORK]\nPatchKitKeysUrl=https://wrong.test\n[Other]\nKeep=yes\n';
test('repair resets every routing field and duplicate section, preserves other settings and is idempotent', () => {
  const text = repairText(broken);
  assert.match(text,/ServerUrl=https:\/\/ns.recflare.net/);
  assert.doesNotMatch(text,/dev.proot.app|wrong.test|not a URL/);
  assert.match(text,/UseInstalledTemplate=1/); assert.match(text,/Keep=yes/);
  assert.equal(text.match(/\[Network\]/g).length,1);
  assert.equal(repairText(text),text);
});
test('writes are atomic, accept UTF16, and leave no backups or temporary files', t => {
  const dir=fixture(t), file=path.join(dir,'services.ini');
  fs.writeFileSync(file,Buffer.from('\ufeff'+broken,'utf16le'));
  assert.deepEqual(repairFiles([file,file]),[file]);
  assert.match(fs.readFileSync(file,'utf8'),/Keep=yes/);
  assert.deepEqual(repairFiles([file]),[]);
  assert.deepEqual(fs.readdirSync(dir),['services.ini']);
});
test('a later write failure restores earlier files without backup files', t => {
  const dir=fixture(t), file=path.join(dir,'services.ini'), blocker=path.join(dir,'blocker');
  fs.writeFileSync(file,broken); fs.writeFileSync(blocker,'block');
  assert.throws(()=>repairFiles([file,path.join(blocker,'bad.ini')]),/repair failed/);
  assert.equal(fs.readFileSync(file,'utf8'),broken);
  assert.deepEqual(fs.readdirSync(dir).sort(),['blocker','services.ini']);
});
test('repairs active/default profiles and known Studio projects while preserving hash-protected template files and scenes', async t => {
  const dir=fixture(t), userData=path.join(dir,'state'), project=path.join(dir,'project'), legacy=path.join(dir,'legacy');
  for (const p of [project,legacy]) {
    fs.mkdirSync(path.join(p,'ProjectSettings/Packages/com.recroom.studio'),{recursive:true});
    fs.writeFileSync(path.join(p,'ProjectSettings/Packages/com.recroom.studio/BranchId.txt'),'branch');
    fs.writeFileSync(path.join(p,'ProjectSettings/RecRoomStudioPatch.ini'),broken);
    fs.writeFileSync(path.join(p,'scene.unity'),'authored');
  }
  fs.mkdirSync(userData); fs.writeFileSync(path.join(userData,'recent-studio-projects.json'),JSON.stringify({branch:project,stale:path.join(dir,'missing')}));
  const custom=path.join(dir,'custom.ini'); fs.writeFileSync(custom,broken);
  const app=path.join(dir,'app'), template=path.join(app,'RecRoomStudio/ProjectTemplate/ProjectSettings'); fs.mkdirSync(template,{recursive:true});
  fs.writeFileSync(path.join(template,'RecRoomStudioPatch.ini'),broken);
  const args={appId:STUDIO_APP_ID, configName:'fixture', userData, appRootDirPath:app, env:{LOCALAPPDATA:dir,RECROOM_STUDIO_PATCH_CONFIG:custom}, platform:'win32', registeredProjects:[legacy],searchRoots:[]};
  const result=await repairLauncherRouting(args);
  assert.equal(result.length,4);
  assert.equal(fs.readFileSync(path.join(template,'RecRoomStudioPatch.ini'),'utf8'),broken);
  for(const file of result) assert.doesNotMatch(fs.readFileSync(file,'utf8'),/dev.proot.app|wrong.test/);
  assert.equal(fs.readFileSync(path.join(project,'scene.unity'),'utf8'),'authored');
  assert.deepEqual(await repairLauncherRouting(args),[]);
  assert.deepEqual(await repairLauncherRouting({appId:'game'}),[]);
});
test('an open project is skipped and reported without blocking global routing repair', async t => {
  const dir=fixture(t), project=path.join(dir,'project');
  fs.mkdirSync(path.join(project,'ProjectSettings/Packages/com.recroom.studio'),{recursive:true});
  fs.writeFileSync(path.join(project,'ProjectSettings/Packages/com.recroom.studio/BranchId.txt'),'branch');
  fs.mkdirSync(path.join(project,'Temp'));fs.writeFileSync(path.join(project,'Temp/UnityLockfile'),'');
  const projectIni=path.join(project,'ProjectSettings/RecRoomStudioPatch.ini');fs.writeFileSync(projectIni,broken);
  const profile=path.join(dir,'custom.ini');fs.writeFileSync(profile,broken);
  const warnings=[];
  await repairLauncherRouting({appId:STUDIO_APP_ID,userData:path.join(dir,'state'),env:{LOCALAPPDATA:dir,RECROOM_STUDIO_PATCH_CONFIG:profile},platform:'win32',registeredProjects:[project],searchRoots:[],onWarning:w=>warnings.push(w)});
  assert.equal(fs.readFileSync(profile,'utf8'),repairText(broken));
  assert.equal(warnings[0].code,'PROJECT_OPEN');
  assert.equal(fs.readFileSync(projectIni,'utf8'),broken);
  const report=JSON.parse(fs.readFileSync(path.join(dir,'state/logs/routing-repair.json'),'utf8'));
  assert.equal(report.completed,true);assert.equal(report.warnings[0].code,'PROJECT_OPEN');
});
test('inaccessible search folders are reported while accessible projects remain discoverable',async t=>{
  const dir=fixture(t), good=path.join(dir,'good'), bad=path.join(dir,'bad');
  fs.mkdirSync(path.join(good,'RecRoomStudioPackages'),{recursive:true});fs.mkdirSync(bad);
  fs.writeFileSync(path.join(good,'RecRoomStudioPackages/VersionInfo.json'),'{}');
  const mockFs={...fs,promises:{...fs.promises,lstat:async file=>{
    if(file===bad)throw Object.assign(new Error('denied'),{code:'EACCES'});
    return fs.promises.lstat(file);
  }}};
  const discovery=load('src/platform/studio-project-discovery.cjs',{fs:mockFs});
  const warnings=[];
  assert.deepEqual(Array.from(await discovery.discoverProjects([dir],{onWarning:(file,error)=>warnings.push({file,code:error.code})})),[good]);
  assert.equal(warnings[0].code,'EACCES');
});
test('global profile write failures remain fatal and produce safe diagnostics',async t=>{
  const dir=fixture(t),profile=path.join(dir,'directory-not-config');fs.mkdirSync(profile);
  const userData=path.join(dir,'state');
  await assert.rejects(repairLauncherRouting({appId:STUDIO_APP_ID,userData,env:{LOCALAPPDATA:dir,RECROOM_STUDIO_PATCH_CONFIG:profile},platform:'win32',registeredProjects:[],searchRoots:[]}));
  const report=JSON.parse(fs.readFileSync(path.join(userData,'logs/routing-repair.json'),'utf8'));
  assert.equal(report.completed,false);assert.equal(report.warnings[0].stage,'global-routing');
  assert.equal('message' in report.warnings[0],false);
});
test('filesystem discovery finds unknown Studio projects, skips caches and plain Unity projects, and avoids duplicate roots',async t=>{
  const dir=fixture(t);
  for(const name of ['unknown/deep/studio','Library/ignored','ordinary']) {
    const project=path.join(dir,name);fs.mkdirSync(path.join(project,'Packages'),{recursive:true});fs.mkdirSync(path.join(project,'ProjectSettings'),{recursive:true});
    fs.writeFileSync(path.join(project,'Packages/manifest.json'),JSON.stringify({dependencies:{[name==='ordinary'?'com.unity.test':'com.recroom.studio']:'1'}}));
  }
  const found=await discoverProjects([dir,path.join(dir,'unknown')]);
  assert.deepEqual(found,[path.join(dir,'unknown/deep/studio')]);
  await assert.rejects(discoverProjects([dir],{maxDirectories:1}),/directory limit/);
  const args={appId:STUDIO_APP_ID,userData:path.join(dir,'state'),env:{LOCALAPPDATA:dir},platform:'win32',registeredProjects:[],searchRoots:[dir]};
  const repaired=await repairLauncherRouting(args);
  assert(repaired.includes(path.join(dir,'unknown/deep/studio/ProjectSettings/RecRoomStudioPatch.ini')));
  assert.equal(fs.existsSync(path.join(dir,'ordinary/ProjectSettings/RecRoomStudioPatch.ini')),false);
});
test('Hub project discovery accepts path-keyed and nested project records without scanning unrelated values',t=>{
  const dir=fixture(t), hub=path.join(dir,'UnityHub');fs.mkdirSync(hub);
  const external=path.join(dir,'external');
  fs.writeFileSync(path.join(hub,'projects-v1.json'),JSON.stringify({[external]:{title:'project'},projects:[{projectPath:path.join(dir,'another')}],other:'ignore'}));
  assert.deepEqual(hubProjects({APPDATA:dir},'win32').sort(),[external,path.join(dir,'another')].sort());
});
test('development template is deleted as a whole without touching sibling install or authored files',t=>{
  const dir=fixture(t), template=path.join(dir,'RecRoomStudio/ProjectTemplate'), config=path.join(template,'ProjectSettings/RecRoomStudioPatch.ini');
  fs.mkdirSync(path.dirname(config),{recursive:true});fs.writeFileSync(config,'[Services]\nStudio=https://studio.dev.proot.app\n');
  fs.writeFileSync(path.join(template,'sdk.tgz'),'contaminated');fs.writeFileSync(path.join(dir,'app_data.json'),'installed');
  fs.mkdirSync(path.join(dir,'authored'));fs.writeFileSync(path.join(dir,'authored/scene.unity'),'keep');
  assert.equal(removeDevelopmentTemplate(dir),true);
  assert.equal(fs.existsSync(template),false);
  assert.equal(fs.readFileSync(path.join(dir,'app_data.json'),'utf8'),'installed');
  assert.equal(fs.readFileSync(path.join(dir,'authored/scene.unity'),'utf8'),'keep');
  assert.equal(removeDevelopmentTemplate(dir),false);
  assert.throws(()=>removeDevelopmentTemplate('relative'),/Invalid Studio install/);
});
test('production templates and comments are preserved; template folders are never treated as unknown projects',async t=>{
  const dir=fixture(t), template=path.join(dir,'RecRoomStudio/ProjectTemplate'), config=path.join(template,'ProjectSettings/RecRoomStudioPatch.ini');
  fs.mkdirSync(path.dirname(config),{recursive:true});
  fs.writeFileSync(config,'; https://ns.dev.proot.app\n[Network]\nServerUrl=https://ns.recflare.net\n');
  fs.mkdirSync(path.join(template,'RecRoomStudioPackages'));fs.writeFileSync(path.join(template,'RecRoomStudioPackages/VersionInfo.json'),'{}');
  assert.equal(removeDevelopmentTemplate(dir),false);
  assert.deepEqual(await discoverProjects([dir,template]),[]);
});
test('a template junction outside the install is never deleted',t=>{
  const dir=fixture(t), outside=path.join(dir,'outside'), install=path.join(dir,'install');
  fs.mkdirSync(path.join(outside,'ProjectSettings'),{recursive:true});
  fs.writeFileSync(path.join(outside,'ProjectSettings/RecRoomStudioPatch.ini'),'[Network]\nServerUrl=https://ns.dev.proot.app\n');
  fs.mkdirSync(path.join(install,'RecRoomStudio'),{recursive:true});
  fs.symlinkSync(outside,path.join(install,'RecRoomStudio/ProjectTemplate'),process.platform==='win32'?'junction':'dir');
  assert.throws(()=>removeDevelopmentTemplate(install),/Refusing to delete/);
  assert.equal(fs.existsSync(path.join(outside,'ProjectSettings/RecRoomStudioPatch.ini')),true);
});
test('automatic profile/project fixes change only development routing and create no backup files',t=>{
  const dir=fixture(t), active=path.join(dir,'active.ini'), normal=path.join(dir,'RecRoomStudioPatch/services.ini');
  fs.mkdirSync(path.dirname(normal));fs.writeFileSync(active,'[Network]\nWebUrl=https://www.dev.proot.app\n');
  const clean='; preserve exact bytes\r\n[Network]\r\nServerUrl=https://ns.recflare.net\r\n';fs.writeFileSync(normal,clean);
  assert.deepEqual(repairDevelopmentProfiles({env:{LOCALAPPDATA:dir,RECROOM_STUDIO_PATCH_CONFIG:active},platform:'win32'}),[active]);
  assert.equal(fs.readFileSync(normal,'utf8'),clean);
  assert.deepEqual(repairDevelopmentProfiles({env:{LOCALAPPDATA:dir,RECROOM_STUDIO_PATCH_CONFIG:active},platform:'win32'}),[]);
  const project=path.join(dir,'project'),config=path.join(project,'ProjectSettings/RecRoomStudioPatch.ini');
  fs.mkdirSync(path.dirname(config),{recursive:true});fs.writeFileSync(config,'[Services]\nStudio=https://studio.dev.proot.app\n');
  assert.deepEqual(repairDevelopmentProject(project),[config]);
  assert.doesNotMatch(fs.readFileSync(config,'utf8'),/dev.proot.app/);
  assert.deepEqual(fs.readdirSync(path.dirname(config)),['RecRoomStudioPatch.ini']);
});
test('automatic template reinstall is scheduled for contamination and interrupted downloads, then stops after success',async t=>{
  const dir=fixture(t), template=path.join(dir,'RecRoomStudio/ProjectTemplate'), config=path.join(template,'ProjectSettings/RecRoomStudioPatch.ini');
  fs.mkdirSync(path.dirname(config),{recursive:true});fs.writeFileSync(config,'[Network]\nServerUrl=https://ns.dev.proot.app\n');
  const calls=[], args={registeredApps:{[STUDIO_APP_ID]:{registeredBranches:{main:{rootDir:{path:dir},installedVersionId:171}}}},startRepair:async args=>calls.push(args)};
  await repairInstalledDevelopmentTemplates(args);assert.equal(calls.length,1);
  removeDevelopmentTemplate(dir);assert.equal(needsDevelopmentTemplateReinstall(dir),true);
  await repairInstalledDevelopmentTemplates(args);assert.equal(calls.length,2);
  assert.throws(()=>completeTemplateReinstall(dir),/did not complete/);
  fs.mkdirSync(path.dirname(config),{recursive:true});fs.writeFileSync(config,'[Network]\nServerUrl=https://ns.recflare.net\n');
  completeTemplateReinstall(dir);assert.equal(needsDevelopmentTemplateReinstall(dir),false);
  await repairInstalledDevelopmentTemplates(args);assert.equal(calls.length,2);
});
test('automatic project checks run before Unity launch and block launching if the fix fails',async()=>{
  const calls=[];
  const dependencies={
    '@recovered/routing-repair':{repairDevelopmentProfiles:()=>calls.push('profiles'),repairDevelopmentProject:()=>calls.push('project')},
    './$':framework({$childprocess:{spawn:()=>{calls.push('spawn');const p=new(require('events').EventEmitter)();p.unref=()=>{};queueMicrotask(()=>p.emit('spawn'));return p;}}}),
    './fetch-unity-editor-exe-path':{fetchUnityEditorExePath:async()=>'/fixture/Unity'},
  };
  await load('src/launcher/open-unity-project.js',dependencies).openUnityProject({unityProjectDirPath:'/fixture/project'});
  assert.deepEqual(calls,['profiles','project','spawn']);
  calls.length=0;dependencies['@recovered/routing-repair'].repairDevelopmentProject=()=>{throw Error('locked');};
  await assert.rejects(load('src/launcher/open-unity-project.js',dependencies).openUnityProject({unityProjectDirPath:'/fixture/project'}),/locked/);
  assert.deepEqual(calls,['profiles']);
});
test('Windows repair reads branch project paths from the launcher registry and surfaces registry failures', async t => {
  const dir=fixture(t), project=path.join(dir,'project'), config=path.join(project,'ProjectSettings/RecRoomStudioPatch.ini');
  fs.mkdirSync(path.join(project,'RecRoomStudioPackages'),{recursive:true});fs.writeFileSync(path.join(project,'RecRoomStudioPackages/VersionInfo.json'),'{}');
  fs.mkdirSync(path.dirname(config));fs.writeFileSync(config,broken);
  let registryError=false;
  class Registry {
    static HKCU='HKCU';static REG_SZ='REG_SZ';
    constructor(args){assert.equal(args.key,'\\Software\\Rec Room Inc.\\fixture');}
    keyExists(callback){callback(registryError?Error('access denied'):null,true);}
    values(callback){callback(null,[{name:'recroom_branch_123_last_studio_unity_project_dir_path',type:'REG_SZ',value:project}]);}
  }
  const module=load('src/platform/routing-repair.cjs',{'./paths.cjs':require('../src/platform/paths.cjs'),'./studio-project-discovery.cjs':require('../src/platform/studio-project-discovery.cjs'),'../../config/product.json':require('../config/product.json'),winreg:Registry});
  const args={appId:STUDIO_APP_ID,configName:'fixture',userData:path.join(dir,'state'),env:{LOCALAPPDATA:dir},platform:'win32',searchRoots:[]};
  await module.repairLauncherRouting(args);
  assert.equal(fs.readFileSync(config,'utf8'),repairText(broken));
  registryError=true;
  const warnings=[];await module.repairLauncherRouting({...args,onWarning:w=>warnings.push(w)});
  assert.equal(warnings[0].stage,'project-registry');
});
function repairWorkflow({routingFailure=false, completionFailure=false, updaterFailure=false, projectWarning=false}={}) {
  const events=[], taskBox={value:{}}, ongoing={value:{}}, cancellations={value:{}};
  const fw=framework({PatchKit:{startAppRepairTask:async()=>{events.push('updater');return {appRepairTaskId:1};},fetchAppRepairTaskState:async()=>({isFinished:true,errorTypeName:updaterFailure?'AppRepairTaskHasBeenCancelled':undefined})}});
  fw.__.FatalError = class FatalError extends Error {};
  fw.__.$electron={dialog:{showMessageBox:async()=>events.push('partial-project-notice')}};
  const deps={
    './$':fw,'./config':{CONFIG:{value:{name:'fixture'}}},
    '@recovered/routing-repair':{STUDIO_APP_ID:'app',removeDevelopmentTemplate:()=>events.push('template-check'),completeTemplateReinstall:()=>{},repairLauncherRouting:async args=>{events.push(args.appRootDirPath?'routing-after':'routing-before');if(routingFailure || (completionFailure&&args.appRootDirPath))throw Error('routing failed');if(projectWarning&&args.onWarning)args.onWarning({code:'EACCES'});}},
    './apps-branches-ongoing-task-id-box':{APPS_BRANCHES_ONGOING_TASK_ID_BOX:ongoing},
    './apps-branches-repair-tasks-cancellation-token-box':{APPS_BRANCHES_REPAIR_TASKS_CANCELLATION_TOKEN_BOX:cancellations},
    './gen-app-branch-new-task-id':{genAppBranchNewTaskId:()=>1},
    './fetch-app-branch-internal-state':{fetchAppBranchInternalState:async()=>({rootDirState:{arePermissionsSufficient:true,path:'fixture-app'}})},
    './retrieve-app-branch-info':{retrieveAppBranchInfo:async()=>{events.push('metadata');return {secret:'fixture'};}},
    './enqueue-stats-app-branch-entry':{enqueueStatsAppBranchEntry:async()=>{}},
    './stats-app-branch-entry-type':{StatsAppBranchEntryType:{}},
    './fetch-app-branch-task-state':{fetchAppBranchTaskState:async()=>taskBox.value.app.branch[1].value},
    './apps-branches-tasks-state-box':{APPS_BRANCHES_TASKS_STATE_BOX:taskBox},
  };
  return {fn:load('src/vendor/patchkit-launcher2-base@0.0.57/dist/start-app-branch-repair-task.js',deps).startAppBranchRepairTask,events,taskBox,ongoing};
}
test('Repair normalizes routing before metadata and again after updater completion',async()=>{
  const f=repairWorkflow();await f.fn({appId:'app',appBranchId:'branch'});
  await new Promise(resolve=>setImmediate(resolve));
  assert.deepEqual(f.events,['routing-before','metadata','template-check','updater','routing-after']);
  assert.equal(f.taskBox.value.app.branch[1].value.isFinished,true);
  assert.equal(f.taskBox.value.app.branch[1].value.errorTypeName,undefined);
});
test('routing failure aborts before downloads; final routing failure cannot report repair success',async()=>{
  const f=repairWorkflow({routingFailure:true});await assert.rejects(f.fn({appId:'app',appBranchId:'branch'}),/routing failed/);
  assert.deepEqual(f.events,['routing-before']);
  const g=repairWorkflow({completionFailure:true});await g.fn({appId:'app',appBranchId:'branch'});await new Promise(resolve=>setImmediate(resolve));
  assert.equal(g.taskBox.value.app.branch[1].value.errorTypeName,'FatalError');
  assert.equal(g.ongoing.value.app.branch.value,undefined);
});
test('a failed or canceled updater never runs the post-repair routing step',async()=>{
  const f=repairWorkflow({updaterFailure:true});await f.fn({appId:'app',appBranchId:'branch'});await new Promise(resolve=>setImmediate(resolve));
  assert.deepEqual(f.events,['routing-before','metadata','template-check','updater']);
});
test('project-search warnings show a partial-check notice without changing successful template repair to failure',async()=>{
  const f=repairWorkflow({projectWarning:true});await f.fn({appId:'app',appBranchId:'branch'});await new Promise(resolve=>setImmediate(resolve));
  assert.equal(f.taskBox.value.app.branch[1].value.isFinished,true);
  assert.equal(f.taskBox.value.app.branch[1].value.errorTypeName,undefined);
  assert.equal(f.events.at(-1),'partial-project-notice');
});
