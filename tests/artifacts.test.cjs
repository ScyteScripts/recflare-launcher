'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {root} = require('./harness.cjs');
const sha = bytes => crypto.createHash('sha256').update(bytes).digest('hex');

test('packaged ASAR has valid offsets and independently verified integrity for every file',()=>{
 const bytes=fs.readFileSync(path.join(root,'dist/RecFlare Launcher/resources/app.asar'));
 const header=JSON.parse(bytes.subarray(16,16+bytes.readUInt32LE(12)).toString());
 const start=8+bytes.readUInt32LE(4);let count=0;
 function walk(files){for(const item of Object.values(files)){if(item.files){walk(item.files);continue;}const data=bytes.subarray(start+Number(item.offset),start+Number(item.offset)+item.size);assert.equal(data.length,item.size);assert.equal(sha(data),item.integrity.hash);for(let i=0,j=0;i<data.length;i+=item.integrity.blockSize,j++)assert.equal(sha(data.subarray(i,i+item.integrity.blockSize)),item.integrity.blocks[j]);count++;}}
 walk(header.files);assert.ok(count>=10);for(const page of ['index.html','app.js','studio.js','styles.css'])assert.ok(header.files.theme.files[page],page);
 assert.ok(!bytes.includes(Buffer.from('node.napi.uv1.node')));
 function noDebug(files){for(const [name,item]of Object.entries(files)){assert.equal(/\.(map|pdb|mdb)$/i.test(name),false,name);if(item.files)noDebug(item.files);}}
 noDebug(header.files);
});

test('portable package uses verified selectable Electron and rebuilt updater without legacy binaries',()=>{
 const dir=path.join(root,'dist/RecFlare Launcher');
 const config=JSON.parse(fs.readFileSync(path.join(root,'config/runtime.json')));
 const runtime=path.join(root,'build/runtimes',config.version+'-win32-x64');
 const baseline=JSON.parse(fs.readFileSync(path.join(runtime,'runtime-manifest.json')));
 for(const f of baseline.files){
  if(f.path==='resources/default_app.asar')continue;
  if(f.path==='electron.exe'){const exe=fs.readFileSync(path.join(dir,'RecFlare Launcher.exe'));assert.equal(exe.subarray(0,2).toString(),'MZ','rebranded exe must be a valid Windows executable');assert.notEqual(sha(exe),f.sha256,'exe should carry the RecFlare icon and version, not the stock Electron ones');continue;}
  assert.equal(sha(fs.readFileSync(path.join(dir,f.path))),f.sha256,f.path);
 }
 assert.equal(fs.existsSync(path.join(dir,'resources/libpkapps')),false);
 assert.equal(fs.existsSync(path.join(dir,'resources/updater/RecFlare.Updater.exe')),true);
 const manifest=JSON.parse(fs.readFileSync(path.join(dir,'build-manifest.json')));
 assert.equal(manifest.electron,config.version);
 assert.equal(manifest.files.some(f=>/\.node$|fastlist|(?:^|\/)PatchKit\.[^/]+\.dll$/.test(f.path)),false);
 assert.equal(manifest.releaseDebugFiles,false);
 assert.equal(manifest.files.some(f=>/\.(map|pdb|mdb)$/i.test(f.path)),false);
 for(const required of ['RecFlare.Updater.deps.json','RecFlare.Updater.runtimeconfig.json'])assert.ok(fs.existsSync(path.join(dir,'resources/updater',required)));
 assert.ok(fs.existsSync(path.join(root,'build/updater/RecFlare.Updater.pdb')),'Private symbols should remain available');
 const icon=fs.readFileSync(path.join(dir,'resources/launcher.ico'));
 assert.equal(icon.readUInt16LE(0),0);assert.equal(icon.readUInt16LE(2),1);assert.ok(icon.readUInt16LE(4)>=4);
});

test('build consumes recovered launcher/API source and pinned dependencies',()=>{
 const report=JSON.parse(fs.readFileSync(path.join(root,'build/build-report.json')));
 const inputs=Object.keys(report.metafile.inputs);
 assert.equal(inputs.filter(x=>x.startsWith('src/launcher/')).length,38);
 assert.equal(inputs.filter(x=>x.startsWith('src/api/')).length,33);
 assert.ok(report.sourceModules.length>=900);
 assert.equal(report.localFrameworkPackages.length,13);
 assert.ok(report.localFrameworkPackages.every(p=>p.source.startsWith('file:src/vendor/')));
 assert.equal(report.nativeAddonCount,0);
 assert.ok(inputs.includes('src/updater/bridge.cjs'));
 assert.ok(inputs.includes('src/platform/processes.cjs'));
 assert.ok(inputs.includes('src/platform/protocol.cjs'));
 assert.equal(inputs.some(p=>/node_modules\/(?:@xingrz\/ffi-napi|ref-napi|ref-struct-di|ps-list|electron-app-universal-protocol-client)\//.test(p)),false);
 assert.equal(report.lockfileSha256,sha(fs.readFileSync(path.join(root,'package-lock.json'))));
 const map=JSON.parse(fs.readFileSync(path.join(root,'build/source-app/main.js.map')));
 assert.ok(map.sources.some(s=>s.includes('src/vendor/patchkit-launcher2-base@0.0.57')));
});

test('the page is the RecFlare launcher UI, with no telemetry hosts and no React bundle',()=>{
 const theme=path.join(root,'build/source-app/theme');
 for(const page of ['index.html','app.js','studio.js','styles.css','icons.js'])assert.ok(fs.existsSync(path.join(theme,page)),page);
 assert.equal(fs.existsSync(path.join(theme,'assets/main.js')),false);
 for(const page of ['index.html','app.js','studio.js']){
  const text=fs.readFileSync(path.join(theme,page),'utf8');
  assert.equal(/rudderstack|statsig|sentry|segment\.io|mixpanel|google-analytics/i.test(text),false,page);
 }
 const html=fs.readFileSync(path.join(theme,'index.html'),'utf8');
 assert.ok(html.includes("script-src 'self'"));
});

test('release application binaries do not embed personal build paths',()=>{
 const dir=path.join(root,'dist/RecFlare Launcher');
 for(const file of ['resources/app.asar','resources/updater/RecFlare.Updater.dll']) {
  const bytes=fs.readFileSync(path.join(dir,file));
  for(const encoding of ['utf8','utf16le'])assert.equal(/[A-Z]:[\\/](?:Users|Development)[\\/]/i.test(bytes.toString(encoding)),false,file);
 }
});
