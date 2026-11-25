"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.initializeElectronUpdater = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const default_auto_process_update_1 = require("./default-auto-process-update");
const does_check_update_task_exist_1 = require("./does-check-update-task-exist");
const download_update_tasks_state_box_1 = require("./download-update-tasks-state-box");
const ongoing_download_update_task_id_1 = require("./ongoing-download-update-task-id");
const update_state_1 = require("./update-state");
exports.initializeElectronUpdater = _1.__.declareManagedFunc()
    .name(`initializeElectronUpdater`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    // Development builds are updated by their operator. Template updates remain enabled.
    does_check_update_task_exist_1.DOES_CHECK_UPDATE_TASK_EXIST.value = false;
}).initializeElectronUpdater;
