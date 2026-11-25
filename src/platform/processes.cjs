'use strict';
const path = require('path');
const {execFile} = require('child_process');
const {promisify} = require('util');
const exec = promisify(execFile);

module.exports = async function listProcesses() {
  if(process.platform==='win32') {
    const shell=path.join(process.env.SystemRoot || 'C:\\Windows','System32','WindowsPowerShell','v1.0','powershell.exe');
    const {stdout}=await exec(shell,['-NoProfile','-NonInteractive','-Command','Get-CimInstance Win32_Process -ErrorAction Stop | Select-Object ProcessId,ParentProcessId,Name | ConvertTo-Json -Compress'],{windowsHide:true,timeout:15000,maxBuffer:10*1024*1024});
    const items=JSON.parse(stdout.replace(/^\uFEFF/,'')||'[]');
    return (Array.isArray(items)?items:[items]).map(p=>({pid:p.ProcessId,ppid:p.ParentProcessId,name:p.Name}));
  }
  const {stdout}=await exec('ps',['-eo','pid=,ppid=,comm='],{timeout:15000});
  return stdout.trim().split('\n').filter(Boolean).map(line=>{
    const match=/^\s*(\d+)\s+(\d+)\s+(.+)$/.exec(line);if(!match)throw new Error('Unexpected process enumeration output');
    return {pid:Number(match[1]),ppid:Number(match[2]),name:path.basename(match[3])};
  });
};
