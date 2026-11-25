"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startApplyUpdateTaskDef = void 0;
const _1 = require("./$");
const ongoing_apply_update_task_is_not_finished_1 = require("./ongoing-apply-update-task-is-not-finished");
const ongoing_quit_task_is_not_finished_1 = require("./ongoing-quit-task-is-not-finished");
const update_does_not_exist_1 = require("./update-does-not-exist");
const update_is_not_downloaded_1 = require("./update-is-not-downloaded");
exports.startApplyUpdateTaskDef = _1.__.defineManagedFunc()
    .name(`startApplyUpdateTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    ongoing_apply_update_task_is_not_finished_1.OngoingApplyUpdateTaskIsNotFinished,
    ongoing_quit_task_is_not_finished_1.OngoingQuitTaskIsNotFinished,
    update_is_not_downloaded_1.UpdateIsNotDownloaded,
    update_does_not_exist_1.UpdateDoesNotExist,
])
    .async()
    .executeType().startApplyUpdateTaskDef;
//# sourceMappingURL=start-apply-update-task-def.js.map