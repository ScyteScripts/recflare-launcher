"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startInstallUnityEditorTask = void 0;
const _1 = require("./$");
const fetch_unity_hub_exe_path_1 = require("./fetch-unity-hub-exe-path");
const make_sure_that_unity_hub_process_doesnt_exist_1 = require("./make-sure-that-unity-hub-process-doesnt-exist");
const ongoing_install_unity_editor_task_state_1 = require("./ongoing-install-unity-editor-task-state");
exports.startInstallUnityEditorTask = _1.__.declareManagedFunc()
    .async()
    .def(_1._.startInstallUnityEditorTaskDef)
    .execute(async ({ unityEditorId, installPath }) => {
    if (ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value !== undefined
        &&
            ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.isFinished === false) {
        throw new _1._.OngoingInstallUnityEditorTaskAlreadyExists();
    }
    ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value = {
        processOutput: ``,
        isFinished: false,
    };
    (async () => {
        try {
            const unityHubExePath = await (0, fetch_unity_hub_exe_path_1.fetchUnityHubExePath)();
            if (unityHubExePath === undefined) {
                throw new _1._.UnityHubIsNotInstalled();
            }
            await (0, make_sure_that_unity_hub_process_doesnt_exist_1.makeSureThatUnityHubProcessDoesntExist)();
            if (installPath !== undefined) {
                await new Promise((resolvePromise, rejectPromise) => {
                    try {
                        _1.__.$childprocess.spawn(unityHubExePath, [
                            `--`,
                            `--headless`,
                            `install-path`,
                            `--set`,
                            installPath,
                        ], {
                            stdio: `pipe`,
                        }).on(`exit`, resolvePromise);
                    }
                    catch (e) {
                        rejectPromise(e);
                    }
                });
            }
            await new Promise((resolvePromise, rejectPromise) => {
                try {
                    let processHandle = _1.__.$childprocess.spawn(unityHubExePath, [
                        `--`,
                        `--headless`,
                        `install`,
                        `--version`,
                        unityEditorId.version,
                        `--changeset`,
                        unityEditorId.changeset
                    ], {
                        stdio: `pipe`,
                    });
                    processHandle.stdout.on(`data`, (chunk) => {
                        _1.__.checkAssertion(ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value !== undefined);
                        // string format looks like [2A [0JProgress: [Unity 2021.3.30f1] downloading X%
                        const versionString = `[Unity ${unityEditorId.version}]`;
                        let dataChunk = chunk.toString();
                        let versionIndex = dataChunk.indexOf(versionString);
                        // sometimes two lines get added to one chunk. In that case, just report the second one
                        const secondVersionIndex = dataChunk.indexOf(versionString, versionIndex + 1);
                        if (secondVersionIndex !== -1) {
                            versionIndex = secondVersionIndex;
                        }
                        if (versionIndex !== -1) {
                            dataChunk = dataChunk.substring(versionIndex + versionString.length + 1);
                        }
                        ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.processOutput = dataChunk;
                    });
                    processHandle.on(`exit`, resolvePromise);
                }
                catch (e) {
                    rejectPromise(e);
                }
            });
            await (0, make_sure_that_unity_hub_process_doesnt_exist_1.makeSureThatUnityHubProcessDoesntExist)();
            _1.__.checkAssertion(ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value !== undefined);
            ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.isFinished = true;
        }
        catch (e) {
            _1.__.checkAssertion(ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value !== undefined);
            if (e instanceof _1._.UnityHubIsNotInstalled) {
                ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.errorTypeName = _1._.UnityHubIsNotInstalled.name;
            }
            else if (e instanceof _1._.UnityEditorIsAlreadyInstalled) {
                ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.errorTypeName = _1._.UnityEditorIsAlreadyInstalled.name;
            }
            else {
                ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.errorTypeName = _1.__.FatalError.name;
            }
        }
        finally {
            _1.__.checkAssertion(ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value !== undefined);
            ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value.isFinished = true;
        }
    })();
}).startInstallUnityEditorTask;
//# sourceMappingURL=start-install-unity-editor-task.js.map