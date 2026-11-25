"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppArticlesLangPresentation = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.fetchAppArticlesLangPresentation = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppArticlesLangPresentationDef)
    .execute(async ({ appId, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const appArticlesLangPresentation = await _1.__.PatchKit.fetchAppsCatalogAppArticlesPresentation({
        appsCatalogId: config_1.CONFIG.value.appsCatalogId,
        appsCatalogAppId: appId,
    });
    return appArticlesLangPresentation;
}).fetchAppArticlesLangPresentation;
//# sourceMappingURL=fetch-app-articles-lang-presentation.js.map