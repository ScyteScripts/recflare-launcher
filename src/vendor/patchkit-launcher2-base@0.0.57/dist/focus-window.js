"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.focusWindow = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.focusWindow = _1.__.declareManagedFunc()
    .async()
    .def(_1._.focusWindowDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.setAlwaysOnTop(true);
    window_handle_1.WINDOW_HANDLE.value.focus();
    window_handle_1.WINDOW_HANDLE.value.setAlwaysOnTop(false);
}).focusWindow;
//# sourceMappingURL=focus-window.js.map