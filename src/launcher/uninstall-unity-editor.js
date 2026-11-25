"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.uninstallUnityEditor = void 0;
const _1 = require("./$");
const fetch_unity_editor_exe_path_1 = require("./fetch-unity-editor-exe-path");
exports.uninstallUnityEditor = _1.__.declareManagedFunc()
    .async()
    .def(_1._.uninstallUnityEditorDef)
    .execute(async ({ unityEditorId }) => {
    const unityEditorExePath = await (0, fetch_unity_editor_exe_path_1.fetchUnityEditorExePath)({
        unityEditorId,
    });
    if (unityEditorExePath === undefined) {
        return;
    }
    const unityDirectoryPrefix = unityEditorExePath.substring(0, unityEditorExePath.indexOf("Unity.exe"));
    await new Promise((resolvePromise) => {
        _1.__.$childprocess.spawn(`"${unityDirectoryPrefix}Uninstall.exe"`, [], {
            shell: true,
        }).on(`exit`, resolvePromise);
    });
}).uninstallUnityEditor;
//# sourceMappingURL=uninstall-unity-editor.js.map