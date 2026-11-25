"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppState = void 0;
const _1 = require("./$");
const fetch_settings_1 = require("./fetch-settings");
const retrieve_app_branch_state_1 = require("./retrieve-app-branch-state");
exports.retrieveAppState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.retrieveAppStateDef)
    .execute(async ({ appId, }) => {
    var _a, _b;
    const settings = await (0, fetch_settings_1.fetchSettings)();
    const appRegisteredBranchesState = {};
    for (const appBranchId of Object.keys((_b = (_a = settings.apps[appId]) === null || _a === void 0 ? void 0 : _a.branches) !== null && _b !== void 0 ? _b : {})) {
        const appBranchState = await _1.__.invokeAsyncExp(async () => {
            try {
                return await (0, retrieve_app_branch_state_1.retrieveAppBranchState)({
                    appId,
                    appBranchId,
                });
            }
            catch (e) {
                if (e instanceof _1._.AppBranchIsNotRegistered) {
                    return undefined;
                }
                throw e;
            }
        });
        if (appBranchState !== undefined) {
            appRegisteredBranchesState[appBranchId] = appBranchState;
        }
    }
    if (Object.entries(appRegisteredBranchesState).length === 0) {
        throw new _1._.AppIsNotRegistered();
    }
    return {
        registeredBranches: appRegisteredBranchesState,
    };
}).retrieveAppState;
exports.retrieveAppState.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-app-state.js.map