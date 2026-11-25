"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesQuitTaskExist = void 0;
const _1 = require("./$");
const does_quit_task_exist_1 = require("./does-quit-task-exist");
exports.fetchDoesQuitTaskExist = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchDoesQuitTaskExistDef)
    .execute(async () => {
    return does_quit_task_exist_1.DOES_QUIT_TASK_EXIST.value;
}).fetchDoesQuitTaskExist;
exports.fetchDoesQuitTaskExist.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-does-quit-task-exist.js.map