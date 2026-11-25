"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startQuitTaskDef = void 0;
const _1 = require("./$");
const ongoing_quit_task_is_not_finished_1 = require("./ongoing-quit-task-is-not-finished");
exports.startQuitTaskDef = _1.__.defineManagedFunc()
    .name(`startQuitTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    ongoing_quit_task_is_not_finished_1.OngoingQuitTaskIsNotFinished,
])
    .async()
    .executeType().startQuitTaskDef;
//# sourceMappingURL=start-quit-task-def.js.map