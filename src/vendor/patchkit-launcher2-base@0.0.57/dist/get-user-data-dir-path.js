"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getUserDataDirPath = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.getUserDataDirPath = _1.__.declareManagedFunc()
    .name(`getUserDataDirPath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    if (config_1.CONFIG.value.overrideUserDataDirPath !== undefined) {
        return config_1.CONFIG.value.overrideUserDataDirPath;
    }
    return _1.__.$electron.app.getPath(`userData`);
}).getUserDataDirPath;
//# sourceMappingURL=get-user-data-dir-path.js.map