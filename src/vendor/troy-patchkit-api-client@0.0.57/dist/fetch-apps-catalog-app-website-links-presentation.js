"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppWebsiteLinksPresentation = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppsCatalogAppWebsiteLinksPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppWebsiteLinksPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, }) => {
    const resourceInfoAsJson = await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
        appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/website_urls`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appsCatalogAppWebsiteLinksPresentation = {};
    for (const resourceWebsiteUrl of resourceInfo.website_urls) {
        appsCatalogAppWebsiteLinksPresentation[resourceWebsiteUrl.id] = {
            url: resourceWebsiteUrl.url,
            kind: _1.__.invokeSyncExp(() => {
                if (resourceWebsiteUrl.kind === `other`) {
                    return _1._.AppsCatalogAppWebsiteLinkKind.Other;
                }
                else if (resourceWebsiteUrl.kind === `homepage`) {
                    return _1._.AppsCatalogAppWebsiteLinkKind.Homepage;
                }
                else if (resourceWebsiteUrl.kind === `support`) {
                    return _1._.AppsCatalogAppWebsiteLinkKind.Support;
                }
                else if (resourceWebsiteUrl.kind === `forum`) {
                    return _1._.AppsCatalogAppWebsiteLinkKind.Forum;
                }
                _1.__.checkInvalidAssertion();
            }),
            name: typeof resourceWebsiteUrl.display_name !== `string`
                ? undefined
                : resourceWebsiteUrl.display_name,
            description: typeof resourceWebsiteUrl.description !== `string`
                ? undefined
                : resourceWebsiteUrl.description,
            order: typeof resourceWebsiteUrl.order !== `number`
                ? Number.MAX_SAFE_INTEGER
                : resourceWebsiteUrl.order,
        };
    }
    return appsCatalogAppWebsiteLinksPresentation;
}).fetchAppsCatalogAppWebsiteLinksPresentation;
//# sourceMappingURL=fetch-apps-catalog-app-website-links-presentation.js.map