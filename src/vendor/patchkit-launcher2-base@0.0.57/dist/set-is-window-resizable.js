"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.setIsWindowResizable = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.setIsWindowResizable = _1.__.declareManagedFunc()
    .async()
    .def(_1._.setIsWindowResizableDef)
    .execute(async ({ isWindowResizable, }) => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.setResizable(isWindowResizable);
}).setIsWindowResizable;
//# sourceMappingURL=set-is-window-resizable.js.map