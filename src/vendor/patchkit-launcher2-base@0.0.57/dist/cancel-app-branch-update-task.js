"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.cancelAppBranchUpdateTask = void 0;
const _1 = require("./$");
const apps_branches_tasks_state_box_1 = require("./apps-branches-tasks-state-box");
const apps_branches_update_tasks_cancellation_token_box_1 = require("./apps-branches-update-tasks-cancellation-token-box");
exports.cancelAppBranchUpdateTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.cancelAppBranchUpdateTaskDef)
    .execute(async ({ appId, appBranchId, appBranchUpdateTaskId, }) => {
    var _a, _b;
    const appBranchTaskStateBox = (_b = (_a = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchUpdateTaskId];
    if ((appBranchTaskStateBox === null || appBranchTaskStateBox === void 0 ? void 0 : appBranchTaskStateBox.value) === undefined
        ||
            appBranchTaskStateBox.value.type !== _1._.AppBranchTaskType.UpdateTask) {
        throw new _1._.AppBranchUpdateTaskDoesNotExist();
    }
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
    if (appBranchUpdateTaskCancellationTokenBox.value !== undefined) {
        throw new _1._.AppBranchUpdateTaskHasBeenAlreadyCancelled();
    }
    appBranchUpdateTaskCancellationTokenBox.value = {};
}).cancelAppBranchUpdateTask;
//# sourceMappingURL=cancel-app-branch-update-task.js.map