"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.openUnityProjectDef = void 0;
const _1 = require("./$");
const unity_editor_is_not_installed_1 = require("./unity-editor-is-not-installed");
exports.openUnityProjectDef = _1.__.defineManagedFunc()
    .name(`openUnityProject`)
    .namespace(_1.$namespace)
    .errorTypes([
    unity_editor_is_not_installed_1.UnityEditorIsNotInstalled,
])
    .async()
    .executeType().openUnityProjectDef;
//# sourceMappingURL=open-unity-project-def.js.map