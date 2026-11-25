"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUnityUninstallerLocation = void 0;
const _1 = require("./$");
const fs = require("fs");
const fetch_unity_editor_exe_path_1 = require("./fetch-unity-editor-exe-path");
exports.fetchUnityUninstallerLocation = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchUnityUninstallerLocationDef)
    .execute(async ({ unityEditorId, }) => {
    const unityEditorExePath = await (0, fetch_unity_editor_exe_path_1.fetchUnityEditorExePath)({
        unityEditorId,
    });
    if (unityEditorExePath === undefined) {
        return undefined;
    }
    const unityDirectoryPrefix = unityEditorExePath.substring(0, unityEditorExePath.indexOf("Unity.exe"));
    const uninstallerLocation = `${unityDirectoryPrefix}Uninstall.exe`;
    if (!fs.existsSync(uninstallerLocation)) {
        return undefined;
    }
    return uninstallerLocation;
}).fetchUnityUninstallerLocation;
//# sourceMappingURL=fetch-unity-uninstaller-location.js.map