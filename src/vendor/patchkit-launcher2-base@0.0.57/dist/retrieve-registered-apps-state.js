"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveRegisteredAppsState = void 0;
const _1 = require("./$");
const retrieve_app_state_1 = require("./retrieve-app-state");
const fetch_settings_1 = require("./fetch-settings");
exports.retrieveRegisteredAppsState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.retrieveRegisteredAppsStateDef)
    .execute(async () => {
    var _a;
    const settings = await (0, fetch_settings_1.fetchSettings)();
    const registeredAppsState = {};
    for (const appId of Object.keys((_a = settings.apps) !== null && _a !== void 0 ? _a : {})) {
        const appState = await _1.__.invokeAsyncExp(async () => {
            try {
                return await (0, retrieve_app_state_1.retrieveAppState)({
                    appId,
                });
            }
            catch (e) {
                if (e instanceof _1._.AppIsNotRegistered) {
                    return undefined;
                }
                throw e;
            }
        });
        if (appState !== undefined) {
            registeredAppsState[appId] = appState;
        }
    }
    return registeredAppsState;
}).retrieveRegisteredAppsState;
exports.retrieveRegisteredAppsState.config.isLoggingEnabled = false;
//# sourceMappingURL=retrieve-registered-apps-state.js.map