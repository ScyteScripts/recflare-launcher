"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.initializeProtocolClient = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const focus_window_1 = require("./focus-window");
const last_protocol_request_id_as_number_1 = require("./last-protocol-request-id-as-number");
const protocol_pending_requests_info_box_1 = require("./protocol-pending-requests-info-box");
const show_window_1 = require("./show-window");
exports.initializeProtocolClient = _1.__.declareManagedFunc()
    .name(`initializeProtocolClient`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    if (config_1.CONFIG.value.protocol === undefined) {
        return;
    }
    _1.__.$electronappuniversalprotocolclient.default.on(`request`, async (electronAppUniversalProtocolClientRequestUrl) => {
        await (0, show_window_1.showWindow)();
        await (0, focus_window_1.focusWindow)();
        const protocolPendingRequestIdAsNumber = last_protocol_request_id_as_number_1.LAST_PROTOCOL_REQUEST_ID_AS_NUMBER.value === undefined
            ? 0
            : last_protocol_request_id_as_number_1.LAST_PROTOCOL_REQUEST_ID_AS_NUMBER.value + 1;
        last_protocol_request_id_as_number_1.LAST_PROTOCOL_REQUEST_ID_AS_NUMBER.value = protocolPendingRequestIdAsNumber;
        const protocolPendingRequestId = String(protocolPendingRequestIdAsNumber);
        const protocolPendingRequestInfoBox = _1.__.invokeSyncExp(() => {
            if (protocol_pending_requests_info_box_1.PROTOCOL_PENDING_REQUESTS_INFO_BOX.value[protocolPendingRequestId] === undefined) {
                protocol_pending_requests_info_box_1.PROTOCOL_PENDING_REQUESTS_INFO_BOX.value[protocolPendingRequestId] = {
                    value: undefined,
                };
            }
            return protocol_pending_requests_info_box_1.PROTOCOL_PENDING_REQUESTS_INFO_BOX.value[protocolPendingRequestId];
        });
        protocolPendingRequestInfoBox.value = {
            url: electronAppUniversalProtocolClientRequestUrl,
        };
    });
    await _1.__.$electronappuniversalprotocolclient.default.initialize({
        protocol: config_1.CONFIG.value.protocol.id,
        mode: config_1.CONFIG.value.mode === _1._.Mode.Dev
            ? `development`
            : `production`,
    });
}).initializeProtocolClient;
//# sourceMappingURL=initialize-protocol-client.js.map