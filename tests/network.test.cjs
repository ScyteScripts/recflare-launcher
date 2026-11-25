'use strict';
const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('fs'),os=require('os'),path=require('path');
const network=require('../src/network.cjs');
const {isIntegrated}=require('../src/platform/installation.cjs');
test('Rec Room hostnames route to Recflare with paths, queries and fragments intact',()=>{
 for(const [from,to] of [
  ['https://recroom.com/terms-of-service','https://recflare.net/terms-of-service'],
  ['https://rec.net/room/one?x=1#part','https://recflare.net/room/one?x=1#part'],
  ['https://ns.rec.net','https://ns.recflare.net/'],
  ['http://api.recroom.com:8080/test','https://api.recflare.net/test'],
  ['https://img.recroom.net/image','https://img.recflare.net/image'],
  ['https://cdn.recroom.cloud/file','https://cdn.recflare.net/file']
 ])assert.equal(network.rewriteUrl(from),to);
});
test('routing preserves unrelated hosts, vendor services and non-web protocols',()=>{
 for(const value of ['https://notrecroom.com/a','https://rec.net.example.com/a','https://example.com/recroom.com','https://api2.patchkit.net/1/apps/a','recroom-studio-launcher://open','not a URL','https://ns.recflare.net'])assert.equal(network.rewriteUrl(value),value);
 assert.equal(network.isOriginalTelemetry('https://recroom-dataplane.rudderstack.com/v1/batch'),true);
 assert.equal(network.isOriginalTelemetry('https://events.statsigapi.net/v1/log_event'),true);
 assert.equal(network.isOriginalTelemetry('https://example.com/statsigapi.net'),false);
});
test('network hook redirects upstream requests and blocks original telemetry',async()=>{
 let callback;
 network.attach({app:{whenReady:()=>Promise.resolve()},session:{defaultSession:{webRequest:{onBeforeRequest:fn=>callback=fn}}}});
 await Promise.resolve();
 for(const [url,expected]of [['https://rec.net/path',{redirectURL:'https://recflare.net/path'}],['https://recroom-dataplane.rudderstack.com/v1/batch',{cancel:true}],['https://unity.com/download',{}]])callback({url},result=>assert.deepEqual(result,expected));
});
test('installed mode requires an owned integrated receipt and handles Unicode INI files',t=>{
 const dir=fs.mkdtempSync(path.join(os.tmpdir(),'studio-mode-'));
 t.after(()=>fs.rmSync(dir,{recursive:true,force:true}));
 const file=path.join(dir,'source-launcher-install.ini');
 assert.equal(isIntegrated(dir),false);
 for(const encoding of ['utf8','utf16le']){
  fs.writeFileSync(file,'\ufeff[Setup]\r\nProduct=RecRoomStudioSourceLauncher\r\nNoIntegration=0\r\n',encoding);
  assert.equal(isIntegrated(dir),true);
 }
 fs.writeFileSync(file,'[Setup]\nProduct=RecRoomStudioSourceLauncher\nNoIntegration=1\n');assert.equal(isIntegrated(dir),false);
 fs.writeFileSync(file,'[Setup]\nProduct=Other\nNoIntegration=0\n');assert.equal(isIntegrated(dir),false);
});
