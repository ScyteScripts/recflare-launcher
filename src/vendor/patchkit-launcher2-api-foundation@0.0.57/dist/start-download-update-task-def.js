"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startDownloadUpdateTaskDef = void 0;
const _1 = require("./$");
const update_does_not_exist_1 = require("./update-does-not-exist");
const ongoing_download_update_task_is_not_finished_1 = require("./ongoing-download-update-task-is-not-finished");
const update_is_already_downloaded_1 = require("./update-is-already-downloaded");
exports.startDownloadUpdateTaskDef = _1.__.defineManagedFunc()
    .name(`startDownloadUpdateTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    update_does_not_exist_1.UpdateDoesNotExist,
    update_is_already_downloaded_1.UpdateIsAlreadyDownloaded,
    ongoing_download_update_task_is_not_finished_1.OngoingDownloadUpdateTaskIsNotFinished,
])
    .async()
    .executeType().startDownloadUpdateTaskDef;
//# sourceMappingURL=start-download-update-task-def.js.map