"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsUnityEditorInstalled = void 0;
const _1 = require("./$");
const fetch_unity_editor_exe_path_1 = require("./fetch-unity-editor-exe-path");
exports.fetchIsUnityEditorInstalled = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchIsUnityEditorInstalledDef)
    .execute(async ({ unityEditorId, }) => {
    const unityEditorExePath = await (0, fetch_unity_editor_exe_path_1.fetchUnityEditorExePath)({
        unityEditorId,
    });
    return unityEditorExePath !== undefined;
}).fetchIsUnityEditorInstalled;
//# sourceMappingURL=fetch-is-unity-editor-installed.js.map