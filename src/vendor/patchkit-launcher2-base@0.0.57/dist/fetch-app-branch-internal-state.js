"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchInternalState = void 0;
const _1 = require("./$");
const apps_branches_cached_internal_state_box_1 = require("./apps-branches-cached-internal-state-box");
const fetch_settings_1 = require("./fetch-settings");
exports.fetchAppBranchInternalState = _1.__.declareManagedFunc()
    .name(`fetchAppBranchInternalState`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppBranchIsNotAllowed,
    _1._.AppBranchIsNotRegistered,
])
    .async()
    .execute(async ({ appId, appBranchId, }) => {
    var _a, _b, _c, _d;
    var _e, _f;
    const appBranchRootDirState = await _1.__.invokeAsyncExp(async () => {
        var _a, _b, _c;
        const settings = await (0, fetch_settings_1.fetchSettings)();
        const appBranchRootDirPath = (_c = (_b = (_a = settings.apps[appId]) === null || _a === void 0 ? void 0 : _a.branches) === null || _b === void 0 ? void 0 : _b[appBranchId]) === null || _c === void 0 ? void 0 : _c.rootDirPath;
        if (appBranchRootDirPath !== undefined) {
            try {
                const doesAppBranchRootDirExist = await _1.__.PatchKit.fetchDoesAppRootDirExist({
                    appRootDirPath: appBranchRootDirPath,
                });
                if (doesAppBranchRootDirExist) {
                    return {
                        arePermissionsSufficient: true,
                        path: appBranchRootDirPath,
                    };
                }
            }
            catch (e) {
                if (e instanceof _1.__.PatchKit.AppRootDirPermissionsAreInsufficient) {
                    return {
                        path: appBranchRootDirPath,
                        arePermissionsSufficient: false,
                    };
                }
                throw e;
            }
        }
        return undefined;
    });
    if (appBranchRootDirState === undefined) {
        if (((_a = apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) !== undefined) {
            apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId][appBranchId].value = undefined;
        }
        else {
            (_b = (_e = apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_e[appId] = {});
            apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId][appBranchId] = {
                value: undefined,
            };
        }
        throw new _1._.AppBranchIsNotRegistered();
    }
    const doesAppBranchNeedRepairing = await _1.__.PatchKit.fetchDoesAppNeedRepairing({
        appRootDirPath: appBranchRootDirState.path,
    });
    const appBranchInstalledVersionId = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.PatchKit.fetchAppInstalledVersionId({
                appRootDirPath: appBranchRootDirState.path,
            });
        }
        catch (e) {
            if (e instanceof _1.__.PatchKit.AppInstalledVersionDoesNotExist) {
                return undefined;
            }
            throw e;
        }
    });
    const appBranchLastInstalledVersionId = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.PatchKit.fetchAppLastInstalledVersionId({
                appRootDirPath: appBranchRootDirState.path,
            });
        }
        catch (e) {
            if (e instanceof _1.__.PatchKit.AppLastInstalledVersionDoesNotExist) {
                return undefined;
            }
            throw e;
        }
    });
    const appBranchLastFinishedDataTaskInfo = await _1.__.invokeAsyncExp(async () => {
        const patchKitAppLastFinishedDataTaskInfo = await _1.__.invokeAsyncExp(async () => {
            try {
                return await _1.__.PatchKit.fetchAppLastFinishedDataTaskInfo({
                    appRootDirPath: appBranchRootDirState.path,
                });
            }
            catch (e) {
                if (e instanceof _1.__.PatchKit.AppLastFinishedDataTaskDoesNotExist) {
                    return undefined;
                }
                throw e;
            }
        });
        return _1.__.invokeSyncExp(() => {
            if (patchKitAppLastFinishedDataTaskInfo === undefined) {
                return undefined;
            }
            switch (patchKitAppLastFinishedDataTaskInfo.type) {
                case _1.__.PatchKit.AppTaskType.UpdateTask:
                    return {
                        type: _1._.AppBranchTaskType.UpdateTask,
                        progress: patchKitAppLastFinishedDataTaskInfo.progress,
                        errorTypeName: _1.__.invokeSyncExp(() => {
                            if (patchKitAppLastFinishedDataTaskInfo.errorTypeName === undefined) {
                                return undefined;
                            }
                            switch (patchKitAppLastFinishedDataTaskInfo.errorTypeName) {
                                case _1.__.PatchKit.AppDownloadingAccessTokenIsExpired.name:
                                    return _1._.AppBranchDownloadingAccessTokenIsExpired.name;
                                case _1.__.PatchKit.AppDownloadingAccessTokenIsInvalid.name:
                                    return _1._.AppBranchDownloadingAccessTokenIsInvalid.name;
                                case _1.__.PatchKit.AppLicenseKeyIsBlocked.name:
                                    return _1._.AppBranchLicenseKeyIsBlocked.name;
                                case _1.__.PatchKit.AppLicenseKeyIsInvalid.name:
                                    return _1._.AppBranchLicenseKeyIsInvalid.name;
                                case _1.__.PatchKit.AppLicenseKeyIsRequired.name:
                                    return _1._.AppBranchLicenseKeyIsRequired.name;
                                case _1.__.PatchKit.AppPasswordIsInvalid.name:
                                    return _1._.AppBranchPasswordIsInvalid.name;
                                case _1.__.PatchKit.AppRootDirDoesNotHaveEnoughFreeSpaceForUpdateTask.name:
                                    return _1._.AppBranchRootDirDoesNotHaveEnoughFreeSpaceForUpdateTask.name;
                                case _1.__.PatchKit.AppRootDirPermissionsAreInsufficient.name:
                                    return _1._.AppBranchRootDirPermissionsAreInsufficient.name;
                                case _1.__.PatchKit.AppUpdateTaskHasBeenCancelled.name:
                                    return _1._.AppBranchUpdateTaskHasBeenCancelled.name;
                                case _1.__.FatalError.name:
                                    return _1.__.FatalError.name;
                            }
                        }),
                    };
                case _1.__.PatchKit.AppTaskType.RepairTask:
                    return {
                        type: _1._.AppBranchTaskType.RepairTask,
                        progress: patchKitAppLastFinishedDataTaskInfo.progress,
                        errorTypeName: _1.__.invokeSyncExp(() => {
                            if (patchKitAppLastFinishedDataTaskInfo.errorTypeName === undefined) {
                                return undefined;
                            }
                            switch (patchKitAppLastFinishedDataTaskInfo.errorTypeName) {
                                case _1.__.PatchKit.AppDownloadingAccessTokenIsExpired.name:
                                    return _1._.AppBranchDownloadingAccessTokenIsExpired.name;
                                case _1.__.PatchKit.AppDownloadingAccessTokenIsInvalid.name:
                                    return _1._.AppBranchDownloadingAccessTokenIsInvalid.name;
                                case _1.__.PatchKit.AppLicenseKeyIsBlocked.name:
                                    return _1._.AppBranchLicenseKeyIsBlocked.name;
                                case _1.__.PatchKit.AppLicenseKeyIsInvalid.name:
                                    return _1._.AppBranchLicenseKeyIsInvalid.name;
                                case _1.__.PatchKit.AppLicenseKeyIsRequired.name:
                                    return _1._.AppBranchLicenseKeyIsRequired.name;
                                case _1.__.PatchKit.AppPasswordIsInvalid.name:
                                    return _1._.AppBranchPasswordIsInvalid.name;
                                case _1.__.PatchKit.AppRootDirDoesNotHaveEnoughFreeSpaceForRepairTask.name:
                                    return _1._.AppBranchRootDirDoesNotHaveEnoughFreeSpaceForRepairTask.name;
                                case _1.__.PatchKit.AppRootDirPermissionsAreInsufficient.name:
                                    return _1._.AppBranchRootDirPermissionsAreInsufficient.name;
                                case _1.__.PatchKit.AppRepairTaskHasBeenCancelled.name:
                                    return _1._.AppBranchRepairTaskHasBeenCancelled.name;
                                case _1.__.FatalError.name:
                                    return _1.__.FatalError.name;
                            }
                        }),
                    };
            }
        });
    });
    const appBranchInternalState = {
        rootDirState: appBranchRootDirState,
        installedVersionId: appBranchInstalledVersionId,
        lastInstalledVersionId: appBranchLastInstalledVersionId,
        doesNeedRepairing: doesAppBranchNeedRepairing,
        lastFinishedDataTaskInfo: appBranchLastFinishedDataTaskInfo,
    };
    if (((_c = apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId]) === null || _c === void 0 ? void 0 : _c[appBranchId]) !== undefined) {
        apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId][appBranchId].value =
            appBranchInternalState;
    }
    else {
        (_d = (_f = apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value)[appId]) !== null && _d !== void 0 ? _d : (_f[appId] = {});
        apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId][appBranchId] = {
            value: appBranchInternalState,
        };
    }
    return appBranchInternalState;
}).fetchAppBranchInternalState;
exports.fetchAppBranchInternalState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-app-branch-internal-state.js.map