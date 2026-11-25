"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUnityProjectUnityEditorId = void 0;
const _1 = require("./$");
exports.fetchUnityProjectUnityEditorId = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchUnityProjectUnityEditorIdDef)
    .execute(async ({ unityProjectDirPath, }) => {
    const unityProjectVersionFilePath = _1.__.$path.resolve(unityProjectDirPath, `ProjectSettings`, `ProjectVersion.txt`);
    const unityProjectVersion = await _1.__.$fs.readFileSync(unityProjectVersionFilePath, `utf-8`);
    const regex1 = /m_EditorVersion:\s*([^\s]+)/g;
    const regex1Match = regex1.exec(unityProjectVersion);
    const unityEditorVersion = regex1Match !== null ? regex1Match[1] : undefined;
    _1.__.checkAssertion(unityEditorVersion !== undefined);
    const regex2 = /\(([^)]+)\)/g;
    const regex2Match = regex2.exec(unityProjectVersion);
    const unityEditorChangeset = regex2Match !== null ? regex2Match[1] : undefined;
    _1.__.checkAssertion(unityEditorChangeset !== undefined);
    return {
        version: unityEditorVersion,
        changeset: unityEditorChangeset,
    };
}).fetchUnityProjectUnityEditorId;
//# sourceMappingURL=fetch-unity-project-unity-editor-id.js.map