"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.onProtocolPendingRequestHandledDef = void 0;
const _1 = require("./$");
const protocol_pending_request_does_not_exist_1 = require("./protocol-pending-request-does-not-exist");
exports.onProtocolPendingRequestHandledDef = _1.__.defineManagedFunc()
    .name(`onProtocolPendingRequestHandled`)
    .namespace(_1.$namespace)
    .errorTypes([
    protocol_pending_request_does_not_exist_1.ProtocolPendingRequestDoesNotExist,
])
    .async()
    .executeType().onProtocolPendingRequestHandledDef;
//# sourceMappingURL=on-protocol-pending-request-handled-def.js.map