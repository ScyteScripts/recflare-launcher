"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchTranslationsInfoDef = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchTranslationsInfoDef = _1.__.defineManagedFunc()
    .name(`fetchTranslationsInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchTranslationsInfoDef;
//# sourceMappingURL=fetch-translations-info-def.js.map