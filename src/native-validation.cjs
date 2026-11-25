'use strict';
const fs = require('fs');
const path = require('path');
const P = require('troy-patchkit-app-electron-client');
const {fetch} = require('cross-fetch');

exports.run = async () => {
  const config = JSON.parse(fs.readFileSync(path.join(process.resourcesPath, 'config.json'), 'utf8'));
  const appRootDirPath = path.join(process.env.STUDIO_LAUNCHER_VALIDATION_ROOT, 'native-install');
  const cancellationRoot = path.join(process.env.STUDIO_LAUNCHER_VALIDATION_ROOT, 'native-cancel');
  const appSecret = process.env.STUDIO_LAUNCHER_FIXTURE_SECRET;
  const results = [];
  const wait = ms => new Promise(resolve => setTimeout(resolve, ms));
  async function poll(id, repair=false, allowCancellation=false, fetchState) {
    for (let i = 0; i < 350; i++) {
      const state = fetchState ? await fetchState(id) : repair ? await P.fetchAppRepairTaskState(id) : await P.fetchAppUpdateTaskState(id);
      if (state.isFinished) {
        if (state.errorTypeName && !(allowCancellation && state.errorTypeName === 'AppUpdateTaskHasBeenCancelled')) throw new Error(JSON.stringify(state));
        return state;
      }
      await wait(100);
    }
    throw new Error('Native updater test timed out');
  }
  await P.createAppRootDir({appRootDirPath, isAppNotEmptyRootDirAllowed:false});
  await poll(await P.startAppUpdateTask({appRootDirPath, appSecret}));
  if (fs.readFileSync(path.join(appRootDirPath,'hello.txt'),'utf8') !== 'fixture-v171\n') throw new Error('Installation bytes differ');
  results.push('native-install-v171');
  fs.writeFileSync(path.join(appRootDirPath,'hello.txt'),'corrupted');
  await poll(await P.startAppRepairTask({appRootDirPath,appSecret}),true);
  await poll(await P.startAppUpdateTask({appRootDirPath,appSecret}));
  if (fs.readFileSync(path.join(appRootDirPath,'hello.txt'),'utf8') !== 'fixture-v171\n') throw new Error('Repair did not restore bytes');
  results.push('native-corruption-repair');
  const template = path.join(appRootDirPath,'RecRoomStudio/ProjectTemplate');
  const templateConfig = path.join(template,'ProjectSettings/RecRoomStudioPatch.ini');
  fs.writeFileSync(templateConfig,'[Network]\nServerUrl=https://ns.dev.proot.app\n');
  const removed = require('./platform/routing-repair.cjs').removeDevelopmentTemplate(appRootDirPath);
  if (!removed || fs.existsSync(template) || !fs.existsSync(path.join(appRootDirPath,'hello.txt')))
    throw new Error('Contaminated template deletion boundary failed');
  await poll(await P.startAppRepairTask({appRootDirPath,appSecret}),true);
  if (fs.readFileSync(templateConfig,'utf8') !== '[Network]\nServerUrl=https://ns.recflare.net\n' ||
      fs.readFileSync(path.join(template,'RecRoomStudioPackages/sdk.tgz'),'utf8') !== 'fixture-sdk-171')
    throw new Error('Repair did not re-download the deleted template package');
  results.push('dev-template-removed-and-reinstalled-from-verified-package');
  require('./platform/routing-repair.cjs').completeTemplateReinstall(appRootDirPath);
  if (require('./platform/routing-repair.cjs').needsDevelopmentTemplateReinstall(appRootDirPath))
    throw new Error('Template reinstall completion marker was not cleared');
  await fetch(process.env.STUDIO_LAUNCHER_FIXTURE_ORIGIN+'/_fixture/latest?version=172');
  await poll(await P.startAppUpdateTask({appRootDirPath,appSecret}));
  if (fs.readFileSync(path.join(appRootDirPath,'hello.txt'),'utf8') !== 'fixture-v172\n') throw new Error('Update bytes differ');
  results.push('native-update-v172');
  if(await P.fetchAppInstalledVersionId({appRootDirPath})!==172)throw new Error('Installed version metadata differs');
  const movedRoot=path.join(process.env.STUDIO_LAUNCHER_VALIDATION_ROOT,'moved-install');
  await poll(await P.startAppMoveTask({appRootDirPath,appNewRootDirPath:movedRoot}),false,false,P.fetchAppMoveTaskState);
  if(fs.readFileSync(path.join(movedRoot,'hello.txt'),'utf8')!=='fixture-v172\n')throw new Error('Move lost content');
  results.push('source-updater-move');
  await poll(await P.startAppUninstallTask({appRootDirPath:movedRoot}),false,false,P.fetchAppUninstallTaskState);
  if(fs.existsSync(path.join(movedRoot,'hello.txt')))throw new Error('Uninstall retained managed content');
  results.push('source-updater-uninstall');
  await fetch(process.env.STUDIO_LAUNCHER_FIXTURE_ORIGIN+'/_fixture/slow?enabled=1');
  await P.createAppRootDir({appRootDirPath:cancellationRoot,isAppNotEmptyRootDirAllowed:false});
  const task = await P.startAppUpdateTask({appRootDirPath:cancellationRoot,appSecret});
  let transferStarted=false;
  for(let i=0;i<150;i++) {
    const response=await fetch(process.env.STUDIO_LAUNCHER_FIXTURE_ORIGIN+'/_fixture/status');
    if((await response.json()).slowTransferBytes>0){transferStarted=true;break;}
    await wait(100);
  }
  if(!transferStarted) throw new Error('Cancellation fixture never began transferring content');
  await P.cancelAppUpdateTask(task);
  const state = await poll(task,false,true);
  if (state.errorTypeName !== 'AppUpdateTaskHasBeenCancelled') throw new Error('Cancellation was not observed');
  results.push('native-download-cancellation');
  return results;
};
