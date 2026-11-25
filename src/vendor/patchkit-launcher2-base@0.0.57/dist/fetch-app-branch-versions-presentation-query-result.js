"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchVersionsPresentationQueryResult = void 0;
const _1 = require("./$");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
exports.fetchAppBranchVersionsPresentationQueryResult = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchVersionsPresentationQueryResultDef)
    .execute(async ({ appId, appAccessToken, appBranchId, }) => {
    const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
    const appBranchVersionsPresentation = await _1.__.PatchKit.fetchAppVersionsPresentation({
        appSecret: appBranchInfo.secret,
    });
    return {
        versionsPresentation: appBranchVersionsPresentation,
    };
}).fetchAppBranchVersionsPresentationQueryResult;
//# sourceMappingURL=fetch-app-branch-versions-presentation-query-result.js.map