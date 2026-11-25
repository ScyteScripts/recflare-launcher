"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchMainServiceResourceInfoAsJson = void 0;
const _1 = require("./$");
const policy = require("@recovered/policy");
const api_client_id_1 = require("./api-client-id");
const main_service_api_cache_url_1 = require("./main-service-api-cache-url");
const main_service_api_url_1 = require("./main-service-api-url");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const service_resource_does_not_exist_1 = require("./service-resource-does-not-exist");
exports.fetchMainServiceResourceInfoAsJson = _1.__.declareManagedFunc()
    .name(`fetchMainServiceResourceInfoAsJson`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_resource_does_not_exist_1.ServiceResourceDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ mainServiceResourcePath, }) => {
    const installed = policy.installedResource(mainServiceResourcePath);
    if (installed !== undefined) return installed;
    const getIsRequestValid = ({ requestResponse, }) => {
        return (requestResponse.ok
            ||
                (requestResponse.status >= 400 && requestResponse.status < 500));
    };
    const validRequestResponse = await _1.__.invokeAsyncExp(async () => {
        const requestsHeaders = {};
        if (api_client_id_1.API_CLIENT_ID.value !== undefined) {
            requestsHeaders[`PatchKitCaller`] = api_client_id_1.API_CLIENT_ID.value;
        }
        try {
            const requestResponse = await _1.__.$crossfetch.fetch(policy.resourceUrl(mainServiceResourcePath, main_service_api_url_1.MAIN_SERVICE_API_URL.value, "PatchKitApiUrl"), {
                headers: requestsHeaders,
            });
            if (getIsRequestValid({ requestResponse })) {
                return requestResponse;
            }
        }
        catch (e) {
            _1.__.writeTraceLog({
                kind: `on-patchkit-api-service-request-failed`,
                patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
            });
        }
        try {
            const requestResponse = await _1.__.$crossfetch.fetch(policy.resourceUrl(mainServiceResourcePath, main_service_api_cache_url_1.MAIN_SERVICE_API_CACHE_URL.value, "PatchKitCacheUrl"), {
                headers: requestsHeaders,
            });
            if (getIsRequestValid({ requestResponse })) {
                return requestResponse;
            }
        }
        catch (e) {
            _1.__.writeTraceLog({
                kind: `on-patchkit-api-service-request-failed`,
                patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
            });
        }
        throw new service_connection_is_not_available_1.ServiceConnectionIsNotAvailable();
    });
    if (validRequestResponse.ok) {
        try {
            return await validRequestResponse.text();
        }
        catch (e) {
            _1.__.writeTraceLog({
                kind: `on-patchkit-api-service-request-failed`,
                patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
            });
            throw new service_connection_is_not_available_1.ServiceConnectionIsNotAvailable();
        }
    }
    else if (validRequestResponse.status === 404) {
        throw new service_resource_does_not_exist_1.ServiceResourceDoesNotExist();
    }
    else {
        // TODO: Handle 4xx and other responses.
        _1.__.checkInvalidAssertion();
    }
}).fetchMainServiceResourceInfoAsJson;
exports.fetchMainServiceResourceInfoAsJson.config.isLoggingEnabled = false;
exports.fetchMainServiceResourceInfoAsJson.config.isLoggingExecResultEnabled = false;
//# sourceMappingURL=fetch-main-service-resource-info-as-json.js.map