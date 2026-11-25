"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.hideWindowDef = void 0;
const _1 = require("./$");
const hiding_window_is_not_allowed_1 = require("./hiding-window-is-not-allowed");
exports.hideWindowDef = _1.__.defineManagedFunc()
    .name(`hideWindow`)
    .namespace(_1.$namespace)
    .errorTypes([
    hiding_window_is_not_allowed_1.HidingWindowIsNotAllowed,
])
    .async()
    .executeType().hideWindowDef;
//# sourceMappingURL=hide-window-def.js.map