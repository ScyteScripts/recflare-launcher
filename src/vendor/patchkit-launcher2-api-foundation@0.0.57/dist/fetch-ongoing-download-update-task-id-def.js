"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchOngoingDownloadUpdateTaskIdDef = void 0;
const _1 = require("./$");
const ongoing_download_update_task_does_not_exist_1 = require("./ongoing-download-update-task-does-not-exist");
exports.fetchOngoingDownloadUpdateTaskIdDef = _1.__.defineManagedFunc()
    .name(`fetchOngoingDownloadUpdateTaskId`)
    .namespace(_1.$namespace)
    .errorTypes([
    ongoing_download_update_task_does_not_exist_1.OngoingDownloadUpdateTaskDoesNotExist,
])
    .async()
    .executeType().fetchOngoingDownloadUpdateTaskIdDef;
//# sourceMappingURL=fetch-ongoing-download-update-task-id-def.js.map