"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.convertStringOrUndefinedToLibpkappsBindingNullableString = void 0;
const _1 = require("./$");
const convert_string_to_libpkapps_binding_string_1 = require("./convert-string-to-libpkapps-binding-string");
exports.convertStringOrUndefinedToLibpkappsBindingNullableString = _1.__.declareManagedFunc()
    .name(`convertStringOrUndefinedToLibpkappsBindingNullableString`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ string, }) => {
    return string === undefined
        ? null
        : (0, convert_string_to_libpkapps_binding_string_1.convertStringToLibpkappsBindingString)({ string });
}).convertStringOrUndefinedToLibpkappsBindingNullableString;
exports.convertStringOrUndefinedToLibpkappsBindingNullableString.config.isLoggingEnabled = false;
//# sourceMappingURL=convert-string-or-undefined-to-libpkapps-binding-nullable-string.js.map