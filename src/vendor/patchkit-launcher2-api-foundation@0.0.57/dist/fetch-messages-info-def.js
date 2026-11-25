"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchMessagesInfoDef = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchMessagesInfoDef = _1.__.defineManagedFunc()
    .name(`fetchMessagesInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchMessagesInfoDef;
//# sourceMappingURL=fetch-messages-info-def.js.map