'use strict';
// Value descriptors for the recovered JS API. No native memory or ABI is involved.
const types = Object.fromEntries(['void','CString','int32','int64','bool'].map(name=>[name,name]));
exports.ref = {types,refType:type=>type,alloc:()=>({value:undefined}),deref:slot=>slot.value};
exports.struct = () => fields => fields;
