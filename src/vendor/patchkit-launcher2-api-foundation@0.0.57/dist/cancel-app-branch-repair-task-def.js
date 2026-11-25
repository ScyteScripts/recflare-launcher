"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.cancelAppBranchRepairTaskDef = void 0;
const _1 = require("./$");
const app_branch_repair_task_does_not_exist_1 = require("./app-branch-repair-task-does-not-exist");
const app_branch_repair_task_has_been_already_cancelled_1 = require("./app-branch-repair-task-has-been-already-cancelled");
exports.cancelAppBranchRepairTaskDef = _1.__.defineManagedFunc()
    .name(`cancelAppBranchRepairTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_repair_task_does_not_exist_1.AppBranchRepairTaskDoesNotExist,
    app_branch_repair_task_has_been_already_cancelled_1.AppBranchRepairTaskHasBeenAlreadyCancelled,
])
    .async()
    .executeType().cancelAppBranchRepairTaskDef;
//# sourceMappingURL=cancel-app-branch-repair-task-def.js.map