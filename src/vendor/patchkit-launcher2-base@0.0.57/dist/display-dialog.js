"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.displayDialog = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.displayDialog = _1.__.declareManagedFunc()
    .async()
    .def(_1._.displayDialogDef)
    .execute(async ({ dialogConfig, dialogOptionsInfo, dialogDefaultOptionId, }) => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    const { response: dialogSelectedOptionIndex, } = await _1.__.$electron.dialog.showMessageBox(window_handle_1.WINDOW_HANDLE.value, {
        buttons: Object.entries(dialogOptionsInfo).map(([_dialogOptionId, dialogOptionInfo]) => dialogOptionInfo.label),
        title: dialogConfig.title,
        message: dialogConfig.message,
        type: _1.__.invokeSyncExp(() => {
            switch (dialogConfig.type) {
                case _1._.DialogType.DefaultDialog:
                    return `none`;
                case _1._.DialogType.InfoDialog:
                    return `info`;
                case _1._.DialogType.ErrorDialog:
                    return `error`;
                case _1._.DialogType.QuestionDialog:
                    return `question`;
                case _1._.DialogType.WarningDialog:
                    return `warning`;
            }
        }),
        defaultId: dialogDefaultOptionId !== undefined
            ? Object.keys(dialogOptionsInfo).indexOf(dialogDefaultOptionId)
            : undefined,
    });
    const dialogSelectedOptionId = Object.keys(dialogOptionsInfo)[dialogSelectedOptionIndex];
    return {
        dialogSelectedOptionId,
    };
}).displayDialog;
//# sourceMappingURL=display-dialog.js.map