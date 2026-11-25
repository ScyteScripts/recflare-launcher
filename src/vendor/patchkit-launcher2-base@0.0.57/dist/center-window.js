"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.centerWindow = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.centerWindow = _1.__.declareManagedFunc()
    .async()
    .def(_1._.centerWindowDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.center();
}).centerWindow;
//# sourceMappingURL=center-window.js.map