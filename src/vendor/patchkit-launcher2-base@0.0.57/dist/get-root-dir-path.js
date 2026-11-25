"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getRootDirPath = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.getRootDirPath = _1.__.declareManagedFunc()
    .name(`getRootDirPath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    if (config_1.CONFIG.value.overrideRootDirPath !== undefined) {
        return config_1.CONFIG.value.overrideRootDirPath;
    }
    return _1.__.$path.dirname(_1.__.$electron.app.getPath(`exe`));
}).getRootDirPath;
exports.getRootDirPath.config.isLoggingEnabled = false;
//# sourceMappingURL=get-root-dir-path.js.map