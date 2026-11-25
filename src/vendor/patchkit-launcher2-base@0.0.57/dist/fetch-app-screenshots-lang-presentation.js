"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppScreenshotsLangPresentation = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.fetchAppScreenshotsLangPresentation = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppScreenshotsLangPresentationDef)
    .execute(async ({ appId, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const appScreenshotsLangPresentation = await _1.__.PatchKit.fetchAppsCatalogAppScreenshotsPresentation({
        appsCatalogId: config_1.CONFIG.value.appsCatalogId,
        appsCatalogAppId: appId,
    });
    return appScreenshotsLangPresentation;
}).fetchAppScreenshotsLangPresentation;
//# sourceMappingURL=fetch-app-screenshots-lang-presentation.js.map