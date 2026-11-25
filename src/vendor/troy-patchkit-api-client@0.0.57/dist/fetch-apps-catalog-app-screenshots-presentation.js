"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppScreenshotsPresentation = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppsCatalogAppScreenshotsPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppScreenshotsPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, }) => {
    const resourceInfoAsJson = await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
        appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/screenshots`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appsCatalogAppScreenshotsPresentation = {};
    for (const resourceScreenshot of resourceInfo.screenshots) {
        appsCatalogAppScreenshotsPresentation[resourceScreenshot.id] = {
            url: resourceScreenshot.image.url,
            name: typeof resourceScreenshot.display_name !== `string`
                ? undefined
                : resourceScreenshot.display_name,
            description: typeof resourceScreenshot.description !== `string`
                ? undefined
                : resourceScreenshot.description,
            order: typeof resourceScreenshot.order !== `number`
                ? Number.MAX_SAFE_INTEGER
                : resourceScreenshot.order,
        };
    }
    return appsCatalogAppScreenshotsPresentation;
}).fetchAppsCatalogAppScreenshotsPresentation;
//# sourceMappingURL=fetch-apps-catalog-app-screenshots-presentation.js.map