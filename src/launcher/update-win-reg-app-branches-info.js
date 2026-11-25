"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.updateWinRegAppBranchesInfo = void 0;
const _1 = require("./$");
const create_winreg_handle_1 = require("./create-winreg-handle");
// eslint-disable-next-line @typescript-eslint/tslint/config
const $winreg = require("winreg");
exports.updateWinRegAppBranchesInfo = _1.__.declareManagedFunc()
    .name(`updateWinRegAppBranchesInfo`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const winRegHandle = await (0, create_winreg_handle_1.createWinRegHandle)();
    const registeredAppsState = await _1.__.PatchKitLauncher2.retrieveRegisteredAppsState();
    for (const [appId, appState] of Object.entries(registeredAppsState)) {
        for (const [appBranchId, appBranchState] of Object.entries(appState.registeredBranches)) {
            const appBranchInfo = await _1.__.PatchKitLauncher2.retrieveAppBranchInfo({ appId, appBranchId });
            const appBranchInstalledVersionId = appBranchState.installedVersionId;
            if (appBranchState.ongoingTaskId === undefined) {
                const winRegAppBranchInfoKey = `recroom_branch_${appBranchId}_info`;
                if (appBranchInstalledVersionId !== undefined) {
                    await new Promise((resolvePromise, rejectPromise) => {
                        try {
                            winRegHandle.set(winRegAppBranchInfoKey, $winreg.REG_MULTI_SZ, JSON.stringify({
                                secret: appBranchInfo.secret,
                                dirPath: appBranchState.rootDir.path,
                                installedVersionId: appBranchInstalledVersionId,
                            }), (e) => {
                                if (e === undefined || e === null) {
                                    resolvePromise();
                                }
                                else {
                                    rejectPromise(e);
                                }
                            });
                        }
                        catch (e) {
                            rejectPromise(e);
                        }
                    });
                }
                else {
                    const doesWinRegAppBranchInfoKeyExist = await new Promise((resolvePromise, rejectPromise) => {
                        try {
                            winRegHandle.valueExists(winRegAppBranchInfoKey, (e, r) => {
                                if (e === undefined || e === null) {
                                    resolvePromise(r);
                                }
                                else {
                                    rejectPromise(e);
                                }
                            });
                        }
                        catch (e) {
                            rejectPromise(e);
                        }
                    });
                    if (doesWinRegAppBranchInfoKeyExist) {
                        await new Promise((resolvePromise, rejectPromise) => {
                            try {
                                winRegHandle.remove(winRegAppBranchInfoKey, (e) => {
                                    if (e === undefined || e === null) {
                                        resolvePromise();
                                    }
                                    else {
                                        rejectPromise(e);
                                    }
                                });
                            }
                            catch (e) {
                                rejectPromise(e);
                            }
                        });
                    }
                }
            }
        }
    }
}).updateWinRegAppBranchesInfo;
//# sourceMappingURL=update-win-reg-app-branches-info.js.map