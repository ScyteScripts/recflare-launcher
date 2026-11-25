"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppWebsiteLinksLangPresentation = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.fetchAppWebsiteLinksLangPresentation = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppWebsiteLinksLangPresentationDef)
    .execute(async ({ appId, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const appWebsiteLinksLangPresentation = await _1.__.PatchKit.fetchAppsCatalogAppWebsiteLinksPresentation({
        appsCatalogId: config_1.CONFIG.value.appsCatalogId,
        appsCatalogAppId: appId,
    });
    return appWebsiteLinksLangPresentation;
}).fetchAppWebsiteLinksLangPresentation;
//# sourceMappingURL=fetch-app-website-links-lang-presentation.js.map