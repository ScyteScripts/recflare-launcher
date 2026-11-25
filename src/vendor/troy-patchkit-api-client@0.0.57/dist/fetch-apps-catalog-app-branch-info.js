"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppBranchInfo = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const service_resource_does_not_exist_1 = require("./service-resource-does-not-exist");
exports.fetchAppsCatalogAppBranchInfo = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppBranchInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppsCatalogAppBranchDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, appsCatalogAppAccessToken, appsCatalogAppBranchId, appsCatalogAppBranchTargetOperatingSystemPlatform, }) => {
    const resourcesPlatformType = _1.__.invokeSyncExp(() => {
        if (appsCatalogAppBranchTargetOperatingSystemPlatform === _1._.AppsCatalogAppBranchTargetOperatingSystemPlatform.Windows) {
            return `windows_x86_64`;
        }
        if (appsCatalogAppBranchTargetOperatingSystemPlatform === _1._.AppsCatalogAppBranchTargetOperatingSystemPlatform.MacOS) {
            return `osx_x86_64`;
        }
        if (appsCatalogAppBranchTargetOperatingSystemPlatform === _1._.AppsCatalogAppBranchTargetOperatingSystemPlatform.Linux) {
            return `linux_x86_64`;
        }
        _1.__.checkInvalidAssertion();
    });
    if (appsCatalogAppBranchId === _1._.APPS_CATALOG_APP_MAIN_BRANCH_ID.value) {
        const resourceInfoAsJson = await _1.__.invokeAsyncExp(async () => {
            try {
                return await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
                    appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/patchkit_apps/${resourcesPlatformType}`,
                    appCatalogsServiceResourceAccessToken: appsCatalogAppAccessToken,
                });
            }
            catch (e) {
                if (e instanceof service_resource_does_not_exist_1.ServiceResourceDoesNotExist) {
                    throw new _1._.AppsCatalogAppBranchDoesNotExist();
                }
                throw e;
            }
        });
        const resourceInfo = _1.__.deserializeValueFromJson({
            json: resourceInfoAsJson,
        });
        return {
            secret: resourceInfo.secret,
        };
    }
    else {
        const resourceInfoAsJson = await _1.__.invokeAsyncExp(async () => {
            try {
                return await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
                    appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/patchkit_apps/${resourcesPlatformType}/branches?limit=100`,
                    appCatalogsServiceResourceAccessToken: appsCatalogAppAccessToken,
                });
            }
            catch (e) {
                if (e instanceof service_resource_does_not_exist_1.ServiceResourceDoesNotExist) {
                    throw new _1._.AppsCatalogAppBranchDoesNotExist();
                }
                throw e;
            }
        });
        const resourceInfo = _1.__.deserializeValueFromJson({
            json: resourceInfoAsJson,
        });
        for (const resourceBranch of resourceInfo.branches) {
            if (resourceBranch.id === appsCatalogAppBranchId) {
                return {
                    name: typeof resourceBranch.display_name === `string` && resourceBranch.display_name !== ``
                        ? resourceBranch.display_name
                        : typeof resourceBranch.name === `string` && resourceBranch.name !== ``
                            ? resourceBranch.name
                            : undefined,
                    secret: resourceBranch.secret,
                };
            }
        }
        throw new _1._.AppsCatalogAppBranchDoesNotExist();
    }
}).fetchAppsCatalogAppBranchInfo;
//# sourceMappingURL=fetch-apps-catalog-app-branch-info.js.map