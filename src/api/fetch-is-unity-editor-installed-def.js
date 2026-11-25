"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsUnityEditorInstalledDef = void 0;
const _1 = require("./$");
const unity_hub_is_not_installed_1 = require("./unity-hub-is-not-installed");
exports.fetchIsUnityEditorInstalledDef = _1.__.defineManagedFunc()
    .name(`fetchIsUnityEditorInstalled`)
    .namespace(_1.$namespace)
    .errorTypes([
    unity_hub_is_not_installed_1.UnityHubIsNotInstalled,
])
    .async()
    .executeType().fetchIsUnityEditorInstalledDef;
//# sourceMappingURL=fetch-is-unity-editor-installed-def.js.map