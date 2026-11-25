import fs from 'node:fs';
import path from 'node:path';
import {spawnSync} from 'node:child_process';
import {fileURLToPath} from 'node:url';
const root=path.resolve(path.dirname(fileURLToPath(import.meta.url)),'..');
export function buildUpdater(){
  const log=path.join(root,'build/updater-publish.log');fs.mkdirSync(path.dirname(log),{recursive:true});
  const result=spawnSync('dotnet',['publish',path.join(root,'updater/RecFlare.Updater.csproj'),'-c','Release','-r','win-x64','--self-contained','true','-o',path.join(root,'build/updater'),'-p:RestoreLockedMode=true','-p:PathMap='+root+'=/_/src','-v:q'],{cwd:root,windowsHide:true,encoding:'utf8',maxBuffer:10*1024*1024});
  fs.writeFileSync(log,(result.stdout||'')+(result.stderr||''));
  if(result.error||result.status!==0)throw new Error('Source updater build failed; see '+log);
  console.log('Built recovered .NET updater from source.');
}
if(process.argv[1]&&path.resolve(process.argv[1])===fileURLToPath(import.meta.url))buildUpdater();
