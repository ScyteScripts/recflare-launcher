'use strict';
const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const crypto=require('node:crypto');
const {root,framework,load,child}=require('./harness.cjs');
const launcher='src/launcher/';
const unity=require('../src/platform/unity.cjs');
const windowsUnity={...unity,findInstalledEditor:()=>undefined,hubArgs:a=>unity.hubArgs(a,'win32'),editorExecutable:p=>unity.editorExecutable(p,'win32')};
const fixtureRoot=path.join(root,'validation/unit');fs.mkdirSync(fixtureRoot,{recursive:true});
function fixture(t){const dir=fs.mkdtempSync(path.join(fixtureRoot,'case-'));t.after(()=>fs.rmSync(dir,{recursive:true,force:true}));return dir;}
function hash(b){return crypto.createHash('sha256').update(b).digest('hex');}
function policy(t){const dir=fixture(t),resources=path.join(dir,'resources');fs.mkdirSync(resources);const profile=path.join(dir,'services.ini');fs.writeFileSync(profile,'');const app='fixture-app';const info={installedAppSecretSha256:hash(app),templateVersion:169,distributionVersion:171,templateFiles:[{path:'Packages/sdk.tgz',sha256:hash('sdk')}]};fs.writeFileSync(path.join(resources,'studio-patch-manifest.json'),JSON.stringify(info));const process={resourcesPath:resources,env:{LOCALAPPDATA:dir,RECROOM_STUDIO_PATCH_CONFIG:profile}};return {dir,resources,profile,app,info,policy:load('src/policy.cjs',{}, {process})};}

test('routing scopes origins to the measured Studio application and preserves unrelated URLs',t=>{
 const f=policy(t);
 for (const key of ['PatchKitApiUrl','PatchKitCacheUrl']) assert.equal(f.policy.resourceUrl('1/apps/'+f.app+'/versions/latest/id','https://api2.patchkit.net',key),'https://studio.recflare.net/1/apps/fixture-app/versions/latest/id');
 assert.equal(f.policy.resourceUrl('1/apps/other','https://api2.patchkit.net','PatchKitApiUrl'),'https://api2.patchkit.net/1/apps/other');
 fs.writeFileSync(f.profile,'[Network]\nPatchKitApiUrl=https://studio.example.test\n');
 assert.equal(f.policy.resourceUrl('1/apps/'+f.app+'/versions/latest/id','https://api2.patchkit.net','PatchKitApiUrl'),'https://studio.example.test/1/apps/fixture-app/versions/latest/id');
 assert.equal(f.policy.resourceUrl('1/apps/other','https://api2.patchkit.net','PatchKitApiUrl'),'https://api2.patchkit.net/1/apps/other');
 fs.writeFileSync(f.profile,'[Network]\nPatchKitApiUrl=https://studio.example.test/unexpected\n');
 assert.throws(()=>f.policy.resourceUrl('1/apps/'+f.app,'https://api2.patchkit.net','PatchKitApiUrl'),/origin/);
});

test('template guards select the exact approved release and continue rejecting changed files',t=>{
 const f=policy(t),source=path.join(f.dir,'template');fs.mkdirSync(path.join(source,'Packages'),{recursive:true});
 fs.writeFileSync(path.join(source,'Packages/sdk.tgz'),'hosted-sdk');
 f.info.templateReleases=[{version:171,files:[{path:'Packages/sdk.tgz',sha256:hash('hosted-sdk')}]}];
 fs.writeFileSync(path.join(f.resources,'studio-patch-manifest.json'),JSON.stringify(f.info));
 assert.doesNotThrow(()=>f.policy.validateTemplate(source,171));
 assert.throws(()=>f.policy.validateTemplate(source,169),/package changed/);
 fs.writeFileSync(path.join(source,'Packages/sdk.tgz'),'changed-sdk');
 assert.throws(()=>f.policy.validateTemplate(source,171),/package changed/);
});

test('installed-template mode requires a matching installed version and does not intercept downloads',t=>{
 const f=policy(t);fs.writeFileSync(f.profile,'[Launcher]\nUseInstalledTemplate=1\n');
 const appRoot=path.join(f.dir,'Apps/Rec Room Studio');fs.mkdirSync(appRoot,{recursive:true});
 fs.writeFileSync(path.join(appRoot,'app_data.json'),JSON.stringify({is_installed:true,installed_version_id:171}));
 assert.equal(JSON.parse(f.policy.installedResource('1/apps/'+f.app+'/versions/latest/id')).id,171);
 assert.equal(f.policy.installedResource('1/apps/'+f.app+'/versions/171/content_urls'),undefined);
 fs.writeFileSync(path.join(appRoot,'app_data.json'),JSON.stringify({is_installed:true,installed_version_id:999}));
 assert.throws(()=>f.policy.installedResource('1/apps/'+f.app),/version/);
});

test('project creation copies validated template and branch metadata before opening',async t=>{
 const f=policy(t),template=path.join(f.dir,'app/RecRoomStudio/ProjectTemplate'),dest=path.join(f.dir,'project');
 fs.mkdirSync(path.join(template,'Packages'),{recursive:true});fs.writeFileSync(path.join(template,'Packages/sdk.tgz'),'sdk');
 const calls=[];const fw=framework({PatchKitLauncher2:{fetchAppBranchInternalState:async()=>({installedVersionId:171,rootDirState:{path:path.join(f.dir,'app')}})}});
 const fn=load(launcher+'create-unity-project-from-recroom-studio-template.js',{'./$':fw,'@recovered/policy':f.policy,'./make-sure-that-unity-hub-process-doesnt-exist':{makeSureThatUnityHubProcessDoesntExist:async()=>calls.push('hub-check')},'./open-unity-project':{openUnityProject:async args=>calls.push(args)}}).createUnityProjectFromRecRoomStudioTemplate;
 await fn({appId:'app',appBranchId:'branch',unityEditorId:{version:'6000.0.27f1'},unityProjectDirPath:dest});
 assert.equal(fs.readFileSync(path.join(dest,'Packages/sdk.tgz'),'utf8'),'sdk');
 assert.equal(fs.readFileSync(path.join(dest,'ProjectSettings/Packages/com.recroom.studio/BranchId.txt'),'utf8'),'branch');
 assert.equal(calls[0],'hub-check');assert.equal(calls[1].unityProjectDirPath,dest);
 await assert.rejects(fn({appId:'app',appBranchId:'branch',unityProjectDirPath:dest}),/new or empty directory/);
 assert.equal(fs.readFileSync(path.join(dest,'Packages/sdk.tgz'),'utf8'),'sdk');
 fs.writeFileSync(path.join(template,'Packages/sdk.tgz'),'corrupt');
 await assert.rejects(fn({appId:'app',appBranchId:'branch',unityProjectDirPath:path.join(f.dir,'bad')}),/package changed/);
 assert.equal(fs.existsSync(path.join(f.dir,'bad')),false);
});

test('template metadata selects the pinned Unity editor revision',async t=>{
 const dir=fixture(t),template=path.join(dir,'RecRoomStudio/ProjectTemplate/ProjectSettings');fs.mkdirSync(template,{recursive:true});fs.writeFileSync(path.join(template,'ProjectVersion.txt'),'m_EditorVersion: 6000.0.27f1\nm_EditorVersionWithRevision: 6000.0.27f1 (b8bf2c881068)\n');
 const fn=load(launcher+'fetch-recroom-studio-template-unity-editor-id.js',{'./$':framework({PatchKitLauncher2:{fetchAppBranchInternalState:async()=>({installedVersionId:171,rootDirState:{path:dir}})}})}).fetchRecRoomStudioTemplateUnityEditorId;
 const result=await fn({appId:'app',appBranchId:'branch'});assert.equal(result.version,'6000.0.27f1');assert.equal(result.changeset,'b8bf2c881068');
});

for(const line of ['6000.0.27f1 , installed at C:\\Unity\\Editor\\Unity.exe\r\n','6000.0.27f1 installed at C:\\Unity\\Editor\\Unity.exe\r\n'])test('Unity discovery handles Hub output: '+line.split('installed')[0],async()=>{
 const fn=load(launcher+'fetch-unity-editor-exe-path.js',{'../platform/unity.cjs':windowsUnity,'./$':framework({$childprocess:{spawn:()=>child(line)}}),'./fetch-unity-hub-exe-path':{fetchUnityHubExePath:async()=> 'Hub.exe'}}).fetchUnityEditorExePath;
 assert.equal(await fn({unityEditorId:{version:'6000.0.27f1'}}),'C:\\Unity\\Editor\\Unity.exe');
});

test('project opening passes the selected project as a process argument',async()=>{
 const calls=[];const fn=load(launcher+'open-unity-project.js',{'@recovered/routing-repair':{repairDevelopmentProfiles:()=>{},repairDevelopmentProject:()=>{}},'./$':framework({$childprocess:{spawn:(...args)=>{calls.push(args);const p=child();p.unref=()=>{};queueMicrotask(()=>p.emit('spawn'));return p;}}}),'./fetch-unity-editor-exe-path':{fetchUnityEditorExePath:async()=> 'C:\\Unity\\Unity.exe'}}).openUnityProject;
 await fn({unityEditorId:{version:'6000.0.27f1'},unityProjectDirPath:'D:\\Projects\\A project'});
 assert.equal(calls[0][0],'C:\\Unity\\Unity.exe');assert.deepEqual(Array.from(calls[0][1]),['-projectPath','D:\\Projects\\A project']);
});

test('canceling the Hub-close prompt preserves processes and aborts',async()=>{
 let killed=false;class Canceled extends Error{}
 const fn=load(launcher+'make-sure-that-unity-hub-process-doesnt-exist.js',{'./$':framework({PatchKitLauncher2:{DialogType:{WarningDialog:1},displayDialog:async()=>({dialogSelectedOptionId:'cancel'})}}),'./fetch-does-unity-hub-process-exist':{fetchDoesUnityHubProcessExist:async()=>true},'./kill-unity-hub-processes':{killUnityHubProcesses:async()=>{killed=true;}},'./unity-hub-process-exists':{UnityHubProcessExists:Canceled}}).makeSureThatUnityHubProcessDoesntExist;
 await assert.rejects(fn(),Canceled);assert.equal(killed,false);
});

test('directory picker cancellation produces no destination',async()=>{
 const fn=load(launcher+'display-select-dir-dialog.js',{'./$':framework({PatchKitLauncher2:{WINDOW_HANDLE:{value:{}}},$electron:{dialog:{showOpenDialog:async()=>({canceled:true,filePaths:[]})}}})}).displaySelectDirDialog;
 const r=await fn({selectDirDialogConfig:{title:'Choose'}});assert.equal(r.selectDirDialogResult.dirPath,undefined);
});

test('Unity install reports progress and finishes without running a real installer',async()=>{
 const state={value:undefined};const calls=[];
 const fn=load(launcher+'start-install-unity-editor-task.js',{'../platform/unity.cjs':windowsUnity,'./$':framework({$childprocess:{spawn:(exe,args)=>{calls.push(Array.from(args));return child('[Unity 6000.0.27f1] downloading 100%');}}}),'./fetch-unity-hub-exe-path':{fetchUnityHubExePath:async()=> 'Hub.exe'},'./make-sure-that-unity-hub-process-doesnt-exist':{makeSureThatUnityHubProcessDoesntExist:async()=>{}},'./ongoing-install-unity-editor-task-state':{ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE:state}}).startInstallUnityEditorTask;
 await fn({unityEditorId:{version:'6000.0.27f1',changeset:'b8bf2c881068'},installPath:'D:\\Editors'});
 for(let i=0;i<30&&!state.value?.isFinished;i++)await new Promise(r=>setTimeout(r,1));
 assert.equal(state.value.isFinished,true);assert.match(state.value.processOutput,/100%/);assert.equal(calls[1].includes('b8bf2c881068'),true);
});

test('a concurrent Unity installation is rejected',async()=>{
 const state={value:{isFinished:false}};const fn=load(launcher+'start-install-unity-editor-task.js',{'../platform/unity.cjs':windowsUnity,'./$':framework(),'./fetch-unity-hub-exe-path':{},'./make-sure-that-unity-hub-process-doesnt-exist':{},'./ongoing-install-unity-editor-task-state':{ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE:state}}).startInstallUnityEditorTask;
 await assert.rejects(fn({unityEditorId:{}}),/OngoingInstallUnityEditorTaskAlreadyExists/);
});
