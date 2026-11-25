"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchRepairTask = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const apps_branches_ongoing_task_id_box_1 = require("./apps-branches-ongoing-task-id-box");
const apps_branches_repair_tasks_cancellation_token_box_1 = require("./apps-branches-repair-tasks-cancellation-token-box");
const gen_app_branch_new_task_id_1 = require("./gen-app-branch-new-task-id");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
const enqueue_stats_app_branch_entry_1 = require("./enqueue-stats-app-branch-entry");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
const fetch_app_branch_task_state_1 = require("./fetch-app-branch-task-state");
const apps_branches_tasks_state_box_1 = require("./apps-branches-tasks-state-box");
exports.startAppBranchRepairTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startAppBranchRepairTaskDef)
    .execute(async ({ appId, appAccessToken, appBranchId, appBranchDownloadingCredentials, }) => {
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
    // Repair routing before metadata/download requests, which may themselves be misrouted.
    await require('@recovered/routing-repair').repairLauncherRouting({
        appId, configName: config_1.CONFIG.value.name, discoverUnknown: false,
    });
    const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
        appId, appAccessToken, appBranchId,
    });
    const appBranchInternalState = await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
        appId,
        appBranchId,
    });
    if (!appBranchInternalState.rootDirState.arePermissionsSufficient) {
        throw new _1._.AppBranchRootDirPermissionsAreInsufficient();
    }
    const appBranchRootDirPath = appBranchInternalState.rootDirState.path;
    if (appId === require('@recovered/routing-repair').STUDIO_APP_ID) {
        // A dev-pinned template must be re-downloaded from RecFlare, not edited
        // into a state that bypasses release-manifest validation.
        require('@recovered/routing-repair').removeDevelopmentTemplate(appBranchRootDirPath);
    }
    const { appRepairTaskId: patchKitAppRepairTaskId } = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.PatchKit.startAppRepairTask({
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
            throw e;
        }
    });
    try {
        await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
            statsAppBranchEntryInfo: {
                type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchRepairTaskStartedEntry,
                appId,
                appBranchId,
            },
        });
    }
    catch (_a) {
    }
    const appBranchRepairTaskStartedDate = new Date();
    const appBranchRepairTaskId = (0, gen_app_branch_new_task_id_1.genAppBranchNewTaskId)({
        appId,
        appBranchId,
    });
    appBranchOngoingTaskIdBox.value = appBranchRepairTaskId;
    const appBranchRepairTaskStateBox = _1.__.invokeSyncExp(() => {
        var _a, _b, _c, _d;
        var _e, _f;
        if (((_b = (_a = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchRepairTaskId]) === undefined) {
            (_c = (_e = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value)[appId]) !== null && _c !== void 0 ? _c : (_e[appId] = {});
            (_d = (_f = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId])[appBranchId]) !== null && _d !== void 0 ? _d : (_f[appBranchId] = {});
            apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchRepairTaskId] = {
                value: undefined,
            };
        }
        return apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchRepairTaskId];
    });
    appBranchRepairTaskStateBox.value = {
        type: _1._.AppBranchTaskType.RepairTask,
        isFinished: false,
    };
    // eslint-disable-next-line @typescript-eslint/tslint/config
    (async () => {
        let appBranchRepairTaskProgress;
        try {
            let patchKitAppRepairTaskState;
            let hasPatchKitAppRepairTaskBeenCancelled = false;
            const appBranchRepairTaskCancellationTokenBox = _1.__.invokeSyncExp(() => {
                var _a, _b, _c, _d;
                var _e, _f;
                if (((_b = (_a = apps_branches_repair_tasks_cancellation_token_box_1.APPS_BRANCHES_REPAIR_TASKS_CANCELLATION_TOKEN_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchRepairTaskId]) === undefined) {
                    (_c = (_e = apps_branches_repair_tasks_cancellation_token_box_1.APPS_BRANCHES_REPAIR_TASKS_CANCELLATION_TOKEN_BOX.value)[appId]) !== null && _c !== void 0 ? _c : (_e[appId] = {});
                    (_d = (_f = apps_branches_repair_tasks_cancellation_token_box_1.APPS_BRANCHES_REPAIR_TASKS_CANCELLATION_TOKEN_BOX.value[appId])[appBranchId]) !== null && _d !== void 0 ? _d : (_f[appBranchId] = {});
                    apps_branches_repair_tasks_cancellation_token_box_1.APPS_BRANCHES_REPAIR_TASKS_CANCELLATION_TOKEN_BOX.value[appId][appBranchId][appBranchRepairTaskId] = {
                        value: undefined,
                    };
                }
                return apps_branches_repair_tasks_cancellation_token_box_1.APPS_BRANCHES_REPAIR_TASKS_CANCELLATION_TOKEN_BOX.value[appId][appBranchId][appBranchRepairTaskId];
            });
            do {
                if (appBranchRepairTaskCancellationTokenBox.value !== undefined
                    &&
                        !hasPatchKitAppRepairTaskBeenCancelled) {
                    try {
                        await _1.__.PatchKit.cancelAppRepairTask({
                            appRepairTaskId: patchKitAppRepairTaskId,
                        });
                        hasPatchKitAppRepairTaskBeenCancelled = true;
                    }
                    catch (_a) {
                    }
                }
                try {
                    patchKitAppRepairTaskState = await _1.__.PatchKit.fetchAppRepairTaskState({
                        appRepairTaskId: patchKitAppRepairTaskId,
                    });
                    appBranchRepairTaskProgress = patchKitAppRepairTaskState.progress;
                    appBranchRepairTaskStateBox.value = {
                        type: _1._.AppBranchTaskType.RepairTask,
                        progress: appBranchRepairTaskProgress,
                        isFinished: false,
                    };
                }
                catch (_b) {
                }
            } while (patchKitAppRepairTaskState === undefined
                ||
                    !patchKitAppRepairTaskState.isFinished);
            try {
                await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                    appId,
                    appBranchId,
                });
            }
            catch (_c) {
            }
            if (patchKitAppRepairTaskState.errorTypeName !== undefined) {
                appBranchRepairTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.RepairTask,
                    progress: appBranchRepairTaskProgress,
                    errorTypeName: _1.__.invokeSyncExp(() => {
                        _1.__.checkAssertion(patchKitAppRepairTaskState !== undefined);
                        if (patchKitAppRepairTaskState.errorTypeName === undefined) {
                            return undefined;
                        }
                        switch (patchKitAppRepairTaskState.errorTypeName) {
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
                    isFinished: false,
                };
            }
            else {
                // Recheck user routing after updater completion. Template files remain
                // owned by the updater and retain their release-manifest hashes.
                if (appId === require('@recovered/routing-repair').STUDIO_APP_ID)
                    require('@recovered/routing-repair').completeTemplateReinstall(appBranchRootDirPath);
                const routingWarnings = [];
                await require('@recovered/routing-repair').repairLauncherRouting({
                    appId, configName: config_1.CONFIG.value.name, appRootDirPath: appBranchRootDirPath,
                    onWarning: warning => routingWarnings.push(warning),
                });
                appBranchRepairTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.RepairTask,
                    progress: appBranchRepairTaskProgress,
                    isFinished: true,
                };
                if (routingWarnings.length && _1.__.$electron?.dialog?.showMessageBox) {
                    // Optional project checks must not invalidate a completed template repair.
                    // Most often the projects are open in Unity: say that plainly and list them.
                    const openProjects = routingWarnings.filter(w => w && w.code === 'PROJECT_OPEN' && w.path).map(w => w.path);
                    const allOpen = openProjects.length === routingWarnings.length;
                    const list = openProjects.map(p => '  • ' + require('path').basename(p)).join('\n');
                    try {Promise.resolve(_1.__.$electron.dialog.showMessageBox({
                        type: 'warning', title: 'Close Unity to finish',
                        message: allOpen
                            ? 'Studio is repaired. Close Unity fully to finish.'
                            : 'Studio is repaired, but some projects could not be checked.',
                        detail: (openProjects.length
                            ? (openProjects.length === 1 ? 'This project is' : 'These projects are') + ' open in Unity, so ' + (openProjects.length === 1 ? 'it was' : 'they were') + ' skipped:\n' + list + '\n\n'
                            : '')
                            + 'Close Unity and Unity Hub completely, then click Repair again.'
                            + (allOpen ? '' : '\n\nDetails are in the launcher logs/routing-repair.json file.'),
                    })).catch(() => {});} catch (_noticeError) {}
                }
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
            appBranchRepairTaskStateBox.value = {
                type: _1._.AppBranchTaskType.RepairTask,
                progress: appBranchRepairTaskProgress,
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
                    appBranchTaskId: appBranchRepairTaskId,
                });
                _1.__.checkAssertion(appBranchTaskState.type === _1._.AppBranchTaskType.RepairTask);
                await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                    statsAppBranchEntryInfo: appBranchTaskState.errorTypeName === undefined
                        ? {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchRepairTaskSucceededEntry,
                            appId,
                            appBranchId,
                            appBranchRepairTaskDurationSeconds: ((new Date()).getTime() - appBranchRepairTaskStartedDate.getTime()) / 1000,
                        }
                        : {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchRepairTaskFailedEntry,
                            appId,
                            appBranchId,
                            appBranchRepairTaskErrorTypeName: appBranchTaskState.errorTypeName,
                        },
                });
            }
            catch (_e) {
            }
        }
    })();
    return {
        appBranchRepairTaskId,
    };
}).startAppBranchRepairTask;
//# sourceMappingURL=start-app-branch-repair-task.js.map
