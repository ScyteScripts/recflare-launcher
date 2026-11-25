"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUnityProjectUnityEditorIdDef = void 0;
const _1 = require("./$");
const unity_project_does_not_exist_1 = require("./unity-project-does-not-exist");
exports.fetchUnityProjectUnityEditorIdDef = _1.__.defineManagedFunc()
    .name(`fetchUnityProjectUnityEditorId`)
    .namespace(_1.$namespace)
    .errorTypes([
    unity_project_does_not_exist_1.UnityProjectDoesNotExist,
])
    .async()
    .executeType().fetchUnityProjectUnityEditorIdDef;
//# sourceMappingURL=fetch-unity-project-unity-editor-id-def.js.map