"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.cancelAppBranchUpdateTaskDef = void 0;
const _1 = require("./$");
const app_branch_update_task_does_not_exist_1 = require("./app-branch-update-task-does-not-exist");
const app_branch_update_task_has_been_already_cancelled_1 = require("./app-branch-update-task-has-been-already-cancelled");
exports.cancelAppBranchUpdateTaskDef = _1.__.defineManagedFunc()
    .name(`cancelAppBranchUpdateTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_update_task_does_not_exist_1.AppBranchUpdateTaskDoesNotExist,
    app_branch_update_task_has_been_already_cancelled_1.AppBranchUpdateTaskHasBeenAlreadyCancelled,
])
    .async()
    .executeType().cancelAppBranchUpdateTaskDef;
//# sourceMappingURL=cancel-app-branch-update-task-def.js.map