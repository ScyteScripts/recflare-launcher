"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.displaySelectAppBranchRootDirDialog = void 0;
const _1 = require("./$");
const fetch_app_branch_info_1 = require("./fetch-app-branch-info");
const fetch_app_info_1 = require("./fetch-app-info");
const get_app_branch_default_root_dir_path_1 = require("./get-app-branch-default-root-dir-path");
const window_handle_1 = require("./window-handle");
exports.displaySelectAppBranchRootDirDialog = _1.__.declareManagedFunc()
    .async()
    .def(_1._.displaySelectAppBranchRootDirDialogDef)
    .execute(async ({ selectAppBranchRootDirDialogConfig, }) => {
    var _a, _b;
    const appInfo = await (0, fetch_app_info_1.fetchAppInfo)({
        appId: selectAppBranchRootDirDialogConfig.appId,
        appAccessToken: selectAppBranchRootDirDialogConfig.appAccessToken,
    });
    const appBranchInfo = await (0, fetch_app_branch_info_1.fetchAppBranchInfo)({
        appId: selectAppBranchRootDirDialogConfig.appId,
        appAccessToken: selectAppBranchRootDirDialogConfig.appAccessToken,
        appBranchId: selectAppBranchRootDirDialogConfig.appBranchId,
    });
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value !== undefined);
    const electronDialogResult = await _1.__.$electron.dialog.showOpenDialog(window_handle_1.WINDOW_HANDLE.value, {
        properties: [
            `openDirectory`,
            `createDirectory`,
        ],
        title: selectAppBranchRootDirDialogConfig.title,
        message: selectAppBranchRootDirDialogConfig.message,
        defaultPath: (_a = selectAppBranchRootDirDialogConfig.overrideDefaultDirPath) !== null && _a !== void 0 ? _a : (0, get_app_branch_default_root_dir_path_1.getAppBranchDefaultRootDirPath)({
            appId: selectAppBranchRootDirDialogConfig.appId,
            appName: appInfo.name,
            appBranchId: selectAppBranchRootDirDialogConfig.appBranchId,
            appBranchName: appBranchInfo.name,
        }),
    });
    if (electronDialogResult.canceled || electronDialogResult.filePaths.length === 0) {
        return {
            selectAppBranchRootDirDialogResult: {},
        };
    }
    const electronDialogResultDirPath = electronDialogResult.filePaths[0];
    _1.__.checkAssertion(electronDialogResultDirPath !== undefined);
    if (_1.__.$fs.existsSync(electronDialogResultDirPath)) {
        if (!_1.__.$fs.lstatSync(electronDialogResultDirPath).isDirectory()) {
            return {
                selectAppBranchRootDirDialogResult: {},
            };
        }
        if (_1.__.$fs.readdirSync(electronDialogResultDirPath).length !== 0) {
            return {
                selectAppBranchRootDirDialogResult: {
                    appBranchSelectedRootDirPath: _1.__.$path.resolve(electronDialogResultDirPath, (_b = appInfo.name) !== null && _b !== void 0 ? _b : `App`),
                },
            };
        }
    }
    return {
        selectAppBranchRootDirDialogResult: {
            appBranchSelectedRootDirPath: electronDialogResultDirPath,
        },
    };
}).displaySelectAppBranchRootDirDialog;
//# sourceMappingURL=display-select-app-branch-root-dir-dialog.js.map