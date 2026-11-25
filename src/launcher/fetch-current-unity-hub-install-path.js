"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchCurrentUnityHubInstallPath = void 0;
const _1 = require("./$");
const fetch_unity_hub_exe_path_1 = require("./fetch-unity-hub-exe-path");
exports.fetchCurrentUnityHubInstallPath = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchCurrentUnityHubInstallPathDef)
    .execute(async () => {
    const unity = require('../platform/unity.cjs');
    const localRoot = unity.installRoot();
    if (_1.__.$fs.existsSync(localRoot)) return localRoot;
    const unityHubExePath = await (0, fetch_unity_hub_exe_path_1.fetchUnityHubExePath)();
    if (unityHubExePath === undefined) {
        throw new _1._.UnityHubIsNotInstalled();
    }
    try {
      const output = await unity.query(_1.__.$childprocess.spawn, unityHubExePath, unity.hubArgs(['install-path', '--get']), 5000);
      return output.trim() || localRoot;
    } catch {return localRoot;}
}).fetchCurrentUnityHubInstallPath;
//# sourceMappingURL=fetch-current-unity-hub-install-path.js.map
