"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.makeSureThatUnityHubProcessDoesntExist = void 0;
const _1 = require("./$");
const fetch_does_unity_hub_process_exist_1 = require("./fetch-does-unity-hub-process-exist");
const kill_unity_hub_processes_1 = require("./kill-unity-hub-processes");
const unity_hub_process_exists_1 = require("./unity-hub-process-exists");
exports.makeSureThatUnityHubProcessDoesntExist = _1.__.declareManagedFunc()
    .name(`makeSureThatUnityHubProcessDoesntExist`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    if (await (0, fetch_does_unity_hub_process_exist_1.fetchDoesUnityHubProcessExist)()) {
            const { dialogSelectedOptionId, } = await _1.__.PatchKitLauncher2.displayDialog({
                dialogConfig: {
                    title: `Unity Hub is running`,
                    message: `Please close Unity Hub to continue the current operation.`,
                    type: _1.__.PatchKitLauncher2.DialogType.WarningDialog,
                },
                dialogOptionsInfo: {
                    [`ok`]: {
                        label: `OK`,
                    },
                    [`force_quit`]: {
                        label: `Force Quit`,
                    },
                    [`cancel`]: {
                        label: `Cancel`,
                    },
                },
            });
            if (dialogSelectedOptionId === `cancel`) {
                const failure = new unity_hub_process_exists_1.UnityHubProcessExists();
                failure.message = 'Operation cancelled because Unity Hub is running.';
                throw failure;
            }
            else if (dialogSelectedOptionId === `force_quit`) {
                await (0, kill_unity_hub_processes_1.killUnityHubProcesses)();
            }
        // Give Hub's helper processes time to exit, without reopening the dialog.
        for (let attempt=0; attempt<10; attempt++) {
            if (!await (0, fetch_does_unity_hub_process_exist_1.fetchDoesUnityHubProcessExist)()) return;
            await _1.__.wait(250);
        }
        const failure = new _1.__.FatalError();
        failure.message = 'Unity Hub is still running. Close it from the system tray or Task Manager, then try again.';
        throw failure;
    }
}).makeSureThatUnityHubProcessDoesntExist;
//# sourceMappingURL=make-sure-that-unity-hub-process-doesnt-exist.js.map
