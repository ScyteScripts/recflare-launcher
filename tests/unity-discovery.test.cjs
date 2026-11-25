'use strict';
const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const {EventEmitter}=require('node:events');
const {root,framework,load}=require('./harness.cjs');
const unity=require('../src/platform/unity.cjs');
function fixture(t){const base=path.join(root,'validation/unit');fs.mkdirSync(base,{recursive:true});const dir=fs.mkdtempSync(path.join(base,'editor-'));t.after(()=>{if(!dir.startsWith(base+path.sep))throw Error('Invalid fixture');fs.rmSync(dir,{recursive:true,force:true});});return dir;}
test('installed editor is found in the configured Hub location without invoking Hub',async t=>{
  const dir=fixture(t),data=path.join(dir,'settings'),editors=path.join(dir,'custom-editors');
  fs.mkdirSync(path.join(data,'UnityHub'),{recursive:true});fs.writeFileSync(path.join(data,'UnityHub/secondaryInstallPath.json'),JSON.stringify(editors));
  const exe=path.join(editors,'6000.0.27f1/Editor/Unity.exe');fs.mkdirSync(path.dirname(exe),{recursive:true});fs.writeFileSync(exe,'fixture');
  const env={APPDATA:data,ProgramFiles:path.join(dir,'programs')};
  assert.equal(unity.findInstalledEditor('6000.0.27f1','win32',env,dir),exe);
  assert.equal(unity.findInstalledEditor('6000.0.28f1','win32',env,dir),undefined);
  const implementation={...unity,findInstalledEditor:v=>unity.findInstalledEditor(v,'win32',env,dir)};
  const fn=load('src/launcher/fetch-unity-editor-exe-path.js',{'../platform/unity.cjs':implementation,'./$':framework(),'./fetch-unity-hub-exe-path':{fetchUnityHubExePath:async()=>{throw Error('Hub must not run');}}}).fetchUnityEditorExePath;
  assert.equal(await fn({unityEditorId:{version:'6000.0.27f1'}}),exe);
});
test('hung Hub queries time out, kill only the child, and concurrent queries share one process',async()=>{
  let spawned=0,killed=0;
  const spawn=()=>{spawned++;const child=new EventEmitter();child.stdout=new EventEmitter();child.stderr=new EventEmitter();child.kill=()=>{killed++;child.emit('close',1);};return child;};
  const first=unity.query(spawn,'fixture-hub',['editors'],15);
  assert.equal(unity.query(spawn,'fixture-hub',['editors'],15),first);
  await assert.rejects(first,/did not respond/);
  assert.equal(spawned,1);assert.equal(killed,1);
  await assert.rejects(unity.query(spawn,'fixture-hub',['editors'],15),/did not respond/);
  assert.equal(spawned,2);
});
