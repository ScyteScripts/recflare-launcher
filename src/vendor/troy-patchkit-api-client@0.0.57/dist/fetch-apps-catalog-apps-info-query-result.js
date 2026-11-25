"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppsInfoQueryResult = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppsCatalogAppsInfoQueryResult = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppsInfoQueryResult`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppsInfoQueryConfig, }) => {
    const resourceInfoAsJson = await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
        appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps?show_custom_fields=true`,
        appCatalogsServiceResourceAccessToken: appsCatalogAppsInfoQueryConfig.appsAccessToken,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appsCatalogAppsInfoQueryResult = {
        appsInfo: {},
    };
    for (const resourceApp of resourceInfo.apps) {
        const appsCatalogAppCustomFieldsInfo = {};
        for (const resourceAppCustomFieldValue of resourceApp.custom_fields_values) {
            appsCatalogAppCustomFieldsInfo[resourceAppCustomFieldValue.custom_field_type_name] = {
                value: resourceAppCustomFieldValue.image_url !== null
                    ? resourceAppCustomFieldValue.image_url
                    : resourceAppCustomFieldValue.value,
            };
        }
        appsCatalogAppsInfoQueryResult.appsInfo[resourceApp.id] = {
            name: typeof resourceApp.display_name !== `string`
                ? undefined
                : resourceApp.display_name,
            iconUrl: typeof resourceApp.icon_url !== `string`
                ? undefined
                : resourceApp.icon_url,
            bannerUrl: typeof resourceApp.banner_url !== `string`
                ? undefined
                : resourceApp.banner_url,
            shortDescription: typeof resourceApp.short_description !== `string`
                ? undefined
                : resourceApp.short_description,
            longDescription: typeof resourceApp.long_description !== `string`
                ? undefined
                : resourceApp.long_description,
            order: resourceApp.order,
            customFields: appsCatalogAppCustomFieldsInfo,
        };
    }
    return appsCatalogAppsInfoQueryResult;
}).fetchAppsCatalogAppsInfoQueryResult;
//# sourceMappingURL=fetch-apps-catalog-apps-info-query-result.js.map