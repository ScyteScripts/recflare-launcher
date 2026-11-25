"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppInfo = void 0;
const _1 = require("./$");
const apps_cached_info_box_1 = require("./apps-cached-info-box");
const config_1 = require("./config");
exports.fetchAppInfo = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppInfoDef)
    .execute(async ({ appId, appAccessToken, }) => {
    const patchKitAppsCatalogAppInfo = await _1.__.invokeAsyncExp(async () => {
        _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
        try {
            return await _1.__.PatchKit.fetchAppsCatalogAppInfo({
                appsCatalogId: config_1.CONFIG.value.appsCatalogId,
                appsCatalogAppId: appId,
                appsCatalogAppAccessToken: appAccessToken,
            });
        }
        catch (e) {
            if (e instanceof _1.__.PatchKit.AppsCatalogAppDoesNotExist) {
                if (apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId] !== undefined) {
                    apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId].value = undefined;
                }
                else {
                    apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId] = {
                        value: undefined,
                    };
                }
                throw new _1._.AppDoesNotExist();
            }
            throw e;
        }
    });
    const appInfo = patchKitAppsCatalogAppInfo;
    if (apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId] !== undefined) {
        apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId].value = appInfo;
    }
    else {
        apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId] = {
            value: appInfo,
        };
    }
    return appInfo;
}).fetchAppInfo;
//# sourceMappingURL=fetch-app-info.js.map