"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppLauncherThemeId = void 0;
const _1 = require("./$");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const fetch_main_service_resource_info_as_json_1 = require("./fetch-main-service-resource-info-as-json");
exports.fetchAppLauncherThemeId = _1.__.declareManagedFunc()
    .name(`fetchAppLauncherThemeId`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppLauncherThemeDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appSecret, }) => {
    var _a;
    const resourceInfoAsJson = await (0, fetch_main_service_resource_info_as_json_1.fetchMainServiceResourceInfoAsJson)({
        mainServiceResourcePath: `1/apps/${appSecret}`,
    });
    const resourceInfo = _1.__.deserializeValueFromJson({
        json: resourceInfoAsJson,
    });
    const appLauncherThemeId = (_a = resourceInfo.patcher_theme) === null || _a === void 0 ? void 0 : _a.themes_service_id;
    if (appLauncherThemeId === null || appLauncherThemeId === undefined) {
        throw new _1._.AppLauncherThemeDoesNotExist();
    }
    return appLauncherThemeId;
}).fetchAppLauncherThemeId;
//# sourceMappingURL=fetch-app-launcher-theme-id.js.map