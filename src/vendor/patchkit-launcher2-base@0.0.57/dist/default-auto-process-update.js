"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.defaultAutoProcessUpdate = void 0;
const _1 = require("./$");
const display_dialog_1 = require("./display-dialog");
const fetch_download_update_task_state_1 = require("./fetch-download-update-task-state");
const start_apply_update_task_1 = require("./start-apply-update-task");
const start_download_update_task_1 = require("./start-download-update-task");
const update_state_1 = require("./update-state");
exports.defaultAutoProcessUpdate = _1.__.declareManagedFunc()
    .name(`defaultAutoProcessUpdate`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.UpdateDoesNotExist,
])
    .async()
    .execute(async () => {
    if (update_state_1.UPDATE_STATE.value === undefined) {
        throw new _1._.UpdateDoesNotExist();
    }
    const { downloadUpdateTaskId, } = await (0, start_download_update_task_1.startDownloadUpdateTask)();
    let downloadUpdateTaskState;
    do {
        downloadUpdateTaskState = await (0, fetch_download_update_task_state_1.fetchDownloadUpdateTaskState)({
            downloadUpdateTaskId,
        });
        await _1.__.wait(1000);
    } while (!downloadUpdateTaskState.isFinished);
    if (downloadUpdateTaskState.errorTypeName !== undefined) {
        return;
    }
    const { dialogSelectedOptionId, } = await (0, display_dialog_1.displayDialog)({
        dialogConfig: {
            title: `New Launcher Update Available`,
            message: `A new update for the launcher is now available! Would you like to install it now? Click 'Yes' to begin the update process, or 'No' to postpone the installation until later.`,
            type: _1._.DialogType.DefaultDialog,
        },
        dialogOptionsInfo: {
            yes: {
                label: `Yes`,
            },
            no: {
                label: `No`,
            },
        },
        dialogDefaultOptionId: `yes`,
    });
    if (dialogSelectedOptionId === `yes`) {
        await (0, start_apply_update_task_1.startApplyUpdateTask)();
    }
}).defaultAutoProcessUpdate;
//# sourceMappingURL=default-auto-process-update.js.map