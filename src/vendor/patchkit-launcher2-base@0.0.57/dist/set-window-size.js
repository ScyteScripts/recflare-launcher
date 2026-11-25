"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.setWindowSize = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.setWindowSize = _1.__.declareManagedFunc()
    .async()
    .def(_1._.setWindowSizeDef)
    .execute(async ({ windowSize, }) => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.setSize(windowSize.width, windowSize.height);
}).setWindowSize;
//# sourceMappingURL=set-window-size.js.map