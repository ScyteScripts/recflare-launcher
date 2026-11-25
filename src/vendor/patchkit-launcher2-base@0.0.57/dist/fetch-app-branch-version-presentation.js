"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchVersionPresentation = void 0;
const _1 = require("./$");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
exports.fetchAppBranchVersionPresentation = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchAppBranchVersionPresentationDef)
    .execute(async ({ appId, appAccessToken, appBranchId, appBranchVersionId, }) => {
    const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
    const appBranchVersionPresentation = await _1.__.PatchKit.fetchAppVersionPresentation({
        appSecret: appBranchInfo.secret,
        appVersionId: appBranchVersionId,
    });
    return appBranchVersionPresentation;
}).fetchAppBranchVersionPresentation;
//# sourceMappingURL=fetch-app-branch-version-presentation.js.map