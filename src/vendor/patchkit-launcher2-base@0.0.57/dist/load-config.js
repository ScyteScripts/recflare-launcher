"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.loadConfig = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const read_config_file_contents_1 = require("./read-config-file-contents");
exports.loadConfig = _1.__.declareManagedFunc()
    .name(`loadConfig`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    config_1.CONFIG.value = _1.__.deserializeValueFromJson({
        json: await (0, read_config_file_contents_1.readConfigFileContents)(),
    });
}).loadConfig;
//# sourceMappingURL=load-config.js.map