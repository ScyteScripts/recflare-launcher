"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDownloadUpdateTaskStateDef = void 0;
const _1 = require("./$");
const download_update_task_does_not_exist_1 = require("./download-update-task-does-not-exist");
exports.fetchDownloadUpdateTaskStateDef = _1.__.defineManagedFunc()
    .name(`fetchDownloadUpdateTaskState`)
    .namespace(_1.$namespace)
    .errorTypes([
    download_update_task_does_not_exist_1.DownloadUpdateTaskDoesNotExist,
])
    .async()
    .executeType().fetchDownloadUpdateTaskStateDef;
//# sourceMappingURL=fetch-download-update-task-state-def.js.map