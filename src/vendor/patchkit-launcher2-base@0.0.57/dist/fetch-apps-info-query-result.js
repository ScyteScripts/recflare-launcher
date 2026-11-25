"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsInfoQueryResult = void 0;
const _1 = require("./$");
const apps_cached_info_box_1 = require("./apps-cached-info-box");
const config_1 = require("./config");
exports.fetchAppsInfoQueryResult = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppsInfoQueryResultDef)
    .execute(async ({ appsInfoQueryConfig, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const patchKitAppsCatalogAppsInfoQueryResult = await _1.__.PatchKit.fetchAppsCatalogAppsInfoQueryResult({
        appsCatalogId: config_1.CONFIG.value.appsCatalogId,
        appsCatalogAppsInfoQueryConfig: {
            appsAccessToken: appsInfoQueryConfig.appsAccessToken,
            offset: appsInfoQueryConfig.offset,
            maxCount: appsInfoQueryConfig.maxCount,
        },
    });
    const appsInfoQueryResult = {
        appsInfo: patchKitAppsCatalogAppsInfoQueryResult.appsInfo,
    };
    for (const [appId, appInfo] of Object.entries(appsInfoQueryResult.appsInfo)) {
        if (apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId] !== undefined) {
            apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId].value = appInfo;
        }
        else {
            apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId] = {
                value: appInfo,
            };
        }
    }
    return appsInfoQueryResult;
}).fetchAppsInfoQueryResult;
//# sourceMappingURL=fetch-apps-info-query-result.js.map