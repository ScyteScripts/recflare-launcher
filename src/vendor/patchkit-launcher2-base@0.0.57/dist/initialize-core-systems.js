"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.initializeCoreSystems = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const create_window_1 = require("./create-window");
const export_default_api_funcs_1 = require("./export-default-api-funcs");
const get_logs_dir_path_1 = require("./get-logs-dir-path");
const initialize_electron_updater_1 = require("./initialize-electron-updater");
const check_apps_branches_process_1 = require("./check-apps-branches-process");
const show_window_1 = require("./show-window");
const focus_window_1 = require("./focus-window");
const initialize_protocol_client_1 = require("./initialize-protocol-client");
const initialize_logging_1 = require("./initialize-logging");
const enqueue_stats_general_entry_1 = require("./enqueue-stats-general-entry");
const stats_general_entry_type_1 = require("./stats-general-entry-type");
const load_config_1 = require("./load-config");
exports.initializeCoreSystems = _1.__.declareManagedFunc()
    .name(`initializeCoreSystems`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    // TODO: Finish this function.
    var _a;
    await _1.__.$electron.app.whenReady();
    if (!_1.__.$electron.app.requestSingleInstanceLock()) {
        _1.__.$electron.app.exit(1);
        return;
    }
    _1.__.$electron.Menu.setApplicationMenu(null);
    await (0, load_config_1.loadConfig)();
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    await (0, initialize_logging_1.initializeLogging)();
    if (config_1.CONFIG.value.patchKitOverrideMainServiceApiUrl !== undefined) {
        _1.__.PatchKit.MAIN_SERVICE_API_URL.value = config_1.CONFIG.value.patchKitOverrideMainServiceApiUrl;
    }
    if (config_1.CONFIG.value.patchKitOverrideMainServiceApiCacheUrl !== undefined) {
        _1.__.PatchKit.MAIN_SERVICE_API_CACHE_URL.value = config_1.CONFIG.value.patchKitOverrideMainServiceApiCacheUrl;
    }
    if (config_1.CONFIG.value.patchKitOverrideStatsServiceApiUrl !== undefined) {
        _1.__.PatchKit.STATS_SERVICE_API_URL.value = config_1.CONFIG.value.patchKitOverrideStatsServiceApiUrl;
    }
    if (config_1.CONFIG.value.patchKitOverrideKeysServiceApiUrl !== undefined) {
        _1.__.PatchKit.KEYS_SERVICE_API_URL.value = config_1.CONFIG.value.patchKitOverrideKeysServiceApiUrl;
    }
    if (config_1.CONFIG.value.patchKitOverrideAppCatalogsServiceApiUrl !== undefined) {
        _1.__.PatchKit.APP_CATALOGS_SERVICE_API_URL.value = config_1.CONFIG.value.patchKitOverrideAppCatalogsServiceApiUrl;
    }
    if (config_1.CONFIG.value.patchKitOverrideLauncherThemesServiceApiUrl !== undefined) {
        _1.__.PatchKit.LAUNCHER_THEMES_SERVICE_API_URL.value = config_1.CONFIG.value.patchKitOverrideLauncherThemesServiceApiUrl;
    }
    const overrideAppsCatalogId = process.env[`PK_LAUNCHER2_OVERRIDE_APPS_CATALOG_ID`];
    if (overrideAppsCatalogId !== undefined) {
        _1.__.$electron.dialog.showErrorBox(`PK_LAUNCHER2_OVERRIDE_APPS_CATALOG_ID is set!`, `Using ${overrideAppsCatalogId} as Apps Catalog ID.`);
        config_1.CONFIG.value.appsCatalogId = overrideAppsCatalogId;
    }
    _1.__.PatchKit.API_CLIENT_ID.value = `${config_1.CONFIG.value.id}:${config_1.CONFIG.value.version}`;
    await _1.__.PatchKit.refreshApiSenderId();
    _1.__.PatchKit.LIBPKAPPS_DIR_PATH.value = (_a = config_1.CONFIG.value.overrideLibkappsDirPath) !== null && _a !== void 0 ? _a : _1.__.$path.resolve(process.resourcesPath, `libpkapps`);
    _1.__.PatchKit.createLibpkappsBinding();
    _1.__.PatchKit.initLibpkapps({
        libpkappsLogsDirPath: _1.__.$path.join((0, get_logs_dir_path_1.getLogsDirPath)(), `libpkapps`),
    });
    setInterval(async () => {
        await (0, check_apps_branches_process_1.checkAppBranchesProcess)();
    }, 1000);
    (0, initialize_electron_updater_1.initializeElectronUpdater)();
    (0, export_default_api_funcs_1.exportDefaultApiFuncs)();
    try {await require('@recovered/routing-repair').repairInstalledDevelopmentTemplates({
        registeredApps: await require('./retrieve-registered-apps-state').retrieveRegisteredAppsState(),
        startRepair: args => require('./start-app-branch-repair-task').startAppBranchRepairTask(args),
    });} catch (error) {
        _1.__.$electron.dialog.showErrorBox('Studio template reinstall could not start', 'The development template could not be replaced. Close Unity, check your connection, and run Repair.');
    }
    await (0, create_window_1.createWindow)();
    if (!process.env.STUDIO_LAUNCHER_PORTABLE_ROOT) await (0, initialize_protocol_client_1.initializeProtocolClient)();
    _1.__.$electron.app.on(`second-instance`, async () => {
        await (0, show_window_1.showWindow)();
        await (0, focus_window_1.focusWindow)();
    });
    _1.__.$electron.app.on(`activate`, async () => {
        await (0, show_window_1.showWindow)();
        await (0, focus_window_1.focusWindow)();
    });
    let wasElectronWillQuitCalled = false;
    _1.__.$electron.app.on(`will-quit`, async (electronEvent) => {
        if (!wasElectronWillQuitCalled) {
            wasElectronWillQuitCalled = true;
            electronEvent.preventDefault();
            (0, enqueue_stats_general_entry_1.enqueueStatsGeneralEntry)({
                statsGeneralEntryInfo: {
                    type: stats_general_entry_type_1.StatsGeneralEntryType.StatsQuittedEntry,
                },
            });
            try {
                await Promise.race([
                    _1.__.PatchKit.sendStatsQueuedEntries(),
                    _1.__.wait(5000),
                ]);
            }
            finally {
                await _1.__.wait(500);
                _1.__.$electron.app.quit();
            }
        }
    });
    setInterval(async () => {
        await _1.__.PatchKit.trySendingStatsNextQueuedEntry();
    }, 1000);
    (0, enqueue_stats_general_entry_1.enqueueStatsGeneralEntry)({
        statsGeneralEntryInfo: {
            type: stats_general_entry_type_1.StatsGeneralEntryType.StatsStartedEntry,
        },
    });
}).initializeCoreSystems;
//# sourceMappingURL=initialize-core-systems.js.map
