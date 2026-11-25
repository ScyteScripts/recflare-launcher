"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchIsAppBranchPasswordValid = void 0;
const _1 = require("./$");
const retrieve_app_branch_info_1 = require("./retrieve-app-branch-info");
exports.fetchIsAppBranchPasswordValid = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchIsAppBranchPasswordValidDef)
    .execute(async ({ appId, appAccessToken, appBranchId, appBranchPassword, }) => {
    const appBranchInfo = await (0, retrieve_app_branch_info_1.retrieveAppBranchInfo)({
        appId,
        appAccessToken,
        appBranchId,
    });
    // TODO: Use different endpoint for checking password validity
    const requestResponse = await _1.__.$crossfetch.fetch(`${_1.__.PatchKit.MAIN_SERVICE_API_URL.value}/1/apps/${appBranchInfo.secret}/versions/1/content_urls?password=${appBranchPassword}`);
    if (requestResponse.status === 200) {
        return true;
    }
    return false;
}).fetchIsAppBranchPasswordValid;
//# sourceMappingURL=fetch-is-app-branch-password-valid.js.map