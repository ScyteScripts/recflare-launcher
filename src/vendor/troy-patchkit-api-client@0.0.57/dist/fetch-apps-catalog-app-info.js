"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppInfo = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const service_resource_does_not_exist_1 = require("./service-resource-does-not-exist");
exports.fetchAppsCatalogAppInfo = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppsCatalogAppDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, appsCatalogAppAccessToken, }) => {
    const resourceInfoAsJson = await _1.__.invokeAsyncExp(async () => {
        try {
            return await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
                appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}?show_custom_fields=true`,
                appCatalogsServiceResourceAccessToken: appsCatalogAppAccessToken,
            });
        }
        catch (e) {
            if (e instanceof service_resource_does_not_exist_1.ServiceResourceDoesNotExist) {
                throw new _1._.AppsCatalogAppDoesNotExist();
            }
            throw e;
        }
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appsCatalogAppCustomFieldsInfo = {};
    for (const resourceAppCustomFieldValue of resourceInfo.custom_fields_values) {
        // TODO: Use resourceAppCustomFieldValue.name once added
        appsCatalogAppCustomFieldsInfo[resourceAppCustomFieldValue.custom_field_type_name] = {
            value: resourceAppCustomFieldValue.image_url !== null
                ? resourceAppCustomFieldValue.image_url
                : resourceAppCustomFieldValue.value,
        };
    }
    const appInfo = {
        name: typeof resourceInfo.display_name !== `string`
            ? undefined
            : resourceInfo.display_name,
        iconUrl: typeof resourceInfo.icon_url !== `string`
            ? undefined
            : resourceInfo.icon_url,
        bannerUrl: typeof resourceInfo.banner_url !== `string`
            ? undefined
            : resourceInfo.banner_url,
        shortDescription: typeof resourceInfo.short_description !== `string`
            ? undefined
            : resourceInfo.short_description,
        longDescription: typeof resourceInfo.long_description !== `string`
            ? undefined
            : resourceInfo.long_description,
        order: resourceInfo.order,
        customFields: appsCatalogAppCustomFieldsInfo,
    };
    return appInfo;
}).fetchAppsCatalogAppInfo;
//# sourceMappingURL=fetch-apps-catalog-app-info.js.map