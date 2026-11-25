"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppBranchVersionPresentationDef = void 0;
const _1 = require("./$");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_version_does_not_exist_1 = require("./app-branch-version-does-not-exist");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const app_branch_does_not_exist_1 = require("./app-branch-does-not-exist");
const app_access_token_is_invalid_1 = require("./app-access-token-is-invalid");
const app_access_token_is_expired_1 = require("./app-access-token-is-expired");
exports.fetchAppBranchVersionPresentationDef = _1.__.defineManagedFunc()
    .name(`fetchAppBranchVersionPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_access_token_is_invalid_1.AppAccessTokenIsInvalid,
    app_access_token_is_expired_1.AppAccessTokenIsExpired,
    app_branch_does_not_exist_1.AppBranchDoesNotExist,
    app_branch_version_does_not_exist_1.AppBranchVersionDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchAppBranchVersionPresentationDef;
//# sourceMappingURL=fetch-app-branch-version-presentation-def.js.map