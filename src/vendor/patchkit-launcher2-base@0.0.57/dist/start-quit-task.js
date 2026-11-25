"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startQuitTask = void 0;
const _1 = require("./$");
const does_quit_task_exist_1 = require("./does-quit-task-exist");
exports.startQuitTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startQuitTaskDef)
    .execute(async () => {
    if (does_quit_task_exist_1.DOES_QUIT_TASK_EXIST.value) {
        throw new _1._.OngoingQuitTaskIsNotFinished();
    }
    does_quit_task_exist_1.DOES_QUIT_TASK_EXIST.value = true;
    _1.__.$electron.app.quit();
}).startQuitTask;
//# sourceMappingURL=start-quit-task.js.map