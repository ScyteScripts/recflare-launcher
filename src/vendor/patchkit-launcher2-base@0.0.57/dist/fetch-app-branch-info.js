"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchInfo = void 0;
const _1 = require("./$");
const apps_branches_cached_info_box_1 = require("./apps-branches-cached-info-box");
const config_1 = require("./config");
const get_app_branch_default_root_dir_path_1 = require("./get-app-branch-default-root-dir-path");
const retrieve_app_info_1 = require("./retrieve-app-info");
exports.fetchAppBranchInfo = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchInfoDef)
    .execute(async ({ appId, appAccessToken, appBranchId, }) => {
    var _a, _b;
    var _c;
    const appInfo = await _1.__.invokeAsyncExp(async () => {
        try {
            return await (0, retrieve_app_info_1.retrieveAppInfo)({
                appId,
                appAccessToken,
            });
        }
        catch (e) {
            if (e instanceof _1._.AppIsNotAllowed) {
                throw new _1._.AppBranchIsNotAllowed();
            }
            if (e instanceof _1._.AppDoesNotExist) {
                throw new _1._.AppBranchDoesNotExist();
            }
            throw e;
        }
    });
    const patchKitAppsCatalogAppBranchInfo = await _1.__.invokeAsyncExp(async () => {
        var _a, _b;
        var _c;
        _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
        try {
            return await _1.__.PatchKit.fetchAppsCatalogAppBranchInfo({
                appsCatalogId: config_1.CONFIG.value.appsCatalogId,
                appsCatalogAppId: appId,
                appsCatalogAppAccessToken: appAccessToken,
                appsCatalogAppBranchId: appBranchId,
                appsCatalogAppBranchTargetOperatingSystemPlatform: config_1.CONFIG.value.targetOperatingSystemPlatform,
            });
        }
        catch (e) {
            if (e instanceof _1.__.PatchKit.AppsCatalogAppBranchDoesNotExist) {
                if (((_a = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) !== undefined) {
                    apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId][appBranchId].value = undefined;
                }
                else {
                    (_b = (_c = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
                    apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId][appBranchId] = {
                        value: undefined,
                    };
                }
                throw new _1._.AppBranchDoesNotExist();
            }
            throw e;
        }
    });
    const appBranchInfo = {
        name: patchKitAppsCatalogAppBranchInfo.name,
        secret: patchKitAppsCatalogAppBranchInfo.secret,
        isPreinstalled: false,
        defaultRootDirPath: (0, get_app_branch_default_root_dir_path_1.getAppBranchDefaultRootDirPath)({
            appId,
            appName: appInfo.name,
            appBranchId,
            appBranchName: patchKitAppsCatalogAppBranchInfo.name,
        }),
    };
    if (((_a = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) !== undefined) {
        apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId][appBranchId].value = appBranchInfo;
    }
    else {
        (_b = (_c = apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
        apps_branches_cached_info_box_1.APPS_BRANCHES_CACHED_INFO_BOX.value[appId][appBranchId] = {
            value: appBranchInfo,
        };
    }
    return appBranchInfo;
}).fetchAppBranchInfo;
//# sourceMappingURL=fetch-app-branch-info.js.map