"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchLauncherThemeLatestCompatibleVersionUrl = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const launcher_themes_service_api_url_1 = require("./launcher-themes-service-api-url");
exports.fetchLauncherThemeLatestCompatibleVersionUrl = _1.__.declareManagedFunc()
    .name(`fetchLauncherThemeLatestCompatibleVersionUrl`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.LauncherThemeDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ launcherVersion, launcherThemeId, }) => {
    const requestResponse = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.$crossfetch.fetch(`${launcher_themes_service_api_url_1.LAUNCHER_THEMES_SERVICE_API_URL.value}/v1/themes/${launcherThemeId}/versions/latest?launcherVersion=${launcherVersion}`);
        }
        catch (e) {
            _1.__.writeTraceLog({
                kind: `on-patchkit-api-service-request-failed`,
                patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
            });
            throw new service_connection_is_not_available_1.ServiceConnectionIsNotAvailable();
        }
    });
    const resourceInfoAsJson = await _1.__.invokeAsyncExp(async () => {
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
            throw new _1._.LauncherThemeDoesNotExist();
        }
        else {
            // TODO: Handle 4xx and other responses.
            _1.__.checkInvalidAssertion();
        }
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    return resourceInfo.filesUrl;
}).fetchLauncherThemeLatestCompatibleVersionUrl;
//# sourceMappingURL=fetch-launcher-theme-latest-compatible-version-url.js.map