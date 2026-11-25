'use strict';
const test=require('node:test');
const assert=require('node:assert/strict');
const {EventEmitter}=require('node:events');
const {load}=require('./harness.cjs');

test('source process enumerator finds the running process without fastlist',async()=>{
 const processes=await require('../src/platform/processes.cjs')();
 assert.ok(processes.some(p=>p.pid===process.pid&&p.name));
 assert.ok(processes.every(p=>Number.isInteger(p.pid)&&Number.isInteger(p.ppid)));
});

test('source protocol client preserves startup/second-instance delivery and rejects other schemes',async()=>{
 const app=new EventEmitter(), registrations=[];
 app.whenReady=async()=>{};app.isPackaged=true;app.setAsDefaultProtocolClient=(...args)=>{registrations.push(args);return true;};
 const {default:client}=load('src/platform/protocol.cjs',{electron:{app}},{process:{argv:['launcher','studio://startup'],execPath:'launcher.exe'}});
 const urls=[];client.on('request',url=>urls.push(url));
 await client.initialize({protocol:'studio',mode:'production'});
 app.emit('second-instance',{},['launcher','studio://project','studioevil://ignored']);
 assert.deepEqual(urls,['studio://startup','studio://project']);
 assert.equal(registrations[0][0],'studio');
 await assert.rejects(client.initialize({protocol:'studio',mode:'production'}),/already initialized/);
});

test('source protocol client propagates registration failure',async()=>{
 const app=new EventEmitter();app.whenReady=async()=>{};app.setAsDefaultProtocolClient=()=>false;
 const {default:client}=load('src/platform/protocol.cjs',{electron:{app}},{process:{argv:['launcher'],execPath:'launcher.exe'}});
 await assert.rejects(client.initialize({protocol:'studio',mode:'production'}),/register/);
});
