"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.hideWindow = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.hideWindow = _1.__.declareManagedFunc()
    .async()
    .def(_1._.hideWindowDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    window_handle_1.WINDOW_HANDLE.value.hide();
}).hideWindow;
//# sourceMappingURL=hide-window.js.map