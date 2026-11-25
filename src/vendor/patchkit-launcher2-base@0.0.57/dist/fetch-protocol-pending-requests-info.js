"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchProtocolPendingRequestsInfo = void 0;
const _1 = require("./$");
const protocol_pending_requests_info_box_1 = require("./protocol-pending-requests-info-box");
exports.fetchProtocolPendingRequestsInfo = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchProtocolPendingRequestsInfoDef)
    .execute(async () => {
    const protocolPendingRequestsInfo = {};
    for (const [protocolPendingRequestId, protocolPendingRequestInfoBox] of Object.entries(protocol_pending_requests_info_box_1.PROTOCOL_PENDING_REQUESTS_INFO_BOX.value)) {
        const protocolPendingRequestInfo = protocolPendingRequestInfoBox.value;
        if (protocolPendingRequestInfo !== undefined) {
            protocolPendingRequestsInfo[protocolPendingRequestId] =
                protocolPendingRequestInfo;
        }
    }
    return protocolPendingRequestsInfo;
}).fetchProtocolPendingRequestsInfo;
exports.fetchProtocolPendingRequestsInfo.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-protocol-pending-requests-info.js.map