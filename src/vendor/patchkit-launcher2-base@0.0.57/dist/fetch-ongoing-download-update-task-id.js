"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchOngoingDownloadUpdateTaskId = void 0;
const _1 = require("./$");
const ongoing_download_update_task_id_1 = require("./ongoing-download-update-task-id");
exports.fetchOngoingDownloadUpdateTaskId = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchOngoingDownloadUpdateTaskIdDef)
    .execute(async () => {
    if (ongoing_download_update_task_id_1.ONGOING_DOWNLOAD_UPDATE_TASK_ID.value === undefined) {
        throw new _1._.OngoingDownloadUpdateTaskDoesNotExist();
    }
    return ongoing_download_update_task_id_1.ONGOING_DOWNLOAD_UPDATE_TASK_ID.value;
}).fetchOngoingDownloadUpdateTaskId;
exports.fetchOngoingDownloadUpdateTaskId.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-ongoing-download-update-task-id.js.map