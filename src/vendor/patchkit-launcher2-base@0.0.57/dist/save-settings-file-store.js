"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.saveSettingsFileStore = void 0;
const _1 = require("./$");
const get_settings_file_store_info_1 = require("./get-settings-file-store-info");
exports.saveSettingsFileStore = _1.__.declareManagedFunc()
    .name(`saveSettingsFileStore`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ settings, }) => {
    await _1.__.saveFileStore({
        fileStoreInfo: (0, get_settings_file_store_info_1.getSettingsFileStoreInfo)(),
        fileStoreData: settings,
    });
}).saveSettingsFileStore;
//# sourceMappingURL=save-settings-file-store.js.map