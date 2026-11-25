"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchesInfoQueryResult = void 0;
const _1 = require("./$");
const apps_branches_cached_info_box_1 = require("./apps-branches-cached-info-box");
const config_1 = require("./config");
const get_app_branch_default_root_dir_path_1 = require("./get-app-branch-default-root-dir-path");
const retrieve_app_info_1 = require("./retrieve-app-info");
exports.fetchAppBranchesInfoQueryResult = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchesInfoQueryResultDef)
    .execute(async ({ appId, appAccessToken, appBranchesInfoQueryConfig, }) => {
    var _a, _b;
    var _c;
    const appInfo = await (0, retrieve_app_info_1.retrieveAppInfo)({
        appId,
        appAccessToken,
    });
    const patchKitAppsCatalogAppBranchesInfoQueryResult = await _1.__.invokeAsyncExp(async () => {
        _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
        try {
            return await _1.__.PatchKit.fetchAppsCatalogAppBranchesInfoQueryResult({
                appsCatalogId: config_1.CONFIG.value.appsCatalogId,
                appsCatalogAppId: appId,
                appsCatalogAppAccessToken: appAccessToken,
                appsCatalogAppBranchesInfoQueryConfig: {
                    branchesTargetOperatingSystemPlatform: config_1.CONFIG.value.targetOperatingSystemPlatform,
                    offset: appBranchesInfoQueryConfig.offset,
                    maxCount: appBranchesInfoQueryConfig.maxCount,
                },
            });
        }
        catch (e) {
            if (e instanceof _1.__.PatchKit.AppsCatalogAppDoesNotExist) {
                throw new _1._.AppDoesNotExist();
            }
            throw e;
        }
    });
    const appBranchesInfoQueryResult = {
        branchesInfo: {},
    };
    for (const [appsCatalogAppBranchId, appsCatalogAppBranchInfo] of Object.entries(patchKitAppsCatalogAppBranchesInfoQueryResult.branchesInfo)) {
        const appBranchId = appsCatalogAppBranchId;
        const appBranchInfo = {
            name: appsCatalogAppBranchInfo.name,
            secret: appsCatalogAppBranchInfo.secret,
            isPreinstalled: false,
            defaultRootDirPath: (0, get_app_branch_default_root_dir_path_1.getAppBranchDefaultRootDirPath)({
                appId,
                appName: appInfo.name,
                appBranchId,
                appBranchName: appsCatalogAppBranchInfo.name,
            }),
        };
        appBranchesInfoQueryResult.branchesInfo[appBranchId] = appBranchInfo;
        if (((_a = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) !== undefined) {
            apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId][appBranchId].value = appBranchInfo;
        }
        else {
            (_b = (_c = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
            apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId][appBranchId] = {
                value: appBranchInfo,
            };
        }
    }
    return appBranchesInfoQueryResult;
}).fetchAppBranchesInfoQueryResult;
//# sourceMappingURL=fetch-app-branches-info-query-result.js.map