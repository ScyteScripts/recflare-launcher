"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppsCatalogAppBranchesInfoQueryResult = void 0;
const _1 = require("./$");
const fetch_app_catalogs_service_resource_info_as_json_1 = require("./fetch-app-catalogs-service-resource-info-as-json");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const service_resource_does_not_exist_1 = require("./service-resource-does-not-exist");
exports.fetchAppsCatalogAppBranchesInfoQueryResult = _1.__.declareManagedFunc()
    .name(`fetchAppsCatalogAppBranchesInfoQueryResult`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.AppsCatalogAppDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ appsCatalogId, appsCatalogAppId, appsCatalogAppAccessToken, appsCatalogAppBranchesInfoQueryConfig, }) => {
    const resourcesPlatformType = _1.__.invokeSyncExp(() => {
        if (appsCatalogAppBranchesInfoQueryConfig.branchesTargetOperatingSystemPlatform === _1._.AppsCatalogAppBranchTargetOperatingSystemPlatform.Windows) {
            return `windows_x86_64`;
        }
        if (appsCatalogAppBranchesInfoQueryConfig.branchesTargetOperatingSystemPlatform === _1._.AppsCatalogAppBranchTargetOperatingSystemPlatform.MacOS) {
            return `osx_x86_64`;
        }
        if (appsCatalogAppBranchesInfoQueryConfig.branchesTargetOperatingSystemPlatform === _1._.AppsCatalogAppBranchTargetOperatingSystemPlatform.Linux) {
            return `linux_x86_64`;
        }
        _1.__.checkInvalidAssertion();
    });
    const resource1InfoAsJson = await _1.__.invokeAsyncExp(async () => {
        try {
            return await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
                appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/patchkit_apps/${resourcesPlatformType}`,
                appCatalogsServiceResourceAccessToken: appsCatalogAppAccessToken,
            });
        }
        catch (e) {
            if (e instanceof service_resource_does_not_exist_1.ServiceResourceDoesNotExist) {
                return undefined;
            }
            throw e;
        }
    });
    if (resource1InfoAsJson === undefined) {
        return {
            branchesInfo: {},
        };
    }
    const resource1Info = _1.__.deserializeValueFromJson({
        json: resource1InfoAsJson,
    });
    const appsCatalogAppBranchesInfoQueryResult = {
        branchesInfo: {},
    };
    appsCatalogAppBranchesInfoQueryResult.branchesInfo[_1._.APPS_CATALOG_APP_MAIN_BRANCH_ID.value] = {
        secret: resource1Info.secret,
    };
    const resource2InfoAsJson = await (0, fetch_app_catalogs_service_resource_info_as_json_1.fetchAppCatalogsServiceResourceInfoAsJson)({
        appCatalogsServiceResourcePath: `v1/catalogs/${appsCatalogId}/apps/${appsCatalogAppId}/patchkit_apps/${resourcesPlatformType}/branches?limit=100`,
        appCatalogsServiceResourceAccessToken: appsCatalogAppAccessToken,
    });
    const resource2Info = _1.__.deserializeValueFromJson({
        json: resource2InfoAsJson,
    });
    for (const resource2Branch of resource2Info.branches) {
        if (resource2Branch.name === `main`) {
            continue;
        }
        appsCatalogAppBranchesInfoQueryResult.branchesInfo[resource2Branch.id] = {
            name: typeof resource2Branch.display_name === `string` && resource2Branch.display_name !== ``
                ? resource2Branch.display_name
                : typeof resource2Branch.name === `string` && resource2Branch.name !== ``
                    ? resource2Branch.name
                    : undefined,
            secret: resource2Branch.secret,
        };
    }
    return appsCatalogAppBranchesInfoQueryResult;
}).fetchAppsCatalogAppBranchesInfoQueryResult;
//# sourceMappingURL=fetch-apps-catalog-app-branches-info-query-result.js.map