"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getLogsDirPath = void 0;
const _1 = require("./$");
const get_user_data_dir_path_1 = require("./get-user-data-dir-path");
exports.getLogsDirPath = _1.__.declareManagedFunc()
    .name(`getLogsDirPath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    return _1.__.$path.join((0, get_user_data_dir_path_1.getUserDataDirPath)(), `logs`);
}).getLogsDirPath;
//# sourceMappingURL=get-logs-dir-path.js.map