"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppVersionsPresentation = void 0;
const _1 = require("./$");
const fetch_main_service_resource_info_as_json_1 = require("./fetch-main-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppVersionsPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppVersionsPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appSecret, }) => {
    const resourceInfoAsJson = await (0, fetch_main_service_resource_info_as_json_1.fetchMainServiceResourceInfoAsJson)({
        mainServiceResourcePath: `1/apps/${appSecret}/changelog`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appVersionsPresentation = {};
    for (const resourceVersion of resourceInfo.versions) {
        appVersionsPresentation[resourceVersion.vid] = {
            label: resourceVersion.label,
            changelog: typeof resourceVersion.changelog !== `string`
                ? undefined
                : resourceVersion.changelog,
            publishedDate: typeof resourceVersion.publish_date === `string`
                ? new Date(resourceVersion.publish_date)
                : typeof resourceVersion.publish_time === `number`
                    ? new Date(resourceVersion.publish_time * 1000)
                    : undefined,
        };
    }
    return appVersionsPresentation;
}).fetchAppVersionsPresentation;
//# sourceMappingURL=fetch-app-versions-presentation.js.map