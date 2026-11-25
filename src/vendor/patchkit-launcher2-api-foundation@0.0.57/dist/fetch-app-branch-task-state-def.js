"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchTaskStateDef = void 0;
const _1 = require("./$");
const app_branch_task_does_not_exist_1 = require("./app-branch-task-does-not-exist");
exports.fetchAppBranchTaskStateDef = _1.__.defineManagedFunc()
    .name(`fetchAppBranchTaskState`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_task_does_not_exist_1.AppBranchTaskDoesNotExist,
])
    .async()
    .executeType().fetchAppBranchTaskStateDef;
//# sourceMappingURL=fetch-app-branch-task-state-def.js.map