"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppLatestVersionId = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const fetch_main_service_resource_info_as_json_1 = require("./fetch-main-service-resource-info-as-json");
exports.fetchAppLatestVersionId = _1.__.declareManagedFunc()
    .name(`fetchAppLatestVersionId`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appSecret, }) => {
    const resourceInfoAsJson = await (0, fetch_main_service_resource_info_as_json_1.fetchMainServiceResourceInfoAsJson)({
        mainServiceResourcePath: `1/apps/${appSecret}/versions/latest/id`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    return resourceInfo.id;
}).fetchAppLatestVersionId;
//# sourceMappingURL=fetch-app-latest-version-id.js.map