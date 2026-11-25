"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsUnityEditorInstallBroken = void 0;
const _1 = require("./$");
const fs = require("fs");
const fetch_unity_editor_exe_path_1 = require("./fetch-unity-editor-exe-path");
exports.fetchIsUnityEditorInstallBroken = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchIsUnityEditorInstallBrokenDef)
    .execute(async ({ unityEditorId }) => {
    const unityEditorExePath = await (0, fetch_unity_editor_exe_path_1.fetchUnityEditorExePath)({
        unityEditorId,
    });
    if (unityEditorExePath === undefined) {
        return true;
    }
    const unityDirectoryPrefix = unityEditorExePath.substring(0, unityEditorExePath.indexOf("Unity.exe"));
    const packageManagerExePath = `${unityDirectoryPrefix}Data\\Resources\\PackageManager\\Server\\UnityPackageManager.exe`;
    return !fs.existsSync(unityEditorExePath) || !fs.existsSync(packageManagerExePath);
}).fetchIsUnityEditorInstallBroken;
//# sourceMappingURL=fetch-is-unity-editor-install-broken.js.map