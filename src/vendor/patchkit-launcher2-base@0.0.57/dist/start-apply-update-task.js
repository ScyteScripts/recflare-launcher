"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startApplyUpdateTask = void 0;
const _1 = require("./$");
const does_apply_update_task_exist_1 = require("./does-apply-update-task-exist");
const does_quit_task_exist_1 = require("./does-quit-task-exist");
const update_state_1 = require("./update-state");
exports.startApplyUpdateTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startApplyUpdateTaskDef)
    .execute(async () => {
    if (does_apply_update_task_exist_1.DOES_APPLY_UPDATE_TASK_EXIST.value) {
        throw new _1._.OngoingQuitTaskIsNotFinished();
    }
    if (does_quit_task_exist_1.DOES_QUIT_TASK_EXIST.value) {
        throw new _1._.OngoingQuitTaskIsNotFinished();
    }
    if (update_state_1.UPDATE_STATE.value === undefined) {
        throw new _1._.UpdateDoesNotExist();
    }
    if (!update_state_1.UPDATE_STATE.value.isDownloaded) {
        throw new _1._.UpdateIsNotDownloaded();
    }
    does_apply_update_task_exist_1.DOES_APPLY_UPDATE_TASK_EXIST.value = true;
    does_quit_task_exist_1.DOES_QUIT_TASK_EXIST.value = true;
    _1.__.$electronupdater.autoUpdater.quitAndInstall(true, true);
}).startApplyUpdateTask;
//# sourceMappingURL=start-apply-update-task.js.map