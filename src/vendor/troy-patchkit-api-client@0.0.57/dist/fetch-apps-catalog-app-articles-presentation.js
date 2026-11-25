"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppArticlesPresentation = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchAppsCatalogAppArticlesPresentation = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppArticlesPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, }) => {
    const resourceInfoAsJson = await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
        appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/news_articles`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appsCatalogAppArticlesPresentation = {};
    for (const resourceNewsArticle of resourceInfo.news_articles) {
        appsCatalogAppArticlesPresentation[resourceNewsArticle.id] = {
            title: typeof resourceNewsArticle.title !== `string`
                ? undefined
                : resourceNewsArticle.title,
            body: _1.__.invokeSyncExp(() => {
                if (resourceNewsArticle.body_format === `text`) {
                    return {
                        type: _1._.ContentBodyType.TextBody,
                        value: resourceNewsArticle.body,
                    };
                }
                if (resourceNewsArticle.body_format === `markdown`) {
                    return {
                        type: _1._.ContentBodyType.MarkdownBody,
                        value: resourceNewsArticle.body,
                    };
                }
                if (resourceNewsArticle.body_format === `html`) {
                    return {
                        type: _1._.ContentBodyType.HtmlBody,
                        value: resourceNewsArticle.body,
                    };
                }
                _1.__.checkInvalidAssertion();
            }),
            excerpt: typeof resourceNewsArticle.excerpt !== `string`
                ? undefined
                : resourceNewsArticle.excerpt,
            bannerUrl: typeof resourceNewsArticle.banner_url !== `string`
                ? undefined
                : resourceNewsArticle.banner_url,
            createdDate: new Date(resourceNewsArticle.created_at),
            updatedDate: new Date(resourceNewsArticle.updated_at),
            callToActionUrl: typeof resourceNewsArticle.call_to_action_url !== `string`
                ? undefined
                : resourceNewsArticle.call_to_action_url,
            callToActionLabel: typeof resourceNewsArticle.call_to_action_label !== `string`
                ? undefined
                : resourceNewsArticle.call_to_action_label,
            customData: typeof resourceNewsArticle.custom !== `string`
                ? undefined
                : resourceNewsArticle.custom,
        };
    }
    return appsCatalogAppArticlesPresentation;
}).fetchAppsCatalogAppArticlesPresentation;
//# sourceMappingURL=fetch-apps-catalog-app-articles-presentation.js.map