"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppCatalogsServiceResourceInfoAsJson = void 0;
const _1 = require("./$");
const api_client_id_1 = require("./api-client-id");
const app_catalogs_service_api_url_1 = require("./app-catalogs-service-api-url");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const service_resource_does_not_exist_1 = require("./service-resource-does-not-exist");
exports.fetchAppCatalogsServiceResourceInfoAsJson = _1.__.declareManagedFunc()
    .name(`fetchAppCatalogsServiceResourceInfoAsJson`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_resource_does_not_exist_1.ServiceResourceDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appCatalogsServiceResourcePath, appCatalogsServiceResourceAccessToken, }) => {
    const requestHeaders = {};
    if (api_client_id_1.API_CLIENT_ID.value !== undefined) {
        requestHeaders[`PatchKitCaller`] = api_client_id_1.API_CLIENT_ID.value;
    }
    if (appCatalogsServiceResourceAccessToken !== undefined) {
        requestHeaders[`Authorization`] =
            `Bearer ${appCatalogsServiceResourceAccessToken}`;
    }
    const requestResponse = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.$crossfetch.fetch(`${app_catalogs_service_api_url_1.APP_CATALOGS_SERVICE_API_URL.value}/${appCatalogsServiceResourcePath}`, {
                headers: requestHeaders,
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
    }
    else if (requestResponse.status === 404) {
        throw new service_resource_does_not_exist_1.ServiceResourceDoesNotExist();
    }
    else {
        // TODO: Handle 4xx and other responses.
        _1.__.checkInvalidAssertion();
    }
}).fetchAppCatalogsServiceResourceInfoAsJson;
exports.fetchAppCatalogsServiceResourceInfoAsJson.config.isLoggingEnabled = false;
exports.fetchAppCatalogsServiceResourceInfoAsJson.config.isLoggingExecResultEnabled = false;
//# sourceMappingURL=fetch-app-catalogs-service-resource-info-as-json.js.map