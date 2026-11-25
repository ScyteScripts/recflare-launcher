"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.convertPathToLibpkappsBindingPath = void 0;
const _1 = require("./$");
const convert_string_to_libpkapps_binding_string_1 = require("./convert-string-to-libpkapps-binding-string");
exports.convertPathToLibpkappsBindingPath = _1.__.declareManagedFunc()
    .name(`convertPathToLibpkappsBindingPath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ path, }) => {
    return (0, convert_string_to_libpkapps_binding_string_1.convertStringToLibpkappsBindingString)({
        string: _1.__.$path.normalize(path).replace(RegExp(`\\\\`, `g`), `/`),
    });
}).convertPathToLibpkappsBindingPath;
exports.convertPathToLibpkappsBindingPath.config.isLoggingEnabled = false;
//# sourceMappingURL=convert-path-to-libpkapps-binding-path.js.map