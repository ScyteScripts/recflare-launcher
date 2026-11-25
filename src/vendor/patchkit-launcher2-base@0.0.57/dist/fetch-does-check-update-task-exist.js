"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesCheckUpdateTaskExist = void 0;
const _1 = require("./$");
const does_check_update_task_exist_1 = require("./does-check-update-task-exist");
exports.fetchDoesCheckUpdateTaskExist = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchDoesCheckUpdateTaskExistDef)
    .execute(async () => {
    return does_check_update_task_exist_1.DOES_CHECK_UPDATE_TASK_EXIST.value;
}).fetchDoesCheckUpdateTaskExist;
exports.fetchDoesCheckUpdateTaskExist.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-does-check-update-task-exist.js.map