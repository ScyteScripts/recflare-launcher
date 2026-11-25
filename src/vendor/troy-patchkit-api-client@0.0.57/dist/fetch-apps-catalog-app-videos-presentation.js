"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppVideosPresentation = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppsCatalogAppVideosPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppVideosPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, }) => {
    const resourceInfoAsJson = await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
        appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/videos`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appsCatalogAppVideosPresentation = {};
    for (const resourceInfoVideo of resourceInfo.videos) {
        appsCatalogAppVideosPresentation[resourceInfoVideo.id] = {
            url: resourceInfoVideo.url,
            name: typeof resourceInfoVideo.display_name !== `string`
                ? undefined
                : resourceInfoVideo.display_name,
            description: typeof resourceInfoVideo.description !== `string`
                ? undefined
                : resourceInfoVideo.description,
            order: typeof resourceInfoVideo.order !== `number`
                ? Number.MAX_SAFE_INTEGER
                : resourceInfoVideo.order,
        };
    }
    return appsCatalogAppVideosPresentation;
}).fetchAppsCatalogAppVideosPresentation;
//# sourceMappingURL=fetch-apps-catalog-app-videos-presentation.js.map