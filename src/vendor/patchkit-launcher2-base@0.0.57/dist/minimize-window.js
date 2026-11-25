"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.minimizeWindow = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.minimizeWindow = _1.__.declareManagedFunc()
    .async()
    .def(_1._.minimizeWindowDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.minimize();
}).minimizeWindow;
//# sourceMappingURL=minimize-window.js.map