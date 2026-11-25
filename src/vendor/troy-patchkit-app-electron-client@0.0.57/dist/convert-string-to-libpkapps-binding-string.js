"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.convertStringToLibpkappsBindingString = void 0;
const _1 = require("./$");
exports.convertStringToLibpkappsBindingString = _1.__.declareManagedFunc()
    .name(`convertStringToLibpkappsBindingString`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ string, }) => {
    return string; // JSON-RPC transports values directly; no NUL-terminated ABI buffer.
}).convertStringToLibpkappsBindingString;
exports.convertStringToLibpkappsBindingString.config.isLoggingEnabled = false;
//# sourceMappingURL=convert-string-to-libpkapps-binding-string.js.map
