'use strict';
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const { EventEmitter } = require('node:events');
const root = path.resolve(__dirname, '..');

function framework(overrides={}) {
  const errors = new Proxy({}, {get(target,name) {
    if (!(name in target)) target[name] = class extends Error {constructor(){super(String(name));this.name=String(name);}};
    return target[name];
  }});
  return {_:errors, __:{
    declareManagedFunc() {const builder=new Proxy({}, {get(_t,name){if(name==='execute') return fn=>{fn.config={};return new Proxy(fn,{get(target,key){return key in target?target[key]:target;}});};return ()=>builder;}});return builder;},
    checkAssertion(value){if(!value)throw new Error('Assertion failed');}, FatalError: errors.FatalError,
    invokeSyncExp:fn=>fn(), invokeAsyncExp:fn=>fn(), $fs:fs, $path:path,
    ...overrides
  }};
}

function load(file, dependencies={}, globals={}) {
  const module={exports:{}};
  const requireMock = name => {
    if(Object.hasOwn(dependencies,name))return dependencies[name];
    if (/^\.\.?\/platform\/[a-z-]+\.cjs$/.test(name)) return require(path.resolve(path.dirname(path.join(root,file)),name));
    if(name.startsWith('.') || name.startsWith('@recovered/'))throw new Error('Unmocked dependency: '+name+' in '+file);
    return require(name);
  };
  const absolute=path.join(root,file);
  const factory=vm.runInNewContext('(function(require,module,exports){'+fs.readFileSync(absolute,'utf8')+'\n})', {console,Buffer,setTimeout,clearTimeout,URL,process,...globals},{filename:absolute});
  factory(requireMock,module,module.exports);return module.exports;
}

function child(output='', code=0) {
  const process=new EventEmitter();process.stdout=new EventEmitter();process.stderr=new EventEmitter();
  queueMicrotask(()=>{process.stdout.emit('data',Buffer.from(output));process.emit('exit',code);process.emit('close',code);});
  return process;
}
module.exports={root,framework,load,child};
