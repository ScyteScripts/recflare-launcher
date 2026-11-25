"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.readConfigFileContents = void 0;
const _1 = require("./$");
exports.readConfigFileContents = _1.__.declareManagedFunc()
    .name(`readConfigFileContents`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const { overrideConfigFilePath, } = _1.__.$commandlineargs([
        {
            name: `overrideConfigFilePath`,
        },
    ], {
        partial: true,
    });
    const configFilePath = typeof overrideConfigFilePath === `string`
        ? overrideConfigFilePath
        : _1.__.$path.resolve(process.resourcesPath, `config.json`);
    const configFileContents = await _1.__.$fspromises.readFile(configFilePath, `utf8`);
    if (process.env.STUDIO_LAUNCHER_PORTABLE_ROOT) {
        const config = JSON.parse(configFileContents);
        config.overrideRootDirPath = process.env.STUDIO_LAUNCHER_PORTABLE_ROOT;
        config.overrideUserDataDirPath = _1.__.$path.join(config.overrideRootDirPath, 'electron-user-data');
        return JSON.stringify(config);
    }
    return configFileContents;
}).readConfigFileContents;
//# sourceMappingURL=read-config-file-contents.js.map