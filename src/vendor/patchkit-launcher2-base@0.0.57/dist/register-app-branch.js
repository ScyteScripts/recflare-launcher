"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.registerAppBranch = void 0;
const _1 = require("./$");
const fetch_app_branch_internal_state_1 = require("./fetch-app-branch-internal-state");
const fetch_is_app_branch_registered_1 = require("./fetch-is-app-branch-registered");
const fetch_settings_1 = require("./fetch-settings");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
const save_settings_file_store_1 = require("./save-settings-file-store");
exports.registerAppBranch = _1.__.declareManagedFunc()
    .async()
    .def(_1._.registerAppBranchDef)
    .execute(async ({ appId, appAccessToken, appBranchId, appBranchRootDirPath, isAppBranchNotEmptyRootDirAllowed, }) => {
    try {
        await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
            appId,
            appAccessToken,
            appBranchId,
        });
        if (await (0, fetch_is_app_branch_registered_1.fetchIsAppBranchRegistered)({ appId, appBranchId })) {
            throw new _1._.AppBranchIsAlreadyRegistered();
        }
        try {
            await _1.__.PatchKit.createAppRootDir({
                appRootDirPath: appBranchRootDirPath,
                isAppNotEmptyRootDirAllowed: isAppBranchNotEmptyRootDirAllowed,
            });
        }
        catch (e) {
            if (e instanceof _1.__.PatchKit.AppRootDirIsConflictingWithOtherFileSystemEntry) {
                throw new _1._.AppBranchRootDirIsConflictingWithOtherFileSystemEntry();
            }
            if (e instanceof _1.__.PatchKit.AppRootDirIsNotEmpty) {
                throw new _1._.AppBranchRootDirIsNotEmpty();
            }
            if (e instanceof _1.__.PatchKit.AppRootDirPermissionsAreInsufficient) {
                throw new _1._.AppBranchRootDirPermissionsAreInsufficient();
            }
            throw e;
        }
        const settings = await (0, fetch_settings_1.fetchSettings)();
        await (0, save_settings_file_store_1.saveSettingsFileStore)({
            settings: _1.__.$deepmerge(settings, {
                apps: {
                    [appId]: {
                        branches: {
                            [appBranchId]: {
                                rootDirPath: appBranchRootDirPath,
                            },
                        },
                    },
                },
            }),
        });
    }
    finally {
        try {
            await (0, fetch_app_branch_internal_state_1.fetchAppBranchInternalState)({
                appId,
                appBranchId,
            });
        }
        catch (_a) {
        }
    }
}).registerAppBranch;
//# sourceMappingURL=register-app-branch.js.map