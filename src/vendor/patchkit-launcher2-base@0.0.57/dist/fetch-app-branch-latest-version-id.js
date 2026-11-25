"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchLatestVersionId = void 0;
const _1 = require("./$");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
exports.fetchAppBranchLatestVersionId = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchLatestVersionIdDef)
    .execute(async ({ appId, appAccessToken, appBranchId, }) => {
    const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
    const appBranchLatestVersionId = await _1.__.PatchKit.fetchAppLatestVersionId({
        appSecret: appBranchInfo.secret,
    });
    return appBranchLatestVersionId;
}).fetchAppBranchLatestVersionId;
//# sourceMappingURL=fetch-app-branch-latest-version-id.js.map