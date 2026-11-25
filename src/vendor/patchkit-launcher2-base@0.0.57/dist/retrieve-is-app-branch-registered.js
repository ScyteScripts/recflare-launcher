"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveIsAppBranchRegistered = void 0;
const _1 = require("./$");
const retrieve_app_branch_internal_state_1 = require("./retrieve-app-branch-internal-state");
exports.retrieveIsAppBranchRegistered = _1.__.declareManagedFunc()
    .async()
    .def(_1._.retrieveIsAppBranchRegisteredDef)
    .execute(async ({ appId, appBranchId, }) => {
    try {
        await (0, retrieve_app_branch_internal_state_1.retrieveAppBranchInternalState)({
            appId,
            appBranchId,
        });
    }
    catch (e) {
        if (e instanceof _1._.AppBranchIsNotRegistered) {
            return false;
        }
        throw e;
    }
    return true;
}).retrieveIsAppBranchRegistered;
exports.retrieveIsAppBranchRegistered.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-is-app-branch-registered.js.map