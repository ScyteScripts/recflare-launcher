"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAgreementStateDef = void 0;
const _1 = require("./$");
const agreement_does_not_exist_1 = require("./agreement-does-not-exist");
exports.fetchAgreementStateDef = _1.__.defineManagedFunc()
    .name(`fetchAgreementState`)
    .namespace(_1.$namespace)
    .errorTypes([
    agreement_does_not_exist_1.AgreementDoesNotExist,
])
    .async()
    .executeType().fetchAgreementStateDef;
//# sourceMappingURL=fetch-agreement-state-def.js.map