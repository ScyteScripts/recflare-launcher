"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.setWindowMinSize = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.setWindowMinSize = _1.__.declareManagedFunc()
    .async()
    .def(_1._.setWindowMinSizeDef)
    .execute(async ({ windowMinSize, }) => {
    var _a, _b;
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.setMinimumSize((_a = windowMinSize === null || windowMinSize === void 0 ? void 0 : windowMinSize.width) !== null && _a !== void 0 ? _a : 0, (_b = windowMinSize === null || windowMinSize === void 0 ? void 0 : windowMinSize.height) !== null && _b !== void 0 ? _b : 0);
}).setWindowMinSize;
//# sourceMappingURL=set-window-min-size.js.map