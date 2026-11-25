"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.serializeValueToJson = void 0;
const _1 = require("./$");
exports.serializeValueToJson = _1.__.declareManagedFunc()
    .name(`serializeValueToJson`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ value, }) => {
    return JSON.stringify(value);
}).serializeValueToJson;
exports.serializeValueToJson.config.isLoggingEnabled = false;
//# sourceMappingURL=serialize-value-to-json.js.map