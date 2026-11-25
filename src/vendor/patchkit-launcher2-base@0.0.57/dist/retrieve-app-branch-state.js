"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppBranchState = void 0;
const _1 = require("./$");
const apps_branches_ongoing_task_id_box_1 = require("./apps-branches-ongoing-task-id-box");
const apps_branches_process_info_box_1 = require("./apps-branches-process-info-box");
const retrieve_app_branch_internal_state_1 = require("./retrieve-app-branch-internal-state");
exports.retrieveAppBranchState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.retrieveAppBranchStateDef)
    .execute(async ({ appId, appBranchId, }) => {
    var _a, _b;
    const appBranchInternalState = await (0, retrieve_app_branch_internal_state_1.retrieveAppBranchInternalState)({
        appId,
        appBranchId,
    });
    const appBranchOngoingTaskIdBox = (_a = apps_branches_ongoing_task_id_box_1.APPS_BRANCHES_ONGOING_TASK_ID_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId];
    const appBranchProcessInfoBox = (_b = apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId]) === null || _b === void 0 ? void 0 : _b[appBranchId];
    return {
        rootDir: appBranchInternalState.rootDirState,
        installedVersionId: appBranchInternalState.installedVersionId,
        lastInstalledVersionId: appBranchInternalState.lastInstalledVersionId,
        doesNeedRepairing: appBranchInternalState.doesNeedRepairing,
        ongoingTaskId: appBranchOngoingTaskIdBox === null || appBranchOngoingTaskIdBox === void 0 ? void 0 : appBranchOngoingTaskIdBox.value,
        lastFinishedDataTaskInfo: appBranchInternalState.lastFinishedDataTaskInfo,
        processInfo: appBranchProcessInfoBox === null || appBranchProcessInfoBox === void 0 ? void 0 : appBranchProcessInfoBox.value,
    };
}).retrieveAppBranchState;
exports.retrieveAppBranchState.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-app-branch-state.js.map