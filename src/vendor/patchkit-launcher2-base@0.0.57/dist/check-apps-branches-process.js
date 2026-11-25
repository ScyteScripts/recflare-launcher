"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.checkAppBranchesProcess = void 0;
const _1 = require("./$");
const apps_branches_process_info_box_1 = require("./apps-branches-process-info-box");
const enqueue_stats_app_branch_entry_1 = require("./enqueue-stats-app-branch-entry");
const stats_app_branch_entry_type_1 = require("./stats-app-branch-entry-type");
exports.checkAppBranchesProcess = _1.__.declareManagedFunc()
    .name(`checkAppBranchesProcess`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    for (const appId of Object.keys(apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value)) {
        for (const appBranchId of Object.keys(apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId])) {
            const appBranchProcessInfoBox = apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId][appBranchId];
            if (appBranchProcessInfoBox.value === undefined) {
                continue;
            }
            const appBranchProcessInfo = appBranchProcessInfoBox.value;
            if (!_1.__.$isrunning(appBranchProcessInfo.id)) {
                const appBranchProcessDurationSeconds = (new Date().getTime()
                    -
                        appBranchProcessInfo.startedDate.getTime())
                    /
                        1000;
                appBranchProcessInfoBox.value = undefined;
                try {
                    await (0, enqueue_stats_app_branch_entry_1.enqueueStatsAppBranchEntry)({
                        statsAppBranchEntryInfo: {
                            type: stats_app_branch_entry_type_1.StatsAppBranchEntryType.StatsAppBranchProcessExitedEntry,
                            appId,
                            appBranchId,
                            appBranchProcessDurationSeconds,
                        },
                    });
                }
                catch (_a) {
                }
            }
        }
    }
}).checkAppBranchesProcess;
exports.checkAppBranchesProcess.config.isLoggingEnabled = false;
//# sourceMappingURL=check-apps-branches-process.js.map