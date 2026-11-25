"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getSettingsFileStoreInfo = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const get_root_dir_path_1 = require("./get-root-dir-path");
const get_user_data_dir_path_1 = require("./get-user-data-dir-path");
exports.getSettingsFileStoreInfo = _1.__.declareManagedFunc()
    .name(`getSettingsFileStoreInfo`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    return {
        unversionedContentDataFilePath: _1.__.$path.resolve(config_1.CONFIG.value.targetOperatingSystemPlatform === _1._.TargetOperatingSystemPlatform.Windows
            ? (0, get_root_dir_path_1.getRootDirPath)()
            : (0, get_user_data_dir_path_1.getUserDataDirPath)(), `settings`),
        contentFilePath: _1.__.$path.resolve(config_1.CONFIG.value.targetOperatingSystemPlatform === _1._.TargetOperatingSystemPlatform.Windows
            ? (0, get_root_dir_path_1.getRootDirPath)()
            : (0, get_user_data_dir_path_1.getUserDataDirPath)(), `settings-store.json`),
        currentVersion: 1,
    };
}).getSettingsFileStoreInfo;
exports.getSettingsFileStoreInfo.config.isLoggingEnabled = false;
//# sourceMappingURL=get-settings-file-store-info.js.map