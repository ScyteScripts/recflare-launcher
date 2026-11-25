"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getPendingCheckForRecRoomUpdatesCommandInfoFileStoreInfo = void 0;
const _1 = require("./$");
exports.getPendingCheckForRecRoomUpdatesCommandInfoFileStoreInfo = _1.__.declareManagedFunc()
    .name(`getPendingCheckForRecRoomUpdatesCommandInfoFileStoreInfo`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(_1.__.PatchKitLauncher2.CONFIG.value !== undefined);
    return {
        contentFilePath: _1.__.$path.resolve(_1.__.PatchKitLauncher2.CONFIG.value.targetOperatingSystemPlatform === _1.__.PatchKitLauncher2.TargetOperatingSystemPlatform.Windows
            ? _1.__.PatchKitLauncher2.getRootDirPath()
            : _1.__.PatchKitLauncher2.getUserDataDirPath(), `pending-check-for-recroom-updates-command-info.json`),
        currentVersion: 1,
    };
}).getPendingCheckForRecRoomUpdatesCommandInfoFileStoreInfo;
//# sourceMappingURL=get-pending-check-for-recroom-updates-command-info-file-store-info.js.map