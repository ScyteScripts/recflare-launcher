"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.installUnityHubDef = void 0;
const _1 = require("./$");
const unity_hub_is_already_installed_1 = require("./unity-hub-is-already-installed");
exports.installUnityHubDef = _1.__.defineManagedFunc()
    .name(`installUnityHub`)
    .namespace(_1.$namespace)
    .errorTypes([
    unity_hub_is_already_installed_1.UnityHubIsAlreadyInstalled,
])
    .async()
    .executeType().installUnityHubDef;
//# sourceMappingURL=install-unity-hub-def.js.map