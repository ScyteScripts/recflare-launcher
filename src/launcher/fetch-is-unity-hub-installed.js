"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsUnityHubInstalled = void 0;
const _1 = require("./$");
const fetch_unity_hub_path_1 = require("./fetch-unity-hub-path");
exports.fetchIsUnityHubInstalled = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchIsUnityHubInstalledDef)
    .execute(async () => {
    const unityHubPath = await (0, fetch_unity_hub_path_1.fetchUnityHubPath)();
    return unityHubPath !== undefined;
}).fetchIsUnityHubInstalled;
//# sourceMappingURL=fetch-is-unity-hub-installed.js.map