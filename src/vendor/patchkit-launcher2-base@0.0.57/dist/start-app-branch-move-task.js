"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchMoveTask = void 0;
const _1 = require("./$");
const apps_branches_ongoing_task_id_box_1 = require("./apps-branches-ongoing-task-id-box");
const gen_app_branch_new_task_id_1 = require("./gen-app-branch-new-task-id");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
const fetch_settings_1 = require("./fetch-settings");
const save_settings_file_store_1 = require("./save-settings-file-store");
const fetch_app_branch_task_state_1 = require("./fetch-app-branch-task-state");
const enqueue_stats_app_branch_entry_1 = require("./enqueue-stats-app-branch-entry");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
const apps_branches_tasks_state_box_1 = require("./apps-branches-tasks-state-box");
exports.startAppBranchMoveTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startAppBranchMoveTaskDef)
    .execute(async ({ appId, appBranchId, appBranchNewRootDirPath, }) => {
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
    const { appMoveTaskId: patchKitAppMoveTaskId } = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.PatchKit.startAppMoveTask({
                appRootDirPath: appBranchRootDirPath,
                appNewRootDirPath: appBranchNewRootDirPath,
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
            if (e instanceof _1.__.PatchKit.AppNewRootDirPermissionsAreInsufficient) {
                throw new _1._.AppBranchNewRootDirPermissionsAreInsufficient();
            }
            if (e instanceof _1.__.PatchKit.AppNewRootDirIsConflictingWithOtherFileSystemEntry) {
                throw new _1._.AppBranchNewRootDirIsConflictingWithOtherFileSystemEntry();
            }
            if (e instanceof _1.__.PatchKit.AppNewRootDirIsNotEmpty) {
                throw new _1._.AppBranchNewRootDirIsNotEmpty();
            }
            throw e;
        }
    });
    try {
        await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
            statsAppBranchEntryInfo: {
                type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchMoveTaskStartedEntry,
                appId,
                appBranchId,
            },
        });
    }
    catch (_a) {
    }
    const appBranchMoveTaskStartedDate = new Date();
    const appBranchMoveTaskId = (0, gen_app_branch_new_task_id_1.genAppBranchNewTaskId)({
        appId,
        appBranchId,
    });
    appBranchOngoingTaskIdBox.value = appBranchMoveTaskId;
    const appBranchMoveTaskStateBox = _1.__.invokeSyncExp(() => {
        var _a, _b, _c, _d;
        var _e, _f;
        if (((_b = (_a = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchMoveTaskId]) === undefined) {
            (_c = (_e = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value)[appId]) !== null && _c !== void 0 ? _c : (_e[appId] = {});
            (_d = (_f = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId])[appBranchId]) !== null && _d !== void 0 ? _d : (_f[appBranchId] = {});
            apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchMoveTaskId] = {
                value: undefined,
            };
        }
        return apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId][appBranchId][appBranchMoveTaskId];
    });
    appBranchMoveTaskStateBox.value = {
        type: _1._.AppBranchTaskType.MoveTask,
        isFinished: false,
    };
    // eslint-disable-next-line @typescript-eslint/tslint/config
    (async () => {
        try {
            let patchKitAppMoveTaskState;
            do {
                try {
                    patchKitAppMoveTaskState = await _1.__.PatchKit.fetchAppMoveTaskState({
                        appMoveTaskId: patchKitAppMoveTaskId,
                    });
                }
                catch (_a) {
                }
            } while (patchKitAppMoveTaskState === undefined
                ||
                    !patchKitAppMoveTaskState.isFinished);
            if (patchKitAppMoveTaskState.errorTypeName !== undefined) {
                appBranchMoveTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.MoveTask,
                    errorTypeName: _1.__.invokeSyncExp(() => {
                        _1.__.checkAssertion(patchKitAppMoveTaskState !== undefined);
                        if (patchKitAppMoveTaskState.errorTypeName === undefined) {
                            return undefined;
                        }
                        switch (patchKitAppMoveTaskState.errorTypeName) {
                            case _1.__.PatchKit.AppNewRootDirDoesNotHaveEnoughFreeSpaceForMoveTask.name:
                                return _1._.AppBranchNewRootDirDoesNotHaveEnoughFreeSpaceForMoveTask.name;
                            case _1.__.PatchKit.AppNewRootDirPermissionsAreInsufficient.name:
                                return _1._.AppBranchNewRootDirPermissionsAreInsufficient.name;
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
                const settings = await (0, fetch_settings_1.fetchSettings)();
                await (0, save_settings_file_store_1.saveSettingsFileStore)({
                    settings: _1.__.$deepmerge(settings, {
                        apps: {
                            [appId]: {
                                branches: {
                                    [appBranchId]: {
                                        rootDirPath: appBranchNewRootDirPath,
                                    },
                                },
                            },
                        },
                    }),
                });
                appBranchMoveTaskStateBox.value = {
                    type: _1._.AppBranchTaskType.MoveTask,
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
            catch (_b) {
            }
            appBranchMoveTaskStateBox.value = {
                type: _1._.AppBranchTaskType.MoveTask,
                errorTypeName: _1.__.FatalError.name,
                isFinished: true,
            };
        }
        finally {
            try {
                await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                    appId,
                    appBranchId,
                });
            }
            catch (_c) {
            }
            appBranchOngoingTaskIdBox.value = undefined;
            try {
                const appBranchTaskState = await (0, fetch_app_branch_task_state_1.fetchAppBranchTaskState)({
                    appId,
                    appBranchId,
                    appBranchTaskId: appBranchMoveTaskId,
                });
                _1.__.checkAssertion(appBranchTaskState.type === _1._.AppBranchTaskType.MoveTask);
                await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                    statsAppBranchEntryInfo: appBranchTaskState.errorTypeName === undefined
                        ? {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchMoveTaskSucceededEntry,
                            appId,
                            appBranchId,
                            appBranchMoveTaskDurationSeconds: ((new Date()).getTime() - appBranchMoveTaskStartedDate.getTime()) / 1000,
                        }
                        : {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchMoveTaskFailedEntry,
                            appId,
                            appBranchId,
                            appBranchMoveTaskErrorTypeName: appBranchTaskState.errorTypeName,
                        },
                });
            }
            catch (_d) {
            }
        }
    })();
    return {
        appBranchMoveTaskId,
    };
}).startAppBranchMoveTask;
//# sourceMappingURL=start-app-branch-move-task.js.map