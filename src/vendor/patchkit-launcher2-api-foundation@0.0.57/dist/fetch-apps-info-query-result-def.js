"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsInfoQueryResultDef = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppsInfoQueryResultDef = _1.__.defineManagedFunc()
    .name(`fetchAppsInfoQueryResult`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchAppsInfoQueryResultDef;
//# sourceMappingURL=fetch-apps-info-query-result-def.js.map