"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getWindowPreloadScriptFilePath = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const get_user_data_dir_path_1 = require("./get-user-data-dir-path");
exports.getWindowPreloadScriptFilePath = _1.__.declareManagedFunc()
    .name(`getWindowPreloadScriptFilePath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    return _1.__.$path.resolve((0, get_user_data_dir_path_1.getUserDataDirPath)(), `window-preload.js`);
}).getWindowPreloadScriptFilePath;
//# sourceMappingURL=get-window-preload-script-file-path.js.map