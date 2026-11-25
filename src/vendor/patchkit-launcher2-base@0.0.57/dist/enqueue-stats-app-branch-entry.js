"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.enqueueStatsAppBranchEntry = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const enqueue_stats_entry_1 = require("./enqueue-stats-entry");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
exports.enqueueStatsAppBranchEntry = _1.__.declareManagedFunc()
    .name(`enqueueStatsAppBranchEntry`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ statsAppBranchEntryInfo, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const appBranchSecret = await _1.__.invokeAsyncExp(async () => {
        try {
            const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
                appId: statsAppBranchEntryInfo.appId,
                appBranchId: statsAppBranchEntryInfo.appBranchId,
            });
            return appBranchInfo.secret;
        }
        catch (_a) {
            return undefined;
        }
    });
    const { statsEntryTypeId, statsEntryExtraPayload, } = _1.__.invokeSyncExp(() => {
        switch (statsAppBranchEntryInfo.type) {
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUpdateTaskStartedEntry: {
                return {
                    statsEntryTypeId: `mp_app_update_started`,
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUpdateTaskSucceededEntry: {
                return {
                    statsEntryTypeId: `mp_app_update_succeeded`,
                    statsEntryExtraPayload: {
                        time: Math.ceil(statsAppBranchEntryInfo.appBranchUpdateTaskDurationSeconds),
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUpdateTaskFailedEntry: {
                if (statsAppBranchEntryInfo.appBranchUpdateTaskErrorTypeName === _1._.AppBranchUpdateTaskHasBeenCancelled.name) {
                    return {
                        statsEntryTypeId: `mp_app_update_cancelled`,
                    };
                }
                return {
                    statsEntryTypeId: `mp_app_update_failed`,
                    statsEntryExtraPayload: {
                        reason: statsAppBranchEntryInfo.appBranchUpdateTaskErrorTypeName,
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchRepairTaskStartedEntry: {
                return {
                    statsEntryTypeId: `mp_app_repair_started`,
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchRepairTaskSucceededEntry: {
                return {
                    statsEntryTypeId: `mp_app_repair_succeeded`,
                    statsEntryExtraPayload: {
                        time: Math.ceil(statsAppBranchEntryInfo.appBranchRepairTaskDurationSeconds),
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchRepairTaskFailedEntry: {
                if (statsAppBranchEntryInfo.appBranchRepairTaskErrorTypeName === _1._.AppBranchRepairTaskHasBeenCancelled.name) {
                    return {
                        statsEntryTypeId: `mp_app_repair_cancelled`,
                    };
                }
                return {
                    statsEntryTypeId: `mp_app_repair_failed`,
                    statsEntryExtraPayload: {
                        reason: statsAppBranchEntryInfo.appBranchRepairTaskErrorTypeName,
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchMoveTaskStartedEntry: {
                return {
                    statsEntryTypeId: `mp_app_move_started`,
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchMoveTaskSucceededEntry: {
                return {
                    statsEntryTypeId: `mp_app_move_succeeded`,
                    statsEntryExtraPayload: {
                        time: Math.ceil(statsAppBranchEntryInfo.appBranchMoveTaskDurationSeconds),
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchMoveTaskFailedEntry: {
                return {
                    statsEntryTypeId: `mp_app_move_failed`,
                    statsEntryExtraPayload: {
                        reason: statsAppBranchEntryInfo.appBranchMoveTaskErrorTypeName,
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUninstallTaskStartedEntry: {
                return {
                    statsEntryTypeId: `mp_app_uninstall_started`,
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUninstallTaskSucceededEntry: {
                return {
                    statsEntryTypeId: `mp_app_uninstall_succeeded`,
                    statsEntryExtraPayload: {
                        time: Math.ceil(statsAppBranchEntryInfo.appBranchUninstallTaskDurationSeconds),
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchUninstallTaskFailedEntry: {
                return {
                    statsEntryTypeId: `mp_app_uninstall_failed`,
                    statsEntryExtraPayload: {
                        reason: statsAppBranchEntryInfo.appBranchUninstallTaskErrorTypeName,
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchStartTaskStartedEntry: {
                return {
                    statsEntryTypeId: `mp_app_start_started`,
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchStartTaskSucceededEntry: {
                return {
                    statsEntryTypeId: `mp_app_start_succeeded`,
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchStartTaskFailedEntry: {
                return {
                    statsEntryTypeId: `mp_app_start_failed`,
                    statsEntryExtraPayload: {
                        reason: statsAppBranchEntryInfo.appBranchStartTaskErrorTypeName,
                    },
                };
            }
            case stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchProcessExitedEntry: {
                return {
                    statsEntryTypeId: `mp_app_process_exited`,
                    statsEntryExtraPayload: {
                        time: Math.ceil(statsAppBranchEntryInfo.appBranchProcessDurationSeconds),
                    },
                };
            }
        }
    });
    (0, enqueue_stats_entry_1.enqueueStatsEntry)({
        statsEntryTypeId,
        statsEntryPayload: {
            apps_catalog_id: config_1.CONFIG.value.appsCatalogId,
            apps_catalog_app_id: statsAppBranchEntryInfo.appId,
            apps_catalog_app_branch_id: statsAppBranchEntryInfo.appBranchId === _1._.APP_MAIN_BRANCH_ID.value
                ? undefined
                : statsAppBranchEntryInfo.appBranchId,
            app_secret: appBranchSecret,
            ...statsEntryExtraPayload,
        },
    });
}).enqueueStatsAppBranchEntry;
//# sourceMappingURL=enqueue-stats-app-branch-entry.js.map