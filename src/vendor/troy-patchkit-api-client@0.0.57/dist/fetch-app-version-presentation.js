"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppVersionPresentation = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const fetch_main_service_resource_info_as_json_1 = require("./fetch-main-service-resource-info-as-json");
exports.fetchAppVersionPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppVersionPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appSecret, appVersionId, }) => {
    const resourceInfoAsJson = await (0, fetch_main_service_resource_info_as_json_1.fetchMainServiceResourceInfoAsJson)({
        mainServiceResourcePath: `1/apps/${appSecret}/versions/${appVersionId}`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    return {
        label: resourceInfo.label,
        changelog: typeof resourceInfo.changelog !== `string`
            ? undefined
            : resourceInfo.changelog,
        publishedDate: typeof resourceInfo.publish_time === `string`
            ? new Date(resourceInfo.publish_time)
            : typeof resourceInfo.publish_time === `number`
                ? new Date(resourceInfo.publish_time * 1000)
                : undefined,
    };
}).fetchAppVersionPresentation;
//# sourceMappingURL=fetch-app-version-presentation.js.map