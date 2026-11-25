"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchVersionsPresentationQueryResultDef = void 0;
const _1 = require("./$");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const app_branch_does_not_exist_1 = require("./app-branch-does-not-exist");
const app_access_token_is_invalid_1 = require("./app-access-token-is-invalid");
const app_access_token_is_expired_1 = require("./app-access-token-is-expired");
exports.fetchAppBranchVersionsPresentationQueryResultDef = _1.__.defineManagedFunc()
    .name(`fetchAppBranchVersionsPresentationQueryResult`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_access_token_is_invalid_1.AppAccessTokenIsInvalid,
    app_access_token_is_expired_1.AppAccessTokenIsExpired,
    app_branch_does_not_exist_1.AppBranchDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchAppBranchVersionsPresentationQueryResultDef;
//# sourceMappingURL=fetch-app-branch-versions-presentation-query-result-def.js.map