"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createUnityProjectFromRecRoomStudioTemplate = void 0;
const _1 = require("./$");
const open_unity_project_1 = require("./open-unity-project");
const make_sure_that_unity_hub_process_doesnt_exist_1 = require("./make-sure-that-unity-hub-process-doesnt-exist");
exports.createUnityProjectFromRecRoomStudioTemplate = _1.__.declareManagedFunc()
    .async()
    .def(_1._.createUnityProjectFromRecRoomStudioTemplateDef)
    .execute(async ({ appId, appBranchId, unityEditorId, unityProjectDirPath, }) => {
    try {
    const appBranchInternalState = await _1.__.PatchKitLauncher2.fetchAppBranchInternalState({
        appId,
        appBranchId,
    });
    if (appBranchInternalState.installedVersionId === undefined) {
        throw new _1.__.PatchKitLauncher2.AppBranchIsNotInstalled();
    }
    const recRoomStudioTemplateDirPath = _1.__.$path.resolve(appBranchInternalState.rootDirState.path, `RecRoomStudio`, `ProjectTemplate`);
    if (_1.__.$fs.existsSync(unityProjectDirPath) &&
        (!_1.__.$fs.statSync(unityProjectDirPath).isDirectory() || _1.__.$fs.readdirSync(unityProjectDirPath).length !== 0)) {
        throw new Error("Choose a new or empty directory for the project. Existing project files will not be overwritten.");
    }
    require("@recovered/policy").validateTemplate(recRoomStudioTemplateDirPath, appBranchInternalState.installedVersionId);
    await (0, make_sure_that_unity_hub_process_doesnt_exist_1.makeSureThatUnityHubProcessDoesntExist)();
    _1.__.$fs.cpSync(recRoomStudioTemplateDirPath, unityProjectDirPath, {
        recursive: true,
    });
    _1.__.$fs.mkdirSync(_1.__.$path.resolve(unityProjectDirPath, `ProjectSettings`, `Packages`, `com.recroom.studio`), {
        recursive: true,
    });
    _1.__.$fs.writeFileSync(_1.__.$path.resolve(unityProjectDirPath, `ProjectSettings`, `Packages`, `com.recroom.studio`, `BranchId.txt`), appBranchId);
    await (0, open_unity_project_1.openUnityProject)({
        unityEditorId,
        unityProjectDirPath,
    });
    } catch (error) {
        const failure = new _1.__.FatalError();
        failure.message = error instanceof Error ? error.message : String(error);
        throw failure;
    }
}).createUnityProjectFromRecRoomStudioTemplate;
//# sourceMappingURL=create-unity-project-from-recroom-studio-template.js.map
