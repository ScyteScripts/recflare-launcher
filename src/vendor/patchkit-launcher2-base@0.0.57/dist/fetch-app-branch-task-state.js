"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchTaskState = void 0;
const _1 = require("./$");
const apps_branches_tasks_state_box_1 = require("./apps-branches-tasks-state-box");
exports.fetchAppBranchTaskState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchTaskStateDef)
    .execute(async ({ appId, appBranchId, appBranchTaskId, }) => {
    var _a, _b;
    const appBranchTaskStateBox = (_b = (_a = apps_branches_tasks_state_box_1.APPS_BRANCHES_TASKS_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === null || _b === void 0 ? void 0 : _b[appBranchTaskId];
    if ((appBranchTaskStateBox === null || appBranchTaskStateBox === void 0 ? void 0 : appBranchTaskStateBox.value) === undefined) {
        throw new _1._.AppBranchTaskDoesNotExist();
    }
    return appBranchTaskStateBox.value;
}).fetchAppBranchTaskState;
exports.fetchAppBranchTaskState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-app-branch-task-state.js.map