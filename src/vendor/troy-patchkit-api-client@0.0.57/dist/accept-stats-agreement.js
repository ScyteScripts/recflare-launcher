"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.acceptStatsAgreement = void 0;
const _1 = require("./$");
const main_service_api_url_1 = require("./main-service-api-url");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.acceptStatsAgreement = _1.__.declareManagedFunc()
    .name(`acceptStatsAgreement`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ statsAgreementContextId, statsAgreementContextPreviousToken, }) => {
    let requestUrl = `${main_service_api_url_1.MAIN_SERVICE_API_URL.value}/1/statistics_contents/agree?source_id=${statsAgreementContextId}`;
    if (statsAgreementContextPreviousToken !== ``) {
        requestUrl += `&token=${statsAgreementContextPreviousToken}`;
    }
    const requestResponse = await _1.__.invokeAsyncExp(() => {
        try {
            return _1.__.$crossfetch.fetch(requestUrl, {
                method: `POST`,
            });
        }
        catch (e) {
            _1.__.writeTraceLog({
                kind: `on-patchkit-api-service-request-failed`,
                patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
            });
            throw new service_connection_is_not_available_1.ServiceConnectionIsNotAvailable();
        }
    });
    if (requestResponse.ok) {
        const requestResponseBodyAsJson = await _1.__.invokeAsyncExp(async () => {
            try {
                return await requestResponse.text();
            }
            catch (e) {
                _1.__.writeTraceLog({
                    kind: `on-patchkit-api-service-request-failed`,
                    patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
                });
                throw new service_connection_is_not_available_1.ServiceConnectionIsNotAvailable();
            }
        });
        const requestResponseBody = _1.__.deserializeValueFromJson({
            json: requestResponseBodyAsJson,
        });
        return {
            statsAgreementContextToken: requestResponseBody.token,
        };
    }
    else {
        // TODO: Handle 4xx and other responses.
        _1.__.checkInvalidAssertion();
    }
}).acceptStatsAgreement;
//# sourceMappingURL=accept-stats-agreement.js.map