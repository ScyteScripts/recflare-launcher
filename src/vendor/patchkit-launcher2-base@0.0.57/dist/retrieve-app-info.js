"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppInfo = void 0;
const _1 = require("./$");
const apps_cached_info_box_1 = require("./apps-cached-info-box");
const fetch_app_info_1 = require("./fetch-app-info");
exports.retrieveAppInfo = _1.__.declareManagedFunc()
    .name(`retrieveAppInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppIsNotAllowed,
    _1._.AppDoesNotExist,
    _1._.AppAccessTokenIsInvalid,
    _1._.AppAccessTokenIsExpired,
    _1._.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appId, appAccessToken, }) => {
    const appCachedInfoBox = apps_cached_info_box_1.APPS_CACHED_INFO_BOX.value[appId];
    if (appCachedInfoBox !== undefined) {
        if (appCachedInfoBox.value === undefined) {
            throw new _1._.AppDoesNotExist();
        }
        return appCachedInfoBox.value;
    }
    return await (0, fetch_app_info_1.fetchAppInfo)({
        appId,
        appAccessToken,
    });
}).retrieveAppInfo;
exports.retrieveAppInfo.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-app-info.js.map