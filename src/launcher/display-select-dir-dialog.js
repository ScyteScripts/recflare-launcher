"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.displaySelectDirDialog = void 0;
const _1 = require("./$");
exports.displaySelectDirDialog = _1.__.declareManagedFunc()
    .async()
    .def(_1._.displaySelectDirDialogDef)
    .execute(async ({ selectDirDialogConfig, }) => {
    _1.__.checkAssertion(_1.__.PatchKitLauncher2.WINDOW_HANDLE.value !== undefined);
    const electronDialogResult = await _1.__.$electron.dialog.showOpenDialog(_1.__.PatchKitLauncher2.WINDOW_HANDLE.value, {
        properties: [
            `openDirectory`,
            `createDirectory`,
        ],
        title: selectDirDialogConfig.title,
        message: selectDirDialogConfig.message,
    });
    if (electronDialogResult.canceled || electronDialogResult.filePaths.length === 0) {
        return {
            selectDirDialogResult: {},
        };
    }
    const electronDialogResultDirPath = electronDialogResult.filePaths[0];
    return {
        selectDirDialogResult: {
            dirPath: electronDialogResultDirPath,
        },
    };
}).displaySelectDirDialog;
//# sourceMappingURL=display-select-dir-dialog.js.map