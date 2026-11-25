"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.openUnityProject = void 0;
const _1 = require("./$");
const fetch_unity_editor_exe_path_1 = require("./fetch-unity-editor-exe-path");
exports.openUnityProject = _1.__.declareManagedFunc()
    .async()
    .def(_1._.openUnityProjectDef)
    .execute(async ({ unityEditorId, unityProjectDirPath, }) => {
    require('@recovered/routing-repair').repairDevelopmentProfiles();
    require('@recovered/routing-repair').repairDevelopmentProject(unityProjectDirPath);
    const unityEditorExePath = await (0, fetch_unity_editor_exe_path_1.fetchUnityEditorExePath)({
        unityEditorId,
    });
    if (unityEditorExePath === undefined) {
        throw new _1._.UnityEditorIsNotInstalled();
    }
    _1.__.$childprocess.spawn(unityEditorExePath, [
        `-projectPath`,
        unityProjectDirPath,
    ], {
        detached: true,
    });
}).openUnityProject;
//# sourceMappingURL=open-unity-project.js.map