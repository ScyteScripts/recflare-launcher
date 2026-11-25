"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchUpdateTaskDownloadingTotalBytesEstimatedCount = void 0;
const _1 = require("./$");
const fetch_app_branch_info_1 = require("./fetch-app-branch-info");
const fetch_app_branch_latest_version_id_1 = require("./fetch-app-branch-latest-version-id");
const retrieve_app_branch_state_1 = require("./retrieve-app-branch-state");
const retrieve_is_app_branch_registered_1 = require("./retrieve-is-app-branch-registered");
exports.fetchAppBranchUpdateTaskDownloadingTotalBytesEstimatedCount = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchUpdateTaskDownloadingTotalBytesEstimatedCountDef)
    .execute(async ({ appId, appAccessToken, appBranchId, }) => {
    const appBranchInfo = await (0, fetch_app_branch_info_1.fetchAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
    const appBranchLatestVersionId = await (0, fetch_app_branch_latest_version_id_1.fetchAppBranchLatestVersionId)({
        appId,
        appAccessToken,
        appBranchId,
    });
    if (await (0, retrieve_is_app_branch_registered_1.retrieveIsAppBranchRegistered)({ appId, appBranchId })) {
        const appBranchState = await (0, retrieve_app_branch_state_1.retrieveAppBranchState)({ appId, appBranchId });
        if (appBranchState.installedVersionId !== undefined) {
            let appBranchUpdateTaskDownloadingTotalBytesEstimatedCount = 0;
            for (let appBranchVersionId = appBranchState.installedVersionId + 1; appBranchVersionId <= appBranchLatestVersionId; appBranchVersionId++) {
                const resource1InfoAsJson = await _1.__.PatchKit.fetchMainServiceResourceInfoAsJson({
                    mainServiceResourcePath: `1/apps/${appBranchInfo.secret}/versions/${appBranchVersionId}/diff_summary`,
                });
                const resource1Info = _1.__.deserializeValueFromJson({
                    json: resource1InfoAsJson,
                });
                appBranchUpdateTaskDownloadingTotalBytesEstimatedCount += resource1Info.size;
            }
            return appBranchUpdateTaskDownloadingTotalBytesEstimatedCount;
        }
    }
    const resource2InfoAsJson = await _1.__.PatchKit.fetchMainServiceResourceInfoAsJson({
        mainServiceResourcePath: `1/apps/${appBranchInfo.secret}/versions/${appBranchLatestVersionId}/content_summary`,
    });
    const resource2Info = _1.__.deserializeValueFromJson({
        json: resource2InfoAsJson,
    });
    return resource2Info.size;
}).fetchAppBranchUpdateTaskDownloadingTotalBytesEstimatedCount;
//# sourceMappingURL=fetch-app-branch-update-task-downloading-total-bytes-estimated-count.js.map