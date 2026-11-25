"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getAppBranchDefaultRootDirPath = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const get_root_dir_path_1 = require("./get-root-dir-path");
const get_user_data_dir_path_1 = require("./get-user-data-dir-path");
exports.getAppBranchDefaultRootDirPath = _1.__.declareManagedFunc()
    .name(`getAppBranchDefaultRootDirPath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ appId, appName, appBranchId, appBranchName, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const appBranchDefaultRootDirConfig = appBranchId === _1._.APP_MAIN_BRANCH_ID.value
        ? config_1.CONFIG.value.appsMainBranchDefaultRootDir
        : config_1.CONFIG.value.appsSecondaryBranchDefaultRootDir;
    return _1.__.$path.resolve(appBranchDefaultRootDirConfig.pathTemplate
        .split(`{rootDirPath}`).join((0, get_root_dir_path_1.getRootDirPath)())
        .split(`{userDataDirPath}`).join((0, get_user_data_dir_path_1.getUserDataDirPath)())
        .split(`{appId}`).join(appId)
        .split(`{appName}`).join(appName !== null && appName !== void 0 ? appName : ``)
        .split(`{appBranchId}`).join(appBranchId)
        .split(`{appBranchShortId}`).join(appBranchId.slice(0, 6))
        .split(`{appBranchName}`).join(appBranchName !== null && appBranchName !== void 0 ? appBranchName : ``));
}).getAppBranchDefaultRootDirPath;
//# sourceMappingURL=get-app-branch-default-root-dir-path.js.map