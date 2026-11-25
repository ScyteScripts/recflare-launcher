"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppVideosLangPresentation = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.fetchAppVideosLangPresentation = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppVideosLangPresentationDef)
    .execute(async ({ appId, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const appVideosLangPresentation = await _1.__.PatchKit.fetchAppsCatalogAppVideosPresentation({
        appsCatalogId: config_1.CONFIG.value.appsCatalogId,
        appsCatalogAppId: appId,
    });
    return appVideosLangPresentation;
}).fetchAppVideosLangPresentation;
//# sourceMappingURL=fetch-app-videos-lang-presentation.js.map