'use strict';
const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('fs'),os=require('os'),path=require('path');
const {framework,load}=require('./harness.cjs');
const base='src/launcher/';
function gate({exists,choice='force_quit',kill=async()=>{}}){
 let dialogs=0,waits=0;
 const fn=load(base+'make-sure-that-unity-hub-process-doesnt-exist.js',{
  './$':framework({wait:async()=>{waits++;},PatchKitLauncher2:{DialogType:{WarningDialog:'warning'},displayDialog:async()=>{dialogs++;return {dialogSelectedOptionId:choice};}}}),
  './fetch-does-unity-hub-process-exist':{fetchDoesUnityHubProcessExist:exists},
  './kill-unity-hub-processes':{killUnityHubProcesses:kill},
  './unity-hub-process-exists':{UnityHubProcessExists:class extends Error{}}
 }).makeSureThatUnityHubProcessDoesntExist;
 return {fn,counts:()=>({dialogs,waits})};
}
test('Hub detection excludes editors, licensing clients and unrelated helper names',async()=>{
 let rows=[{name:'Unity.exe'},{name:'Unity.Licensing.Client.exe'},{name:'Unity Hub Helper.exe'}];
 const fn=load(base+'fetch-does-unity-hub-process-exist.js',{'./$':framework(),'ps-list':async()=>rows}).fetchDoesUnityHubProcessExist;
 assert.equal(await fn(),false);rows.push({name:'Unity Hub.exe'});assert.equal(await fn(),true);
});
test('Force Quit uses taskkill on the exact Hub image and surfaces failures',async()=>{
 let command;
 const loadKill=error=>load(base+'kill-unity-hub-processes.js',{'./$':framework(),'child_process':{execFile:(exe,args,options,callback)=>{command={exe,args,options};callback(error);}}},{process}).killUnityHubProcesses;
 await loadKill(null)();
 assert.ok(command.exe.endsWith('taskkill.exe'));
 assert.equal(JSON.stringify(command.args),JSON.stringify(['/IM','Unity Hub.exe','/F','/T']));
 assert.equal(command.options.windowsHide,true);
 await loadKill(Object.assign(new Error('not running'),{code:128}))();
 await assert.rejects(loadKill(Object.assign(new Error('denied'),{code:1}))(),/Could not close Unity Hub/);
});
test('Force Quit waits for shutdown after one dialog',async()=>{
 const states=[true,true,false];let kills=0;
 const g=gate({exists:async()=>states.shift(),kill:async()=>{kills++;}});
 await g.fn();assert.equal(kills,1);assert.deepEqual(g.counts(),{dialogs:1,waits:1});
});
test('Hub that stays running fails after a bounded wait without repeating the dialog',async()=>{
 const g=gate({exists:async()=>true,choice:'ok'});
 await assert.rejects(g.fn(),/still running/);assert.deepEqual(g.counts(),{dialogs:1,waits:10});
});
test('Hub shutdown failure is not swallowed or retried',async()=>{
 const g=gate({exists:async()=>true,kill:async()=>{throw Error('Could not close Unity Hub');}});
 await assert.rejects(g.fn(),/Could not close Unity Hub/);assert.deepEqual(g.counts(),{dialogs:1,waits:0});
});
test('Hub cancellation happens before project files are copied',async t=>{
 const dir=fs.mkdtempSync(path.join(os.tmpdir(),'studio-hub-cancel-'));t.after(()=>fs.rmSync(dir,{recursive:true,force:true}));
 const dest=path.join(dir,'new-project');let opened=false;
 const fn=load(base+'create-unity-project-from-recroom-studio-template.js',{
  './$':framework({PatchKitLauncher2:{fetchAppBranchInternalState:async()=>({installedVersionId:171,rootDirState:{path:dir}})}}),
  '@recovered/policy':{validateTemplate:()=>{}},
  './make-sure-that-unity-hub-process-doesnt-exist':{makeSureThatUnityHubProcessDoesntExist:async()=>{throw Error('Hub operation cancelled');}},
  './open-unity-project':{openUnityProject:async()=>{opened=true;}}
 }).createUnityProjectFromRecRoomStudioTemplate;
 await assert.rejects(fn({appId:'fixture',appBranchId:'main',unityProjectDirPath:dest}),/Hub operation cancelled/);
 assert.equal(fs.existsSync(dest),false);assert.equal(opened,false);
});
function installer({downloadError,event='exit',code=0}={}) {
 let url,spawn;
 const fn=load(base+'install-unity-hub.js',{
  './$':framework({PatchKitLauncher2:{WINDOW_HANDLE:{value:{}}},$childprocess:{execFile:(file,args,options,callback)=>{
   spawn={file,args,options};
   queueMicrotask(()=>callback(event==='error'?new Error('launch failed'):code===0?null:{code,message:'installer stopped'},'',event==='error'?'launch failed':''));
  }}}),
  './fetch-is-unity-hub-installed':{fetchIsUnityHubInstalled:async()=>false},
  'electron-dl':{download:async(_window,target)=>{url=target;if(downloadError)throw downloadError;return {getSavePath:()=> 'C:\\fixture with spaces\\UnityHubSetup-x64.exe'};}}
 },{process:{platform:'win32',env:{SystemRoot:'C:\\Windows'}}}).installUnityHub;
 return {fn,result:()=>({url,spawn})};
}
test('Hub setup requests Windows elevation, waits, and passes paths with spaces as data',async()=>{
 const setup=installer();await setup.fn();
 assert.equal(setup.result().url,'https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup-x64.exe');
 assert.equal(setup.result().spawn.options.windowsHide,true);
 assert.equal(setup.result().spawn.options.env.STUDIO_UNITY_HUB_INSTALLER,'C:\\fixture with spaces\\UnityHubSetup-x64.exe');
 assert.ok(setup.result().spawn.file.endsWith('powershell.exe'));
 assert.ok(setup.result().spawn.args.at(-1).includes('-Verb RunAs -Wait -PassThru'));
 assert.ok(setup.result().spawn.args.at(-1).includes('exit $installer.ExitCode'));
 assert.ok(setup.result().spawn.args.at(-1).includes('Get-AuthenticodeSignature -LiteralPath $env:STUDIO_UNITY_HUB_INSTALLER'));
 assert.ok(setup.result().spawn.args.at(-1).indexOf('Get-AuthenticodeSignature') < setup.result().spawn.args.at(-1).indexOf('-Verb RunAs'));
 assert.ok(!setup.result().spawn.args.at(-1).includes('fixture with spaces'));
});
test('Hub download and installer failures reject instead of leaving installation pending',async()=>{
 await assert.rejects(installer({downloadError:new Error('HTTP 404')}).fn(),/HTTP 404/);
 await assert.rejects(installer({event:'error'}).fn(),/launch failed/);
 await assert.rejects(installer({code:1}).fn(),/failed \(1\)/);
 await assert.rejects(installer({code:null}).fn(),/failed \(null\)/);
});
