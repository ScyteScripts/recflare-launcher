"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.installUnityHub = void 0;
// eslint-disable-next-line @typescript-eslint/tslint/config
const $electrondl = require("electron-dl");
const _1 = require("./$");
const fetch_is_unity_hub_installed_1 = require("./fetch-is-unity-hub-installed");
exports.installUnityHub = _1.__.declareManagedFunc()
    .async()
    .def(_1._.installUnityHubDef)
    .execute(async () => {
    const wasUnityHubInstalled = await (0, fetch_is_unity_hub_installed_1.fetchIsUnityHubInstalled)({});
    if (wasUnityHubInstalled) {
        throw new _1._.UnityHubIsAlreadyInstalled();
    }
    const windowHandle = _1.__.PatchKitLauncher2.WINDOW_HANDLE.value;
    _1.__.checkAssertion(windowHandle !== undefined);
    const url = 'https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup-x64.exe';
    console.log({ kind: 'unity-hub-download-started', url });
    const electronDlDownloadItemHandle = await $electrondl.download(windowHandle, url, {
        showBadge: false,
        showProgressBar: false,
    });
    const installerPath = electronDlDownloadItemHandle.getSavePath();
    console.log({ kind: 'unity-hub-installer-started', installerPath });
    // ShellExecute prompts for the elevation required by the Hub installer.
    // Keep the path in an environment variable, never interpolate it into code.
    const powershell = _1.__.$path.join(process.env.SystemRoot || 'C:\\Windows',
        'System32', 'WindowsPowerShell', 'v1.0', 'powershell.exe');
    // The installer only runs if Windows says it is validly signed by Unity Technologies.
    const command = '$ErrorActionPreference = "Stop"; try { '
        + '$signature = Get-AuthenticodeSignature -LiteralPath $env:STUDIO_UNITY_HUB_INSTALLER; '
        + 'if ($signature.Status -ne "Valid" -or $signature.SignerCertificate.Subject -notmatch "O=Unity Technologies") '
        + '{ [Console]::Error.WriteLine("The Unity Hub installer is not signed by Unity Technologies."); exit 2 }; '
        + '$installer = Start-Process -FilePath $env:STUDIO_UNITY_HUB_INSTALLER -Verb RunAs -Wait -PassThru; '
        + 'exit $installer.ExitCode } catch { [Console]::Error.WriteLine($_.Exception.Message); exit 1 }';
    await new Promise((resolvePromise, rejectPromise) => {
        _1.__.$childprocess.execFile(powershell, ['-NoProfile', '-NonInteractive', '-Command', command], {
            windowsHide: true,
            env: { ...process.env, STUDIO_UNITY_HUB_INSTALLER: installerPath },
        }, (error, stdout, stderr) => {
            if (error) {
                const message = `Unity Hub installer failed (${error.code}): ${String(stderr || error.message).trim()}`;
                console.error({ kind: 'unity-hub-installer-failed', installerPath, message });
                rejectPromise(new Error(message));
            } else {
                console.log({ kind: 'unity-hub-installer-completed', installerPath });
                resolvePromise();
            }
        });
    });
}).installUnityHub;
//# sourceMappingURL=install-unity-hub.js.map
