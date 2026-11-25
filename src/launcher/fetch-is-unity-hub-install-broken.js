"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsUnityHubInstallBroken = void 0;
const _1 = require("./$");
const fs = require("fs");
const fetch_unity_hub_exe_path_1 = require("./fetch-unity-hub-exe-path");
exports.fetchIsUnityHubInstallBroken = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchIsUnityHubInstallBrokenDef)
    .execute(async () => {
    const unityHubExePath = await (0, fetch_unity_hub_exe_path_1.fetchUnityHubExePath)();
    if (unityHubExePath === undefined) {
        return false;
    }
    else if (fs.existsSync(unityHubExePath)) {
        return false;
    }
    else {
        return true;
    }
}).fetchIsUnityHubInstallBroken;
//# sourceMappingURL=fetch-is-unity-hub-install-broken.js.map