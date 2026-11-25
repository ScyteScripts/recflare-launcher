"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.setWindowMaxSize = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.setWindowMaxSize = _1.__.declareManagedFunc()
    .async()
    .def(_1._.setWindowMaxSizeDef)
    .execute(async ({ windowMaxSize, }) => {
    var _a, _b;
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.setMaximumSize((_a = windowMaxSize === null || windowMaxSize === void 0 ? void 0 : windowMaxSize.width) !== null && _a !== void 0 ? _a : 2147483647, (_b = windowMaxSize === null || windowMaxSize === void 0 ? void 0 : windowMaxSize.height) !== null && _b !== void 0 ? _b : 2147483647);
}).setWindowMaxSize;
//# sourceMappingURL=set-window-max-size.js.map