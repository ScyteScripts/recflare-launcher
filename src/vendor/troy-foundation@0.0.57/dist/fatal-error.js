"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.FatalError = void 0;
const _1 = require("./$");
const managed_error_1 = require("./managed-error");
exports.FatalError = (0, managed_error_1.declareManagedErrorType)()
    .name(`FatalError`)
    .namespace(_1.$namespace).FatalError;
//# sourceMappingURL=fatal-error.js.map