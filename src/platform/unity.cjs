'use strict';
const fs = require('fs');
const path = require('path');
const os = require('os');

function hubArgs(args, platform = process.platform) {
  return [...(platform === 'linux' ? [] : ['--']), '--headless', ...args];
}
function hubExecutable(location, platform = process.platform) {
  if (platform === 'win32') return /\.exe$/i.test(location) ? location : path.join(location, 'Unity Hub.exe');
  if (platform === 'darwin' && location.endsWith('.app')) return path.join(location, 'Contents/MacOS/Unity Hub');
  return location;
}
function findHub(platform = process.platform, env = process.env, home = os.homedir()) {
  const candidates = env.STUDIO_UNITY_HUB ? [env.STUDIO_UNITY_HUB] : platform === 'darwin'
    ? ['/Applications/Unity Hub.app', path.join(home, 'Applications/Unity Hub.app')]
    : ['/opt/unityhub/unityhub', '/usr/bin/unityhub', '/usr/local/bin/unityhub',
      ...((env.PATH || '').split(path.delimiter).filter(Boolean).map(dir => path.join(dir, 'unityhub')))];
  return candidates.find(file => fs.existsSync(hubExecutable(file, platform)));
}
function editorExecutable(location, platform = process.platform) {
  if (platform === 'win32') return /\.exe$/i.test(location) ? location : path.basename(location).toLowerCase() === 'editor' ? path.join(location,'Unity.exe') : path.join(location,'Editor/Unity.exe');
  if (platform === 'darwin') {
    if (location.endsWith('.app')) return path.join(location, 'Contents/MacOS/Unity');
    if (path.basename(location) !== 'Unity') return path.join(location, 'Unity.app/Contents/MacOS/Unity');
    return location;
  }
  return path.basename(location) === 'Unity' ? location : path.join(location, 'Editor/Unity');
}
function packageManagerPath(executable, platform = process.platform) {
  return platform === 'darwin'
    ? path.resolve(path.dirname(executable), '../Resources/PackageManager/Server/UnityPackageManager')
    : path.join(path.dirname(executable), 'Data/Resources/PackageManager/Server', platform === 'win32' ? 'UnityPackageManager.exe' : 'UnityPackageManager');
}
function isHubProcess(name) { return /^(?:Unity Hub(?:\.exe)?|unityhub|unityhub-bin)$/i.test(name); }

function installRoot(platform = process.platform, env = process.env, home = os.homedir()) {
  const appData = platform === 'win32' ? env.APPDATA || path.join(home,'AppData/Roaming') : platform === 'darwin' ? path.join(home,'Library/Application Support') : env.XDG_CONFIG_HOME || path.join(home,'.config');
  try {
    const configured = JSON.parse(fs.readFileSync(path.join(appData,'UnityHub/secondaryInstallPath.json'),'utf8'));
    if (typeof configured === 'string' && configured && path.isAbsolute(configured)) return configured;
  } catch (error) {if (error.code !== 'ENOENT' && !(error instanceof SyntaxError)) throw error;}
  return platform === 'win32' ? path.join(env.ProgramFiles || 'C:\\Program Files','Unity/Hub/Editor') : platform === 'darwin' ? '/Applications/Unity/Hub/Editor' : path.join(home,'Unity/Hub/Editor');
}
function findInstalledEditor(version, platform = process.platform, env = process.env, home = os.homedir()) {
  if (!/^\d+\.\d+\.\d+[abfp]\d+$/.test(version)) throw Error('Invalid Unity editor version');
  const base=path.join(installRoot(platform,env,home),version);
  const file=platform === 'win32' ? path.join(base,'Editor/Unity.exe') : editorExecutable(base,platform);
  return fs.existsSync(file) && fs.statSync(file).isFile() ? file : undefined;
}

// Spawn without a shell; drain both pipes and reject failed exits and launch errors.
function run(spawn, executable, args, onOutput = () => {}, {timeoutMs = 0} = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn(executable, args, {stdio: 'pipe', windowsHide: true});
    let timer, finished=false;
    const finish=(error,value)=>{if(finished)return;finished=true;clearTimeout(timer);error?reject(error):resolve(value);};
    if (timeoutMs > 0) timer=setTimeout(()=>{
      finish(new Error('Unity Hub did not respond in time. Open Unity Hub and retry.'));
      // Kill only this command process, never the user's existing Hub session.
      try {child.kill();} catch {}
    },timeoutMs);
    let output = '';
    child.stdout.on('data', chunk => {output += chunk.toString(); onOutput(chunk.toString());});
    child.stderr.on('data', chunk => onOutput(chunk.toString()));
    child.once('error', error=>finish(error));
    child.once('close', code => code === 0 ? finish(null,output) : finish(new Error(`Unity Hub exited with code ${code}. Open Unity Hub to check installation and licensing.`)));
  });
}
const queries = new Map();
function query(spawn, executable, args, timeoutMs = 10000) {
  const key=JSON.stringify([executable,args]);
  if (queries.has(key)) return queries.get(key);
  const pending=run(spawn,executable,args,()=>{},{timeoutMs});
  queries.set(key,pending);
  pending.then(()=>queries.delete(key),()=>queries.delete(key));
  return pending;
}
module.exports = {hubArgs, hubExecutable, findHub, editorExecutable, packageManagerPath, isHubProcess, run, query, installRoot, findInstalledEditor};
