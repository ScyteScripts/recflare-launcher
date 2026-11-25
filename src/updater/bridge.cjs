'use strict';
const fs = require('fs');
const path = require('path');
const {spawn} = require('child_process');
const readline = require('readline');

class UpdaterProcess {
  constructor(executable) {this.executable=executable;this.pending=new Map();this.nextId=0;this.closed=false;}
  start() {
    if(this.child) return;
    this.child=spawn(this.executable,[],{windowsHide:true,stdio:['pipe','pipe','pipe']});
    const fail=error=>{this.closed=true;for(const p of this.pending.values()){clearTimeout(p.timer);p.reject(error);}this.pending.clear();};
    this.child.on('error',fail);
    this.child.on('exit',(code)=>fail(new Error('Updater exited ('+code+')')));
    // Library diagnostics never share the JSON-RPC channel or echo credentials.
    this.child.stderr.on('data',()=>{});
    this.lines=readline.createInterface({input:this.child.stdout});
    this.lines.on('line',line=>{
      let response;try{response=JSON.parse(line);}catch{return fail(new Error('Invalid updater response'));}
      const pending=this.pending.get(response.id);if(!pending)return;
      this.pending.delete(response.id);clearTimeout(pending.timer);pending.resolve(response);
    });
    process.once('exit',()=>{this.child?.kill();});
  }
  call(method,args) {
    this.start();if(this.closed)return Promise.reject(new Error('Updater is closed'));
    const id=++this.nextId;
    return new Promise((resolve,reject)=>{
      const timer=setTimeout(()=>{this.pending.delete(id);reject(new Error('Updater operation timed out: '+method));this.child.kill();},60000);
      this.pending.set(id,{resolve,reject,timer});
      this.child.stdin.write(JSON.stringify({id,method,args})+'\n',error=>{if(error){clearTimeout(timer);this.pending.delete(id);reject(error);}});
    });
  }
  close(){if(this.child&&!this.closed)this.child.stdin.end();}
}
const decode=value=>Buffer.isBuffer(value)?value.toString('utf8').replace(/\0$/,''):value;

function createBinding() {
  const worker=new UpdaterProcess(process.env.STUDIO_UPDATER_EXECUTABLE || path.join(process.resourcesPath,'updater','RecFlare.Updater.exe'));
  let ready;
  try{require('electron').app.once('will-quit',()=>worker.close());}catch{}
  return new Proxy({}, {get(_target,name){
    if(name==='initLibpkapps')return (...args)=>{
      const [,logs,...rest]=args.map(decode);
      fs.mkdirSync(logs,{recursive:true});
      ready=worker.call('initLibpkappsNet',[logs,...rest]).then(r=>{if(r.error)throw new Error('Updater initialization failed');});
      // Recovered initializer has a synchronous signature; later calls await it.
      ready.catch(()=>{});
    };
    const operation=()=>{throw new Error('Updater operations must use the asynchronous interface');};
    operation.async=(...args)=>{
      const callback=args.pop();
      const slot=args.length && args[args.length-1] && typeof args[args.length-1]==='object' && !Buffer.isBuffer(args[args.length-1]) ? args.pop() : null;
      Promise.resolve(ready).then(()=>worker.call(String(name),args.map(decode))).then(response=>{
        if(slot)slot.value=response.result;
        callback(null,response.error);
      },error=>callback(error));
    };
    return operation;
  }});
}
module.exports={UpdaterProcess,createBinding};
