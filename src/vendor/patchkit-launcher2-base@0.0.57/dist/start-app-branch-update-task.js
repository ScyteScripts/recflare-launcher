"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchUpdateTask = void 0;
const _1 = require("./$");
const apps_branches_ongoing_task_id_box_1 = require("./apps-branches-ongoing-task-id-box");
const apps_branches_update_tasks_cancellation_token_box_1 = require("./apps-branches-update-tasks-cancellation-token-box");
const gen_app_branch_new_task_id_1 = require("./gen-app-branch-new-task-id");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
const enqueue_stats_app_branch_entry_1 = require("./enqueue-stats-app-branch-entry");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
const fetch_app_branch_task_state_1 = require("./fetch-app-branch-task-state");
const apps_branches_tasks_state_box_1 = require("./apps-branches-tasks-state-box");
exports.startAppBranchUpdateTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startAppBranchUpdateTaskDef)
    .execute(async ({ appId, appAccessToken, appBranchId, appBranchDownloadingCredentials, }) => {
    const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
    const appBranchOngoingTaskIdBox = _1.__.invokeSyncExp(() => {
        var _a, _b;
        var _c;
        if (((_a = apps_branches_ongoing_task_id_box_1.APPS_BRANCHES_ONGOING_TASK_ID_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === undefined) {
            (_b = (_c = apps_branches_ongoing_task_id_box_1.APPS_BRANCHES_ONGOING_TASK_ID_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
            apps_branches_ongoing_task_id_box_1.APPS_BRANCHES_ONGOING_TASK_ID_BOX.value[appId][appBranchId] = {
                value: undefined,
            };
        }
        return apps_branches_ongoing_task_id_box_1.APPS_BRANCHES_ONGOING_TASK_ID_BOX.value[appId][appBranchId];
    });
    if (appBranchOngoingTaskIdBox.value !== undefined) {
        throw new _1._.AppBranchOngoingTaskExists();
    }
    const appBranchInternalState = await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
        appId,
        appBranchId,
    });
    if (!appBranchInternalState.rootDirState.arePermissionsSufficient) {
        throw new _1._.AppBranchRootDirPermissionsAreInsufficient();
    }
    if (appBranchInternalState.doesNeedRepairing) {
        throw new _1._.AppBranchNeedsRepairing();
    }
    const appBranchRootDirPath = appBranchInternalState.rootDirState.path;
    const { appUpdateTaskId: patchKitAppUpdateTaskId } = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.PatchKit.startAppUpdateTask({
                appRootDirPath: appBranchRootDirPath,
                appSecret: appBranchInfo.secret,
                appPassword: appBranchDownloadingCredentials === null || appBranchDownloadingCredentials === void 0 ? void 0 : appBranchDownloadingCredentials.password,
                appLicenseKey: appBranchDownloadingCredentials === null || appBranchDownloadingCredentials === void 0 ? void 0 : appBranchDownloadingCredentials.licenseKey,
                appAccessToken: appBranchDownloadingCredentials === null || appBranchDownloadingCredentials === void 0 ? void 0 : appBranchDownloadingCredentials.downloadingAccessToken,
            });
        }
        catch (e) {
            try {
                await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                    appId,
                    appBranchId,
                });
            }
            catch (_a) {
            }
            if (e instanceof _1.__.PatchKit.AppRootDirPermissionsAreInsufficient) {
                throw new _1._.AppBranchRootDirPermissionsAreInsufficient();
            }
            if (e instanceof _1.__.PatchKit.AppNeedsRepairing) {
                throw new _1._.AppBranchNeedsRepairing();
            }
            throw e;
        }
    });
    try {
        await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
            statsAppBranchEntryInfo: {
                type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUpdateTaskStartedEntry,
                appId,
                appBranchId,
            },
        });
    }
    catch (_a) {
    }
    const appBranchUpdateTaskStartedDate = new Date();
    const appBranchUpdateTaskId = (0, gen_app_branch_new_task_id_1.genAppBranchNewTaskId)({
        appId,
        appBranchId,
    });
    appBranchOngoingTaskIdBox.value = appBranchUpdateTaskId;
    const appBranchUpdateTaskStateBox = _1.__.invokeSyncExp(() => {
        var _a, _b, _c, _d;
        var _e, _f;
        if (((_b = (_a = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchUpdateTaskId]) === undefined) {
            (_c = (_e = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value)[appId]) !== null && _c !== void 0 ? _c : (_e[appId] = {});
            (_d = (_f = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId])[appBranchId]) !== null && _d !== void 0 ? _d : (_f[appBranchId] = {});
            apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchUpdateTaskId] = {
                value: undefined,
            };
        }
        return apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchUpdateTaskId];
    });
    appBranchUpdateTaskStateBox.value = {
        type: _1._.AppBranchTaskType.UpdateTask,
        isFinished: false,
    };
    // eslint-disable-next-line @typescript-eslint/tslint/config
    (async () => {
        let appBranchUpdateTaskProgress;
        try {
            let patchKitAppUpdateTaskState;
            let hasPatchKitAppUpdateTaskBeenCancelled = false;
            do {
                const appBranchUpdateTaskCancellationTokenBox = _1.__.invokeSyncExp(() => {
                    var _a, _b, _c, _d;
                    var _e, _f;
                    if (((_b = (_a = apps_branches_update_tasks_cancellation_token_box_1.APPS_BRANCHES_UPDATE_TASKS_CANCELLATION_TOKEN_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchUpdateTaskId]) === undefined) {
                        (_c = (_e = apps_branches_update_tasks_cancellation_token_box_1.APPS_BRANCHES_UPDATE_TASKS_CANCELLATION_TOKEN_BOX.value)[appId]) !== null && _c !== void 0 ? _c : (_e[appId] = {});
                        (_d = (_f = apps_branches_update_tasks_cancellation_token_box_1.APPS_BRANCHES_UPDATE_TASKS_CANCELLATION_TOKEN_BOX.value[appId])[appBranchId]) !== null && _d !== void 0 ? _d : (_f[appBranchId] = {});
                        apps_branches_update_tasks_cancellation_token_box_1.APPS_BRANCHES_UPDATE_TASKS_CANCELLATION_TOKEN_BOX.value[appId][appBranchId][appBranchUpdateTaskId] = {
                            value: undefined,
                        };
                    }
                    return apps_branches_update_tasks_cancellation_token_box_1.APPS_BRANCHES_UPDATE_TASKS_CANCELLATION_TOKEN_BOX.value[appId][appBranchId][appBranchUpdateTaskId];
                });
                if (appBranchUpdateTaskCancellationTokenBox.value !== undefined
                    &&
                        !hasPatchKitAppUpdateTaskBeenCancelled) {
                    try {
                        await _1.__.PatchKit.cancelAppUpdateTask({
                            appUpdateTaskId: patchKitAppUpdateTaskId,
                        });
                        hasPatchKitAppUpdateTaskBeenCancelled = true;
                    }
                    catch (_a) {
                    }
                }
                try {
                    patchKitAppUpdateTaskState = await _1.__.PatchKit.fetchAppUpdateTaskState({
                        appUpdateTaskId: patchKitAppUpdateTaskId,
                    });
                    appBranchUpdateTaskProgress = patchKitAppUpdateTaskState.progress;
                    appBranchUpdateTaskStateBox.value = {
                        type: _1._.AppBranchTaskType.UpdateTask,
                        progress: appBranchUpdateTaskProgress,
                        isFinished: false,
                    };
                }
                catch (_b) {
                }
            } while (patchKitAppUpdateTaskState === undefined
                ||
                    !patchKitAppUpdateTaskState.isFinished);
            try {
                await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                    appId,
                    appBranchId,
                });
            }
            catch (_c) {
            }
            if (patchKitAppUpdateTaskState.errorTypeName !== undefined) {
                appBranchUpdateTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.UpdateTask,
                    progress: appBranchUpdateTaskProgress,
                    errorTypeName: _1.__.invokeSyncExp(() => {
                        _1.__.checkAssertion(patchKitAppUpdateTaskState !== undefined);
                        if (patchKitAppUpdateTaskState.errorTypeName === undefined) {
                            return undefined;
                        }
                        switch (patchKitAppUpdateTaskState.errorTypeName) {
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
                    isFinished: true,
                };
            }
            else {
                appBranchUpdateTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.UpdateTask,
                    progress: appBranchUpdateTaskProgress,
                    isFinished: true,
                };
            }
        }
        catch (e) {
            try {
                await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                    appId,
                    appBranchId,
                });
            }
            catch (_d) {
            }
            appBranchUpdateTaskStateBox.value = {
                type: _1._.AppBranchTaskType.UpdateTask,
                progress: appBranchUpdateTaskProgress,
                errorTypeName: _1.__.FatalError.name,
                isFinished: true,
            };
        }
        finally {
            appBranchOngoingTaskIdBox.value = undefined;
            try {
                const appBranchTaskState = await (0, fetch_app_branch_task_state_1.fetchAppBranchTaskState)({
                    appId,
                    appBranchId,
                    appBranchTaskId: appBranchUpdateTaskId,
                });
                _1.__.checkAssertion(appBranchTaskState.type === _1._.AppBranchTaskType.UpdateTask);
                await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                    statsAppBranchEntryInfo: appBranchTaskState.errorTypeName === undefined
                        ? {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUpdateTaskSucceededEntry,
                            appId,
                            appBranchId,
                            appBranchUpdateTaskDurationSeconds: ((new Date()).getTime() - appBranchUpdateTaskStartedDate.getTime()) / 1000,
                        }
                        : {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUpdateTaskFailedEntry,
                            appId,
                            appBranchId,
                            appBranchUpdateTaskErrorTypeName: appBranchTaskState.errorTypeName,
                        },
                });
            }
            catch (_e) {
            }
        }
    })();
    return {
        appBranchUpdateTaskId,
    };
}).startAppBranchUpdateTask;
//# sourceMappingURL=start-app-branch-update-task.js.map