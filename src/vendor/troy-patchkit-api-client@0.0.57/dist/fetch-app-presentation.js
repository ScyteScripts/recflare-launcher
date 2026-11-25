"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppPresentation = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const fetch_main_service_resource_info_as_json_1 = require("./fetch-main-service-resource-info-as-json");
exports.fetchAppPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appSecret, }) => {
    const resourceInfoAsJson = await (0, fetch_main_service_resource_info_as_json_1.fetchMainServiceResourceInfoAsJson)({
        mainServiceResourcePath: `1/apps/${appSecret}`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    return {
        name: typeof resourceInfo.display_name !== `string`
            ? undefined
            : resourceInfo.display_name,
        iconUrl: typeof resourceInfo.icon_image !== `string`
            ? undefined
            : resourceInfo.icon_image,
        bannerUrl: typeof resourceInfo.patcher_banner_image !== `string`
            ? undefined
            : resourceInfo.patcher_banner_image,
        isWhitelabel: resourceInfo.patcher_whitelabel,
    };
}).fetchAppPresentation;
//# sourceMappingURL=fetch-app-presentation.js.map