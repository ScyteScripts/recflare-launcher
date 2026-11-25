'use strict';
const {EventEmitter}=require('events');
const path=require('path');
const {app}=require('electron');

class ProtocolClient extends EventEmitter {
  constructor(){super();this.queue=[];app.on('open-url',(event,url)=>{event.preventDefault();this.receive(url);});}
  receive(url){if(!this.initialized){this.queue.push(url);return;}try{if(new URL(url).protocol===this.scheme+':')this.emit('request',url);}catch{}}
  async initialize({protocol,mode}) {
    if(this.initialized)throw new Error('Protocol client is already initialized');
    if(!/^[a-z][a-z0-9+.-]*$/i.test(protocol))throw new Error('Invalid protocol scheme');
    await app.whenReady();this.scheme=protocol.toLowerCase();
    const args=mode==='development'&&!app.isPackaged?[path.resolve(process.argv[1])]:[];
    if(!app.setAsDefaultProtocolClient(protocol,process.execPath,args))throw new Error('Could not register protocol client');
    this.initialized=true;
    app.on('second-instance',(_event,argv)=>{for(const arg of argv)this.receive(arg);});
    for(const url of [...this.queue,...process.argv])this.receive(url);
    this.queue=[];
  }
}
exports.ProtocolClient=ProtocolClient;
exports.default=new ProtocolClient();
