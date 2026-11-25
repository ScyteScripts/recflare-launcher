"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDownloadUpdateTaskState = void 0;
const _1 = require("./$");
const download_update_tasks_state_box_1 = require("./download-update-tasks-state-box");
exports.fetchDownloadUpdateTaskState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchDownloadUpdateTaskStateDef)
    .execute(async ({ downloadUpdateTaskId, }) => {
    const downloadUpdateTaskStateBox = download_update_tasks_state_box_1.DOWNLOAD_UPDATE_TASKS_STATE_BOX.value[downloadUpdateTaskId];
    if ((downloadUpdateTaskStateBox === null || downloadUpdateTaskStateBox === void 0 ? void 0 : downloadUpdateTaskStateBox.value) === undefined) {
        throw new _1._.DownloadUpdateTaskDoesNotExist();
    }
    return downloadUpdateTaskStateBox.value;
}).fetchDownloadUpdateTaskState;
exports.fetchDownloadUpdateTaskState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-download-update-task-state.js.map