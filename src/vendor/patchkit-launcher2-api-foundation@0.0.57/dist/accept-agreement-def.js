"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.acceptAgreementDef = void 0;
const _1 = require("./$");
const agreement_does_not_exist_1 = require("./agreement-does-not-exist");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.acceptAgreementDef = _1.__.defineManagedFunc()
    .name(`acceptAgreement`)
    .namespace(_1.$namespace)
    .errorTypes([
    agreement_does_not_exist_1.AgreementDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().acceptAgreementDef;
//# sourceMappingURL=accept-agreement-def.js.map