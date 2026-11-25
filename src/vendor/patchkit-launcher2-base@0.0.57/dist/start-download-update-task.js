"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startDownloadUpdateTask = void 0;
const _1 = require("./$");
const download_update_tasks_state_box_1 = require("./download-update-tasks-state-box");
const last_download_update_task_id_1 = require("./last-download-update-task-id");
const ongoing_download_update_task_id_1 = require("./ongoing-download-update-task-id");
const update_state_1 = require("./update-state");
exports.startDownloadUpdateTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startDownloadUpdateTaskDef)
    .execute(async () => {
    if (update_state_1.UPDATE_STATE.value === undefined) {
        throw new _1._.UpdateDoesNotExist();
    }
    if (update_state_1.UPDATE_STATE.value.isDownloaded) {
        throw new _1._.UpdateIsAlreadyDownloaded();
    }
    if (ongoing_download_update_task_id_1.ONGOING_DOWNLOAD_UPDATE_TASK_ID.value !== undefined) {
        throw new _1._.OngoingDownloadUpdateTaskIsNotFinished();
    }
    const downloadUpdateTaskId = last_download_update_task_id_1.LAST_DOWNLOAD_UPDATE_TASK_ID.value === undefined
        ? 0
        : last_download_update_task_id_1.LAST_DOWNLOAD_UPDATE_TASK_ID.value + 1;
    last_download_update_task_id_1.LAST_DOWNLOAD_UPDATE_TASK_ID.value = downloadUpdateTaskId;
    ongoing_download_update_task_id_1.ONGOING_DOWNLOAD_UPDATE_TASK_ID.value = downloadUpdateTaskId;
    const downloadUpdateTaskStateBox = _1.__.invokeSyncExp(() => {
        if (download_update_tasks_state_box_1.DOWNLOAD_UPDATE_TASKS_STATE_BOX.value[downloadUpdateTaskId] === undefined) {
            download_update_tasks_state_box_1.DOWNLOAD_UPDATE_TASKS_STATE_BOX.value[downloadUpdateTaskId] = {
                value: undefined,
            };
        }
        return download_update_tasks_state_box_1.DOWNLOAD_UPDATE_TASKS_STATE_BOX.value[downloadUpdateTaskId];
    });
    downloadUpdateTaskStateBox.value = {
        isFinished: false,
    };
    _1.__.$electronupdater.autoUpdater.downloadUpdate().catch();
    return {
        downloadUpdateTaskId,
    };
}).startDownloadUpdateTask;
//# sourceMappingURL=start-download-update-task.js.map