"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchUpdateTaskDef = void 0;
const _1 = require("./$");
const app_branch_is_not_registered_1 = require("./app-branch-is-not-registered");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_root_dir_permissions_are_insufficient_1 = require("./app-branch-root-dir-permissions-are-insufficient");
const app_branch_is_preinstalled_1 = require("./app-branch-is-preinstalled");
const app_branch_ongoing_task_exists_1 = require("./app-branch-ongoing-task-exists");
const app_branch_needs_repairing_1 = require("./app-branch-needs-repairing");
const app_access_token_is_invalid_1 = require("./app-access-token-is-invalid");
const app_access_token_is_expired_1 = require("./app-access-token-is-expired");
const app_branch_does_not_exist_1 = require("./app-branch-does-not-exist");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.startAppBranchUpdateTaskDef = _1.__.defineManagedFunc()
    .name(`startAppBranchUpdateTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_access_token_is_invalid_1.AppAccessTokenIsInvalid,
    app_access_token_is_expired_1.AppAccessTokenIsExpired,
    app_branch_does_not_exist_1.AppBranchDoesNotExist,
    app_branch_is_not_registered_1.AppBranchIsNotRegistered,
    app_branch_is_preinstalled_1.AppBranchIsPreinstalled,
    app_branch_ongoing_task_exists_1.AppBranchOngoingTaskExists,
    app_branch_root_dir_permissions_are_insufficient_1.AppBranchRootDirPermissionsAreInsufficient,
    app_branch_needs_repairing_1.AppBranchNeedsRepairing,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().startAppBranchUpdateTaskDef;
//# sourceMappingURL=start-app-branch-update-task-def.js.map