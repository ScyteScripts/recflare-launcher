"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.onProtocolPendingRequestHandled = void 0;
const _1 = require("./$");
const protocol_pending_requests_info_box_1 = require("./protocol-pending-requests-info-box");
exports.onProtocolPendingRequestHandled = _1.__.declareManagedFunc()
    .async()
    .def(_1._.onProtocolPendingRequestHandledDef)
    .execute(async ({ protocolPendingRequestId, }) => {
    const protocolPendingRequestInfoBox = protocol_pending_requests_info_box_1.PROTOCOL_PENDING_REQUESTS_INFO_BOX.value[protocolPendingRequestId];
    if ((protocolPendingRequestInfoBox === null || protocolPendingRequestInfoBox === void 0 ? void 0 : protocolPendingRequestInfoBox.value) === undefined) {
        throw new _1._.ProtocolPendingRequestDoesNotExist();
    }
    protocolPendingRequestInfoBox.value = undefined;
}).onProtocolPendingRequestHandled;
exports.onProtocolPendingRequestHandled.config.isLoggingEnabled = false;
//# sourceMappingURL=on-protocol-pending-request-handled.js.map