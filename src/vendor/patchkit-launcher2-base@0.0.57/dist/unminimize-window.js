"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.unminimizeWindow = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.unminimizeWindow = _1.__.declareManagedFunc()
    .async()
    .def(_1._.unminimizeWindowDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.restore();
}).unminimizeWindow;
//# sourceMappingURL=unminimize-window.js.map