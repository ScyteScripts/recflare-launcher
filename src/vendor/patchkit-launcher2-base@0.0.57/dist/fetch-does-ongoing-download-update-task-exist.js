"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesOngoingDownloadUpdateTaskExist = void 0;
const _1 = require("./$");
const ongoing_download_update_task_id_1 = require("./ongoing-download-update-task-id");
exports.fetchDoesOngoingDownloadUpdateTaskExist = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchDoesOngoingDownloadUpdateTaskExistDef)
    .execute(async () => {
    return ongoing_download_update_task_id_1.ONGOING_DOWNLOAD_UPDATE_TASK_ID.value !== undefined;
}).fetchDoesOngoingDownloadUpdateTaskExist;
exports.fetchDoesOngoingDownloadUpdateTaskExist.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-does-ongoing-download-update-task-exist.js.map