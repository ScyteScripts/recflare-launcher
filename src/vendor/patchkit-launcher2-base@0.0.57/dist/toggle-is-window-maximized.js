"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.toggleIsWindowMaximized = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.toggleIsWindowMaximized = _1.__.declareManagedFunc()
    .async()
    .def(_1._.toggleIsWindowMaximizedDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    if (window_handle_1.WINDOW_HANDLE.value.isMaximized()) {
        window_handle_1.WINDOW_HANDLE.value.unmaximize();
    }
    else {
        window_handle_1.WINDOW_HANDLE.value.maximize();
    }
}).toggleIsWindowMaximized;
//# sourceMappingURL=toggle-is-window-maximized.js.map