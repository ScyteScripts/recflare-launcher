"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesApplyUpdateTaskExist = void 0;
const _1 = require("./$");
const does_apply_update_task_exist_1 = require("./does-apply-update-task-exist");
exports.fetchDoesApplyUpdateTaskExist = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchDoesApplyUpdateTaskExistDef)
    .execute(async () => {
    return does_apply_update_task_exist_1.DOES_APPLY_UPDATE_TASK_EXIST.value;
}).fetchDoesApplyUpdateTaskExist;
exports.fetchDoesApplyUpdateTaskExist.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-does-apply-update-task-exist.js.map