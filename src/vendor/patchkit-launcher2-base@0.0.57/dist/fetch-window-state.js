"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchWindowState = void 0;
const _1 = require("./$");
const window_handle_1 = require("./window-handle");
exports.fetchWindowState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchWindowStateDef)
    .execute(async () => {
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    const [windowWidth, windowHeight] = window_handle_1.WINDOW_HANDLE.value.getSize();
    const [windowMinWidth, windowMinHeight] = window_handle_1.WINDOW_HANDLE.value.getMinimumSize();
    const [windowMaxWidth, windowMaxHeight] = window_handle_1.WINDOW_HANDLE.value.getMaximumSize();
    _1.__.checkAssertion(windowWidth !== undefined);
    _1.__.checkAssertion(windowHeight !== undefined);
    _1.__.checkAssertion(windowMinWidth !== undefined);
    _1.__.checkAssertion(windowMinHeight !== undefined);
    _1.__.checkAssertion(windowMaxWidth !== undefined);
    _1.__.checkAssertion(windowMaxHeight !== undefined);
    return {
        isVisible: window_handle_1.WINDOW_HANDLE.value.isVisible(),
        isMaximized: window_handle_1.WINDOW_HANDLE.value.isMaximized(),
        isMinimized: window_handle_1.WINDOW_HANDLE.value.isMinimized(),
        size: {
            width: windowWidth,
            height: windowHeight,
        },
        minSize: {
            width: windowMinWidth,
            height: windowMinHeight,
        },
        maxSize: {
            width: windowMaxWidth,
            height: windowMaxHeight,
        },
    };
}).fetchWindowState;
exports.fetchWindowState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-window-state.js.map