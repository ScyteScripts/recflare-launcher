"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchCurrentUnityHubInstallPathDef = void 0;
const _1 = require("./$");
const unity_hub_is_not_installed_1 = require("./unity-hub-is-not-installed");
exports.fetchCurrentUnityHubInstallPathDef = _1.__.defineManagedFunc()
    .name(`fetchCurrentUnityHubInstallPath`)
    .namespace(_1.$namespace)
    .errorTypes([
    unity_hub_is_not_installed_1.UnityHubIsNotInstalled,
])
    .async()
    .executeType().fetchCurrentUnityHubInstallPathDef;
//# sourceMappingURL=fetch-current-unity-hub-install-path-def.js.map