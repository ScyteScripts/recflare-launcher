"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUnityEditorExePath = void 0;
const _1 = require("./$");
const fetch_unity_hub_exe_path_1 = require("./fetch-unity-hub-exe-path");
exports.fetchUnityEditorExePath = _1.__.declareManagedFunc()
    .name(`fetchUnityEditorExePath`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.UnityHubIsNotInstalled,
])
    .async()
    .execute(async ({ unityEditorId, }) => {
    const unity = require('../platform/unity.cjs');
    const installed = unity.findInstalledEditor(unityEditorId.version);
    if (installed) return installed;
    const unityHubExePath = await (0, fetch_unity_hub_exe_path_1.fetchUnityHubExePath)();
    if (unityHubExePath === undefined) {
        throw new _1._.UnityHubIsNotInstalled();
    }
    const unityHubExeInvokeResult = await unity.query(_1.__.$childprocess.spawn, unityHubExePath, unity.hubArgs(['editors', '--installed']));
    const match = unityHubExeInvokeResult.split(/\r?\n/)
        .map(line => /^\s*([^\s,]+)(?:\s+\([^)]*\))?\s*,?\s+installed at\s+(.+)$/.exec(line))
        .find(row => row && row[1] === unityEditorId.version);
    const unityEditorExePath = match ? unity.editorExecutable(match[2].trim()) : undefined;
    return unityEditorExePath;
}).fetchUnityEditorExePath;
//# sourceMappingURL=fetch-unity-editor-exe-path.js.map
