"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUnityHubPath = void 0;
// eslint-disable-next-line @typescript-eslint/tslint/config
const $winreg = require("winreg");
const _1 = require("./$");
exports.fetchUnityHubPath = _1.__.declareManagedFunc()
    .name(`fetchUnityHubPath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const unityHubWinRegHandle = new $winreg({
        hive: $winreg.HKLM,
        key: `\\Software\\Unity Technologies\\Hub`,
    });
    const isUnityHubInstalled = await new Promise((resolvePromise, rejectPromise) => {
        unityHubWinRegHandle.keyExists((winRegError, doesUnityHubWinRegKeyExists) => {
            if (winRegError !== undefined && winRegError !== null) {
                rejectPromise(winRegError);
                return;
            }
            resolvePromise(doesUnityHubWinRegKeyExists);
        });
    });
    if (!isUnityHubInstalled) {
        return undefined;
    }
    return await new Promise((resolvePromise, rejectPromise) => {
        unityHubWinRegHandle.values((winRegError, unityHubWinRegValues) => {
            if (winRegError !== undefined && winRegError !== null) {
                rejectPromise(winRegError);
                return;
            }
            for (const unityHubWinRegValue of unityHubWinRegValues) {
                if (unityHubWinRegValue.name === `InstallLocation`) {
                    resolvePromise(unityHubWinRegValue.value);
                    return;
                }
            }
            resolvePromise(undefined);
        });
    });
}).fetchUnityHubPath;
//# sourceMappingURL=fetch-unity-hub-path.js.map