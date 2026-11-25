"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getWindowPreloadScript = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.getWindowPreloadScript = _1.__.declareManagedFunc()
    .name(`getWindowPreloadScript`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    return [
        `const electron = require('electron');`,
        `electron.contextBridge.exposeInMainWorld(`,
        `  "patchKitLauncher2Id",`,
        `  ${_1.__.serializeValueToJson({ value: config_1.CONFIG.value.id })},`,
        `);`,
        `electron.contextBridge.exposeInMainWorld(`,
        `  "patchKitLauncher2VersionLabel",`,
        `  ${_1.__.serializeValueToJson({ value: config_1.CONFIG.value.version })},`,
        `);`,
        `electron.contextBridge.exposeInMainWorld(`,
        `  "patchKitLauncher2TargetOperatingSystemPlatform",`,
        `  ${_1.__.serializeValueToJson({ value: config_1.CONFIG.value.targetOperatingSystemPlatform })},`,
        `);`,
        `electron.contextBridge.exposeInMainWorld(`,
        `  "sendPatchKitLauncher2ApiFuncRequest",`,
        `  (patchKitLauncher2ApiFuncFullname, patchKitLauncher2ApiFuncExecArgs) => {`,
        `    return electron.ipcRenderer.invoke(patchKitLauncher2ApiFuncFullname, patchKitLauncher2ApiFuncExecArgs);`,
        `  },`,
        `);`,
        `electron.contextBridge.exposeInMainWorld(`,
        `  "onRecFlareLauncherEvent",`,
        `  (listener) => {`,
        `    electron.ipcRenderer.on("RecFlareLauncher.event", (_event, message) => listener(message));`,
        `  },`,
        `);`,
    ].join(`\n`);
}).getWindowPreloadScript;
//# sourceMappingURL=get-window-preload-script.js.map