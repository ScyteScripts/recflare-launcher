"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchUninstallTask = void 0;
const _1 = require("./$");
const apps_branches_ongoing_task_id_box_1 = require("./apps-branches-ongoing-task-id-box");
const gen_app_branch_new_task_id_1 = require("./gen-app-branch-new-task-id");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
const enqueue_stats_app_branch_entry_1 = require("./enqueue-stats-app-branch-entry");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
const fetch_app_branch_task_state_1 = require("./fetch-app-branch-task-state");
const apps_branches_tasks_state_box_1 = require("./apps-branches-tasks-state-box");
exports.startAppBranchUninstallTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startAppBranchUninstallTaskDef)
    .execute(async ({ appId, appBranchId, }) => {
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
    const appBranchRootDirPath = appBranchInternalState.rootDirState.path;
    const { appUninstallTaskId: patchKitAppUninstallTaskId } = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.PatchKit.startAppUninstallTask({
                appRootDirPath: appBranchRootDirPath,
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
                type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUninstallTaskStartedEntry,
                appId,
                appBranchId,
            },
        });
    }
    catch (_a) {
    }
    const appBranchUninstallTaskStartedDate = new Date();
    const appBranchUninstallTaskId = (0, gen_app_branch_new_task_id_1.genAppBranchNewTaskId)({
        appId,
        appBranchId,
    });
    appBranchOngoingTaskIdBox.value = appBranchUninstallTaskId;
    const appBranchUninstallTaskStateBox = _1.__.invokeSyncExp(() => {
        var _a, _b, _c, _d;
        var _e, _f;
        if (((_b = (_a = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchUninstallTaskId]) === undefined) {
            (_c = (_e = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value)[appId]) !== null && _c !== void 0 ? _c : (_e[appId] = {});
            (_d = (_f = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId])[appBranchId]) !== null && _d !== void 0 ? _d : (_f[appBranchId] = {});
            apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchUninstallTaskId] = {
                value: undefined,
            };
        }
        return apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchUninstallTaskId];
    });
    appBranchUninstallTaskStateBox.value = {
        type: _1._.AppBranchTaskType.UninstallTask,
        isFinished: false,
    };
    // eslint-disable-next-line @typescript-eslint/tslint/config
    (async () => {
        try {
            let patchKitAppUninstallTaskState;
            do {
                try {
                    patchKitAppUninstallTaskState = await _1.__.PatchKit.fetchAppUninstallTaskState({
                        appUninstallTaskId: patchKitAppUninstallTaskId,
                    });
                }
                catch (_a) {
                }
            } while (patchKitAppUninstallTaskState === undefined
                ||
                    !patchKitAppUninstallTaskState.isFinished);
            try {
                await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                    appId,
                    appBranchId,
                });
            }
            catch (_b) {
            }
            if (patchKitAppUninstallTaskState.errorTypeName !== undefined) {
                appBranchUninstallTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.UninstallTask,
                    errorTypeName: _1.__.invokeSyncExp(() => {
                        _1.__.checkAssertion(patchKitAppUninstallTaskState !== undefined);
                        if (patchKitAppUninstallTaskState.errorTypeName === undefined) {
                            return undefined;
                        }
                        switch (patchKitAppUninstallTaskState.errorTypeName) {
                            case _1.__.PatchKit.AppRootDirPermissionsAreInsufficient.name:
                                return _1._.AppBranchRootDirPermissionsAreInsufficient.name;
                            case _1.__.FatalError.name:
                                return _1.__.FatalError.name;
                        }
                    }),
                    isFinished: true,
                };
            }
            else {
                appBranchUninstallTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.UninstallTask,
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
            catch (_c) {
            }
            appBranchUninstallTaskStateBox.value = {
                type: _1._.AppBranchTaskType.UninstallTask,
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
                    appBranchTaskId: appBranchUninstallTaskId,
                });
                _1.__.checkAssertion(appBranchTaskState.type === _1._.AppBranchTaskType.UninstallTask);
                await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                    statsAppBranchEntryInfo: appBranchTaskState.errorTypeName === undefined
                        ? {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUninstallTaskSucceededEntry,
                            appId,
                            appBranchId,
                            appBranchUninstallTaskDurationSeconds: ((new Date()).getTime() - appBranchUninstallTaskStartedDate.getTime()) / 1000,
                        }
                        : {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUninstallTaskFailedEntry,
                            appId,
                            appBranchId,
                            appBranchUninstallTaskErrorTypeName: appBranchTaskState.errorTypeName,
                        },
                });
            }
            catch (_d) {
            }
        }
    })();
    return {
        appBranchUninstallTaskId,
    };
}).startAppBranchUninstallTask;
//# sourceMappingURL=start-app-branch-uninstall-task.js.map