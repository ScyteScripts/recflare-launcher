"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getAssetPath = void 0;
const _1 = require("./$");
const asset_does_not_exist_1 = require("./asset-does-not-exist");
const config_1 = require("./config");
exports.getAssetPath = _1.__.declareManagedFunc()
    .name(`getAssetPath`)
    .namespace(_1.$namespace)
    .errorTypes([
    asset_does_not_exist_1.AssetDoesNotExist,
])
    .sync()
    .execute(({ assetId, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const assetConfig = config_1.CONFIG.value.assets[assetId];
    if (assetConfig === undefined) {
        throw new asset_does_not_exist_1.AssetDoesNotExist();
    }
    if (assetConfig.overridePath !== undefined) {
        return assetConfig.overridePath;
    }
    return _1.__.$path.resolve(process.resourcesPath, `assets`, assetId);
}).getAssetPath;
//# sourceMappingURL=get-asset-path.js.map