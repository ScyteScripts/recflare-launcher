"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.removeUnityHubRegKey = void 0;
const _1 = require("./$");
const $winreg = require("winreg");
exports.removeUnityHubRegKey = _1.__.declareManagedFunc()
    .async()
    .def(_1._.removeUnityHubRegKeyDef)
    .execute(async () => {
    const unityHubWinRegHandle = new $winreg({
        hive: $winreg.HKLM,
        key: `\\SOFTWARE\\Unity Technologies\\Hub`,
    });
    unityHubWinRegHandle.keyExists((winRegError, doesUnityHubWinRegKeyExists) => {
        if (winRegError !== undefined && winRegError !== null) {
            return;
        }
        if (doesUnityHubWinRegKeyExists) {
            const regKeyCleanupScriptLocation = _1.__.PatchKitLauncher2.getAssetPath({ assetId: 'unityHubRegKeyCleanup' });
            // Imports the bundled .reg file that clears Hub's stale InstallLocation value. Windows
            // asks the player to allow Registry Editor first; no shell, so the path is one argument.
            const regedit = _1.__.$path.join(process.env.SystemRoot || 'C:\\Windows', 'regedit.exe');
            _1.__.$childprocess.execFile(regedit, ['/s', regKeyCleanupScriptLocation], (error, _, __) => {
                if (error !== null) {
                    console.log(error.stack);
                }
            });
        }
    });
}).removeUnityHubRegKey;
//# sourceMappingURL=remove-unity-hub-reg-key.js.map