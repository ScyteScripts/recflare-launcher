"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppBranchInfo = void 0;
const _1 = require("./$");
const apps_branches_cached_info_box_1 = require("./apps-branches-cached-info-box");
const fetch_app_branch_info_1 = require("./fetch-app-branch-info");
exports.retrieveAppBranchInfo = _1.__.declareManagedFunc()
    .name(`retrieveAppBranchInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppBranchIsNotAllowed,
    _1._.AppAccessTokenIsInvalid,
    _1._.AppAccessTokenIsExpired,
    _1._.AppBranchDoesNotExist,
    _1._.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appId, appAccessToken, appBranchId, }) => {
    var _a;
    const appBranchCachedInfoBox = (_a = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId];
    if (appBranchCachedInfoBox !== undefined) {
        if (appBranchCachedInfoBox.value === undefined) {
            throw new _1._.AppBranchDoesNotExist();
        }
        return appBranchCachedInfoBox.value;
    }
    return await (0, fetch_app_branch_info_1.fetchAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
}).retrieveAppBranchInfo;
exports.retrieveAppBranchInfo.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-app-branch-info.js.map