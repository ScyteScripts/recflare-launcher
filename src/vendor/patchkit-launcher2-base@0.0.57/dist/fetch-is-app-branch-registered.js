"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsAppBranchRegistered = void 0;
const _1 = require("./$");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
exports.fetchIsAppBranchRegistered = _1.__.declareManagedFunc()
    .name(`fetchIsAppBranchRegistered`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppBranchIsNotAllowed,
])
    .async()
    .execute(async ({ appId, appBranchId, }) => {
    try {
        await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
            appId,
            appBranchId,
        });
        return true;
    }
    catch (e) {
        if (e instanceof _1._.AppBranchIsNotRegistered) {
            return false;
        }
        throw e;
    }
}).fetchIsAppBranchRegistered;
//# sourceMappingURL=fetch-is-app-branch-registered.js.map