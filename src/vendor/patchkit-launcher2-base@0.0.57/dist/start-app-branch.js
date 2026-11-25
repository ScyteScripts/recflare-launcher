"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranch = void 0;
const _1 = require("./$");
const apps_branches_ongoing_task_id_box_1 = require("./apps-branches-ongoing-task-id-box");
const apps_branches_process_info_box_1 = require("./apps-branches-process-info-box");
const enqueue_stats_app_branch_entry_1 = require("./enqueue-stats-app-branch-entry");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
exports.startAppBranch = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startAppBranchDef)
    .execute(async ({ appId, appBranchId, appBranchProcessExtraArgsAsText, }) => {
    var _a, _b;
    try {
        try {
            await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                statsAppBranchEntryInfo: {
                    type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchStartTaskStartedEntry,
                    appId,
                    appBranchId,
                },
            });
        }
        catch (_c) {
        }
        if (((_b = (_a = apps_branches_ongoing_task_id_box_1.APPS_BRANCHES_ONGOING_TASK_ID_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b.value) !== undefined) {
            throw new _1._.AppBranchOngoingTaskExists();
        }
        const appBranchProcessInfoBox = _1.__.invokeSyncExp(() => {
            var _a, _b;
            var _c;
            if (((_a = apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === undefined) {
                (_b = (_c = apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
                apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId][appBranchId] = {
                    value: undefined,
                };
            }
            return apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId][appBranchId];
        });
        if (appBranchProcessInfoBox.value !== undefined) {
            throw new _1._.AppBranchProcessAlreadyExists();
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
        if (appBranchInternalState.installedVersionId === undefined) {
            throw new _1._.AppBranchIsNotInstalled();
        }
        const { appProcessId: appBranchProcessId } = await _1.__.invokeAsyncExp(async () => {
            try {
                return await _1.__.PatchKit.startApp({
                    appRootDirPath: appBranchInternalState.rootDirState.path,
                    appProcessExtraArgsAsText: appBranchProcessExtraArgsAsText,
                });
            }
            catch (e) {
                if (e instanceof _1.__.PatchKit.AppRootDirPermissionsAreInsufficient) {
                    throw new _1._.AppBranchRootDirPermissionsAreInsufficient();
                }
                if (e instanceof _1.__.PatchKit.AppNeedsRepairing) {
                    throw new _1._.AppBranchNeedsRepairing();
                }
                if (e instanceof _1.__.PatchKit.AppIsNotInstalled) {
                    throw new _1._.AppBranchIsNotInstalled();
                }
                throw e;
            }
        });
        const appBranchProcessInfo = {
            id: appBranchProcessId,
            startedDate: new Date(),
        };
        appBranchProcessInfoBox.value = appBranchProcessInfo;
        try {
            await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                statsAppBranchEntryInfo: {
                    type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchStartTaskSucceededEntry,
                    appId,
                    appBranchId,
                },
            });
        }
        catch (_d) {
        }
        return { appBranchProcessInfo };
    }
    catch (e) {
        try {
            await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                statsAppBranchEntryInfo: {
                    type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchStartTaskFailedEntry,
                    appId,
                    appBranchId,
                    appBranchStartTaskErrorTypeName: e instanceof Error
                        ? e.name
                        : `UnknownError`,
                },
            });
        }
        catch (_e) {
        }
        throw e;
    }
}).startAppBranch;
//# sourceMappingURL=start-app-branch.js.map