"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/* eslint-disable @typescript-eslint/tslint/config */
const _1 = require("./$");
const create_unity_project_from_recroom_studio_template_1 = require("./create-unity-project-from-recroom-studio-template");
const create_winreg_handle_1 = require("./create-winreg-handle");
const display_select_dir_dialog_1 = require("./display-select-dir-dialog");
const fetch_recroom_studio_default_branch_id_1 = require("./fetch-recroom-studio-default-branch-id");
const fetch_current_unity_hub_install_path_1 = require("./fetch-current-unity-hub-install-path");
const fetch_is_unity_editor_installed_1 = require("./fetch-is-unity-editor-installed");
const fetch_is_unity_editor_install_broken_1 = require("./fetch-is-unity-editor-install-broken");
const fetch_is_unity_hub_install_broken_1 = require("./fetch-is-unity-hub-install-broken");
const fetch_is_unity_hub_installed_1 = require("./fetch-is-unity-hub-installed");
const fetch_ongoing_install_unity_editor_task_state_1 = require("./fetch-ongoing-install-unity-editor-task-state");
const fetch_recroom_branch_last_studio_unity_project_dir_path_1 = require("./fetch-recroom-branch-last-studio-unity-project-dir-path");
const fetch_recroom_studio_template_unity_editor_id_1 = require("./fetch-recroom-studio-template-unity-editor-id");
const fetch_unity_project_unity_editor_id_1 = require("./fetch-unity-project-unity-editor-id");
const fetch_unity_uninstaller_location_1 = require("./fetch-unity-uninstaller-location");
const start_install_unity_editor_task_1 = require("./start-install-unity-editor-task");
const install_unity_hub_1 = require("./install-unity-hub");
const open_unity_project_1 = require("./open-unity-project");
const remove_unity_hub_reg_key_1 = require("./remove-unity-hub-reg-key");
const uninstall_unity_editor_1 = require("./uninstall-unity-editor");
const get_disk_space_for_path_1 = require("./get-disk-space-for-path");
// @ts-ignore
const $electronRegedit = require("electron-regedit");
// eslint-disable-next-line @typescript-eslint/tslint/config
const $winreg = require("winreg");
const update_win_reg_app_branches_info_1 = require("./update-win-reg-app-branches-info");
const recroom_studio_default_branch_id_1 = require("./recroom-studio-default-branch-id");
const fetch_pending_check_for_recroom_updates_command_info_1 = require("./fetch-pending-check-for-recroom-updates-command-info");
const save_pending_check_for_recroom_updates_command_info_file_store_1 = require("./save-pending-check-for-recroom-updates-command-info-file-store");
(async () => {
    await _1.__.PatchKitLauncher2.initializeCoreSystems();
    // Scan unknown projects asynchronously; opening a selected project also checks
    // it synchronously so it cannot race this background discovery pass.
    const routing = require('@recovered/routing-repair');
    routing.repairLauncherRouting({
        appId: routing.STUDIO_APP_ID,
        configName: _1.__.PatchKitLauncher2.CONFIG.value.name,
        developmentOnly: true,
        ...(process.env.STUDIO_LAUNCHER_VALIDATION_ROOT ? {registeredProjects: [], searchRoots: [process.env.STUDIO_LAUNCHER_VALIDATION_ROOT]} : {}),
    }).catch(() => _1.__.$electron.dialog.showErrorBox('Some Studio projects could not be checked', 'Close Unity and run Repair to finish checking Studio project routing.'));
    // TODO: Remove this export as soon as SDK bug is fixed
    _1.__.PatchKitLauncher2.exportApiFunc(_1.__.PatchKitLauncher2.fetchIsAppBranchPasswordValid);
    _1.__.PatchKitLauncher2.exportApiFunc(create_unity_project_from_recroom_studio_template_1.createUnityProjectFromRecRoomStudioTemplate);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_recroom_studio_default_branch_id_1.fetchRecRoomStudioDefaultBranchId);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_current_unity_hub_install_path_1.fetchCurrentUnityHubInstallPath);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_is_unity_editor_installed_1.fetchIsUnityEditorInstalled);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_is_unity_editor_install_broken_1.fetchIsUnityEditorInstallBroken);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_is_unity_hub_install_broken_1.fetchIsUnityHubInstallBroken);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_is_unity_hub_installed_1.fetchIsUnityHubInstalled);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_recroom_branch_last_studio_unity_project_dir_path_1.fetchRecRoomBranchLastStudioUnityProjectDirPath);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_unity_project_unity_editor_id_1.fetchUnityProjectUnityEditorId);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_unity_uninstaller_location_1.fetchUnityUninstallerLocation);
    _1.__.PatchKitLauncher2.exportApiFunc(start_install_unity_editor_task_1.startInstallUnityEditorTask);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_ongoing_install_unity_editor_task_state_1.fetchOngoingInstallUnityEditorTaskState);
    _1.__.PatchKitLauncher2.exportApiFunc(install_unity_hub_1.installUnityHub);
    _1.__.PatchKitLauncher2.exportApiFunc(open_unity_project_1.openUnityProject);
    _1.__.PatchKitLauncher2.exportApiFunc(fetch_recroom_studio_template_unity_editor_id_1.fetchRecRoomStudioTemplateUnityEditorId);
    _1.__.PatchKitLauncher2.exportApiFunc(display_select_dir_dialog_1.displaySelectDirDialog);
    _1.__.PatchKitLauncher2.exportApiFunc(remove_unity_hub_reg_key_1.removeUnityHubRegKey);
    _1.__.PatchKitLauncher2.exportApiFunc(uninstall_unity_editor_1.uninstallUnityEditor);
    _1.__.PatchKitLauncher2.exportApiFunc(get_disk_space_for_path_1.getDiskSpaceForPath);
    _1.__.checkAssertion(_1.__.PatchKitLauncher2.CONFIG.value !== undefined);
    if (!process.env.STUDIO_LAUNCHER_PORTABLE_ROOT) {
    try {
        const winRegHandle = await (0, create_winreg_handle_1.createWinRegHandle)();
        try {
            await new Promise((resolvePromise, rejectPromise) => {
                try {
                    winRegHandle.set(`exe_file_path`, $winreg.REG_SZ, _1.__.$electron.app.getPath(`exe`), (e) => {
                        if (e === undefined || e === null) {
                            resolvePromise();
                        }
                        else {
                            rejectPromise(e);
                        }
                    });
                }
                catch (e) {
                    rejectPromise(e);
                }
            });
        }
        catch (_a) {
        }
    }
    catch (_b) {
    }
    try {
        new $electronRegedit.ProgId({
            appName: _1.__.PatchKitLauncher2.CONFIG.value.id,
            shell: [
                new $electronRegedit.ShellOption({
                    verb: $electronRegedit.ShellOption.OPEN,
                    command: $electronRegedit.app.getPath(`exe`),
                }),
            ],
        });
        $electronRegedit.Regedit.installAll();
    }
    catch (_c) {
    }
    }
    let currentCheckForRecRoomUpdatesCommandInfo;
    try {
        const checkForRecRoomUpdatesArgIndex = process.argv.indexOf(`--check-for-recroom-updates`);
        if (checkForRecRoomUpdatesArgIndex !== -1) {
            const appBranchId = process.argv[checkForRecRoomUpdatesArgIndex + 1];
            if (appBranchId !== undefined) {
                currentCheckForRecRoomUpdatesCommandInfo = {
                    appBranchId,
                };
            }
        }
    }
    catch (_d) {
    }
    if (currentCheckForRecRoomUpdatesCommandInfo === undefined) {
        try {
            currentCheckForRecRoomUpdatesCommandInfo = await (0, fetch_pending_check_for_recroom_updates_command_info_1.fetchPendingCheckForRecRoomUpdatesCommandInfo)();
        }
        catch (_e) {
        }
    }
    recroom_studio_default_branch_id_1.REC_ROOM_STUDIO_DEFAULT_BRANCH_ID.value = currentCheckForRecRoomUpdatesCommandInfo === undefined
        ? _1.__.PatchKitLauncher2.CONFIG.value.id.includes(`partner`)
            ? `a30105bd-8abd-4919-af33-83d10d581b0f` // partner branch
            : _1.__.PatchKitLauncher2.APP_MAIN_BRANCH_ID.value
        : currentCheckForRecRoomUpdatesCommandInfo.appBranchId;
    if (currentCheckForRecRoomUpdatesCommandInfo !== undefined) {
        try {
            await (0, save_pending_check_for_recroom_updates_command_info_file_store_1.savePendingCheckForRecRoomUpdatesCommandInfoFileStore)({
                pendingCheckForRecRoomUpdatesCommandInfo: currentCheckForRecRoomUpdatesCommandInfo,
            });
            const appsInfoQueryResult = await _1.__.PatchKitLauncher2.fetchAppsInfoQueryResult({
                appsInfoQueryConfig: {},
            });
            const appId = Object.keys(appsInfoQueryResult.appsInfo)[0];
            const appBranchId = currentCheckForRecRoomUpdatesCommandInfo.appBranchId;
            if (appId !== undefined) {
                const appBranchLatestVersionId = await _1.__.PatchKitLauncher2.fetchAppBranchLatestVersionId({
                    appId,
                    appBranchId,
                });
                const interval = setInterval(async () => {
                    const appBranchState = await _1.__.PatchKitLauncher2.retrieveAppBranchState({
                        appId,
                        appBranchId,
                    });
                    if (appBranchState.installedVersionId === appBranchLatestVersionId) {
                        clearInterval(interval);
                        await (0, update_win_reg_app_branches_info_1.updateWinRegAppBranchesInfo)();
                        await (0, save_pending_check_for_recroom_updates_command_info_file_store_1.savePendingCheckForRecRoomUpdatesCommandInfoFileStore)({
                            pendingCheckForRecRoomUpdatesCommandInfo: undefined,
                        });
                        await _1.__.PatchKitLauncher2.startQuitTask();
                    }
                }, 1000);
            }
        }
        catch (_f) {
        }
    }
    if (!process.env.STUDIO_LAUNCHER_PORTABLE_ROOT) setInterval(async () => await (0, update_win_reg_app_branches_info_1.updateWinRegAppBranchesInfo)(), 5000);
})().catch();
exports.default = {};
//# sourceMappingURL=main.js.map