"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.onDebugShortcutInvoked = void 0;
const _1 = require("./$");
const display_dialog_1 = require("./display-dialog");
const focus_file_system_entry_in_file_explorer_1 = require("./focus-file-system-entry-in-file-explorer");
const get_logs_dir_path_1 = require("./get-logs-dir-path");
const get_user_data_dir_path_1 = require("./get-user-data-dir-path");
exports.onDebugShortcutInvoked = _1.__.declareManagedFunc()
    .name(`onDebugShortcutInvoked`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const logsDirPath = (0, get_logs_dir_path_1.getLogsDirPath)();
    if (!_1.__.$fs.existsSync(logsDirPath)) {
        return;
    }
    const logsZipFileName = `LAUNCHER_LOGS_${new Date().toISOString().split(`:`).join(`-`)}.zip`;
    const logsZipFilePath = _1.__.$path.resolve((0, get_user_data_dir_path_1.getUserDataDirPath)(), logsZipFileName);
    await _1.__.$zipafolder.zip(logsDirPath, logsZipFilePath);
    await (0, display_dialog_1.displayDialog)({
        dialogConfig: {
            title: `Logs have been collected!`,
            message: `Please send ${logsZipFileName} file to the support. Thanks for reporting the issue!`,
            type: _1._.DialogType.InfoDialog,
        },
        dialogOptionsInfo: {
            ok: {
                label: `OK`,
            },
        },
        dialogDefaultOptionId: `ok`,
    });
    await (0, focus_file_system_entry_in_file_explorer_1.focusFileSystemEntryInFileExplorer)({
        fileSystemEntryPath: logsZipFilePath,
    });
}).onDebugShortcutInvoked;
//# sourceMappingURL=on-debug-shortcut-invoked.js.map