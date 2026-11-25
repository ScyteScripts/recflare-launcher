"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchRecRoomStudioTemplateUnityEditorId = void 0;
const _1 = require("./$");
exports.fetchRecRoomStudioTemplateUnityEditorId = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchRecRoomStudioTemplateUnityEditorIdDef)
    .execute(async ({ appId, appBranchId, }) => {
    const appBranchInternalState = await _1.__.PatchKitLauncher2.fetchAppBranchInternalState({
        appId,
        appBranchId,
    });
    if (appBranchInternalState.installedVersionId === undefined) {
        throw new _1.__.PatchKitLauncher2.AppBranchIsNotInstalled();
    }
    const recRoomStudioTemplateDirPath = _1.__.$path.resolve(appBranchInternalState.rootDirState.path, `RecRoomStudio`, `ProjectTemplate`);
    const recRoomStudioTemplateProjectVersionFilePath = _1.__.$path.resolve(recRoomStudioTemplateDirPath, `ProjectSettings`, `ProjectVersion.txt`);
    const recRoomStudioTemplateProjectVersion = _1.__.$fs.readFileSync(recRoomStudioTemplateProjectVersionFilePath, `utf-8`);
    const regex1 = /m_EditorVersion:\s*([^\s]+)/g;
    const regex1Match = regex1.exec(recRoomStudioTemplateProjectVersion);
    const unityEditorVersion = regex1Match !== null ? regex1Match[1] : undefined;
    _1.__.checkAssertion(unityEditorVersion !== undefined);
    const regex2 = /\(([^)]+)\)/g;
    const regex2Match = regex2.exec(recRoomStudioTemplateProjectVersion);
    const unityEditorChangeset = regex2Match !== null ? regex2Match[1] : undefined;
    _1.__.checkAssertion(unityEditorChangeset !== undefined);
    return {
        version: unityEditorVersion,
        changeset: unityEditorChangeset,
    };
}).fetchRecRoomStudioTemplateUnityEditorId;
//# sourceMappingURL=fetch-recroom-studio-template-unity-editor-id.js.map