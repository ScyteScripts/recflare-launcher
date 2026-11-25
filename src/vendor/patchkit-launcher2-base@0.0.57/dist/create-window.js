"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createWindow = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const get_asset_path_1 = require("./get-asset-path");
const get_window_preload_script_1 = require("./get-window-preload-script");
const get_window_preload_script_file_path_1 = require("./get-window-preload-script-file-path");
const on_debug_shortcut_invoked_1 = require("./on-debug-shortcut-invoked");
const show_window_1 = require("./show-window");
const start_quit_task_1 = require("./start-quit-task");
const tray_handle_1 = require("./tray-handle");
const window_handle_1 = require("./window-handle");
exports.createWindow = _1.__.declareManagedFunc()
    .name(`createWindow`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    var _a, _b, _c, _d, _e, _f, _g, _h;
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    _1.__.checkAssertion(window_handle_1.WINDOW_HANDLE.value === undefined);
    const windowPreloadScript = (0, get_window_preload_script_1.getWindowPreloadScript)();
    const windowPreloadScriptFilePath = (0, get_window_preload_script_file_path_1.getWindowPreloadScriptFilePath)();
    await _1.__.$fspromises.mkdir(_1.__.$path.dirname(windowPreloadScriptFilePath), {
        recursive: true,
    });
    await _1.__.$fspromises.writeFile(windowPreloadScriptFilePath, windowPreloadScript, `utf8`);
    const themeUrl = config_1.CONFIG.value.overrideThemeUrl !== undefined
        ? config_1.CONFIG.value.overrideThemeUrl
        : require("url").pathToFileURL(require("path").join(__dirname, "theme", "index.html")).href;
    window_handle_1.WINDOW_HANDLE.value = new _1.__.$electron.BrowserWindow({
        width: config_1.CONFIG.value.window.defaultSize.width,
        height: config_1.CONFIG.value.window.defaultSize.height,
        minWidth: (_b = (_a = config_1.CONFIG.value.window.defaultMinSize) === null || _a === void 0 ? void 0 : _a.width) !== null && _b !== void 0 ? _b : 0,
        minHeight: (_d = (_c = config_1.CONFIG.value.window.defaultMinSize) === null || _c === void 0 ? void 0 : _c.height) !== null && _d !== void 0 ? _d : 0,
        maxWidth: (_f = (_e = config_1.CONFIG.value.window.defaultMaxSize) === null || _e === void 0 ? void 0 : _e.width) !== null && _f !== void 0 ? _f : 2147483647,
        maxHeight: (_h = (_g = config_1.CONFIG.value.window.defaultMaxSize) === null || _g === void 0 ? void 0 : _g.height) !== null && _h !== void 0 ? _h : 2147483647,
        resizable: config_1.CONFIG.value.window.defaultIsResizable,
        frame: !config_1.CONFIG.value.window.isBorderless,
        transparent: config_1.CONFIG.value.window.isTransparent,
        roundedCorners: config_1.CONFIG.value.window.areCornersRounded,
        center: true,
        fullscreenable: false,
        icon: (0, get_asset_path_1.getAssetPath)({
            assetId: config_1.CONFIG.value.window.iconFileAssetId,
        }),
        webPreferences: {
            spellcheck: false,
            preload: windowPreloadScriptFilePath,
            sandbox: true,
            contextIsolation: true,
            devTools: true,
            nodeIntegrationInSubFrames: true,
        },
    });
    window_handle_1.WINDOW_HANDLE.value.webContents.setWindowOpenHandler((electronHandlerDetails) => {
        // eslint-disable-next-line @typescript-eslint/tslint/config
        _1.__.$electron.shell.openExternal(require("@recovered/network").rewriteUrl(electronHandlerDetails.url));
        return {
            action: `deny`,
        };
    });
    window_handle_1.WINDOW_HANDLE.value.webContents.on(`will-navigate`, async (electronEvent, windowPendingPageUrl) => {
        if (!windowPendingPageUrl.startsWith(themeUrl)) {
            electronEvent.preventDefault();
            await _1.__.$electron.shell.openExternal(require("@recovered/network").rewriteUrl(windowPendingPageUrl));
        }
    });
    window_handle_1.WINDOW_HANDLE.value.webContents.on(`console-message`, (_electronEvent, electronWindowConsoleMessageLevel, electronWindowConsoleMessage) => {
        // Modern Electron supplies a details object; retain older-runtime support.
        if (typeof electronWindowConsoleMessageLevel === 'object') {
            electronWindowConsoleMessage = electronWindowConsoleMessageLevel.message;
            electronWindowConsoleMessageLevel = electronWindowConsoleMessageLevel.level;
        }
        _1.__.writeTraceLog({
            kind: `on-patchkit-launcher2-window-console-message-log-msg`,
            windowConsoleMessage: electronWindowConsoleMessage,
            windowConsoleMessageLevel: electronWindowConsoleMessageLevel,
        });
    });
    // HACK: https://github.com/electron/electron/issues/34727
    window_handle_1.WINDOW_HANDLE.value.webContents.on(`frame-created`, (_electronEvent, electronEventDetails) => {
        if (electronEventDetails.frame.parent === null) {
            return;
        }
        electronEventDetails.frame.once(`dom-ready`, async () => {
            if (typeof await electronEventDetails.frame.executeJavaScript(`window.patchKitLauncher2Id`) !== `string`) {
                electronEventDetails.frame.reload();
            }
        });
    });
    if (config_1.CONFIG.value.mode === _1._.Mode.Dev) {
        window_handle_1.WINDOW_HANDLE.value.webContents.openDevTools({
            mode: `detach`,
        });
    }
    if (config_1.CONFIG.value.tray !== undefined) {
        const trayElectronImageHandle = _1.__.$electron.nativeImage.createFromPath((0, get_asset_path_1.getAssetPath)({
            assetId: config_1.CONFIG.value.window.iconFileAssetId,
        }));
        if (config_1.CONFIG.value.targetOperatingSystemPlatform === _1._.TargetOperatingSystemPlatform.MacOS) {
            trayElectronImageHandle.resize({ width: 20 });
        }
        tray_handle_1.TRAY_HANDLE.value = new _1.__.$electron.Tray(trayElectronImageHandle);
        tray_handle_1.TRAY_HANDLE.value.setContextMenu(_1.__.$electron.Menu.buildFromTemplate([
            {
                label: `Show`,
                click: async () => {
                    await (0, show_window_1.showWindow)();
                },
            },
            {
                label: `Quit`,
                click: async () => {
                    await (0, start_quit_task_1.startQuitTask)();
                },
            },
        ]));
        if (config_1.CONFIG.value.targetOperatingSystemPlatform === _1._.TargetOperatingSystemPlatform.Windows) {
            tray_handle_1.TRAY_HANDLE.value.on(`click`, async () => {
                await (0, show_window_1.showWindow)();
            });
        }
    }
    _1.__.$electronlocalshortcut.register(window_handle_1.WINDOW_HANDLE.value, `Ctrl+Shift+D`, on_debug_shortcut_invoked_1.onDebugShortcutInvoked);
    await window_handle_1.WINDOW_HANDLE.value.loadURL(themeUrl);
}).createWindow;
//# sourceMappingURL=create-window.js.map
