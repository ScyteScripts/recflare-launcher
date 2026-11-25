"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.killUnityHubProcesses = void 0;
const _1 = require("./$");
const {execFile} = require('child_process');
const path = require('path');
exports.killUnityHubProcesses = _1.__.declareManagedFunc()
    .name(`killUnityHubProcesses`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    // Closes Unity Hub by its exact image name; taskkill answers 128 when none is running.
    const taskkill = path.join(process.env.SystemRoot || 'C:\\Windows', 'System32', 'taskkill.exe');
    await new Promise((resolve, reject) => {
        execFile(taskkill, ['/IM', 'Unity Hub.exe', '/F', '/T'], {windowsHide:true,timeout:15000}, error => {
            if (!error || error.code === 128) return resolve();
            const failure = new _1.__.FatalError();
            failure.message = 'Could not close Unity Hub. Close it from the system tray or Task Manager, then try again.';
            reject(failure);
        });
    });
}).killUnityHubProcesses;
//# sourceMappingURL=kill-unity-hub-processes.js.map
