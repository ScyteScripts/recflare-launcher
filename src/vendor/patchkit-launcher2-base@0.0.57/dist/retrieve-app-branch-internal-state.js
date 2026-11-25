"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppBranchInternalState = void 0;
const _1 = require("./$");
const apps_branches_cached_internal_state_box_1 = require("./apps-branches-cached-internal-state-box");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
exports.retrieveAppBranchInternalState = _1.__.declareManagedFunc()
    .name(`retrieveAppBranchInternalState`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppBranchIsNotAllowed,
    _1._.AppBranchIsNotRegistered,
])
    .async()
    .execute(async ({ appId, appBranchId, }) => {
    var _a;
    const appBranchCachedInternalStateBox = (_a = apps_branches_cached_internal_state_box_1.APPS_BRANCHES_CACHED_INTERNAL_STATE_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId];
    if (appBranchCachedInternalStateBox !== undefined) {
        if (appBranchCachedInternalStateBox.value === undefined) {
            throw new _1._.AppBranchIsNotRegistered();
        }
        return appBranchCachedInternalStateBox.value;
    }
    return await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
        appId,
        appBranchId,
    });
}).retrieveAppBranchInternalState;
exports.retrieveAppBranchInternalState.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-app-branch-internal-state.js.map