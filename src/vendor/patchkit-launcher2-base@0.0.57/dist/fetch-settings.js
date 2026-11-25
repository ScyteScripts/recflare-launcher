"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchSettings = void 0;
const _1 = require("./$");
const convert_unversioned_settings_to_settings_1_1 = require("./convert-unversioned-settings-to-settings-1");
const get_settings_file_store_info_1 = require("./get-settings-file-store-info");
exports.fetchSettings = _1.__.declareManagedFunc()
    .name(`fetchSettings`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const settingsFileStoreContents = await _1.__.fetchFileStoreContent({
        fileStoreInfo: (0, get_settings_file_store_info_1.getSettingsFileStoreInfo)(),
    });
    if (settingsFileStoreContents === undefined) {
        return {
            apps: {},
        };
    }
    if (settingsFileStoreContents.version === undefined) {
        return (0, convert_unversioned_settings_to_settings_1_1.convertUnversionedSettingsToSettings1)({
            unversionedSettings: settingsFileStoreContents.data,
        });
    }
    if (settingsFileStoreContents.version === 1) {
        return settingsFileStoreContents.data;
    }
    _1.__.checkInvalidAssertion();
}).fetchSettings;
exports.fetchSettings.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-settings.js.map