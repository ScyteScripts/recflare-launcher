"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUnityHubExePath = void 0;
const _1 = require("./$");
const fetch_unity_hub_path_1 = require("./fetch-unity-hub-path");
exports.fetchUnityHubExePath = _1.__.declareManagedFunc()
    .name(`fetchUnityHubExePath`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const unityHubPath = await (0, fetch_unity_hub_path_1.fetchUnityHubPath)();
    if (unityHubPath === undefined) {
        return undefined;
    }
    return _1.__.$path.resolve(unityHubPath, `Unity Hub.exe`);
}).fetchUnityHubExePath;
//# sourceMappingURL=fetch-unity-hub-exe-path.js.map