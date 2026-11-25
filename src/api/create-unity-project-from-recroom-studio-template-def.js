"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createUnityProjectFromRecRoomStudioTemplateDef = void 0;
const _1 = require("./$");
const unity_editor_is_not_installed_1 = require("./unity-editor-is-not-installed");
exports.createUnityProjectFromRecRoomStudioTemplateDef = _1.__.defineManagedFunc()
    .name(`createUnityProjectFromRecRoomStudioTemplate`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1.__.PatchKitLauncher2.AppBranchIsNotRegistered,
    _1.__.PatchKitLauncher2.AppBranchIsNotInstalled,
    unity_editor_is_not_installed_1.UnityEditorIsNotInstalled,
])
    .async()
    .executeType().createUnityProjectFromRecRoomStudioTemplateDef;
//# sourceMappingURL=create-unity-project-from-recroom-studio-template-def.js.map