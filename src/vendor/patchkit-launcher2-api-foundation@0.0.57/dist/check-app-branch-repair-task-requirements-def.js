"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.checkAppBranchRepairTaskRequirementsDef = void 0;
const _1 = require("./$");
const app_branch_is_not_registered_1 = require("./app-branch-is-not-registered");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_root_dir_permissions_are_insufficient_1 = require("./app-branch-root-dir-permissions-are-insufficient");
const app_branch_is_preinstalled_1 = require("./app-branch-is-preinstalled");
const app_branch_ongoing_task_exists_1 = require("./app-branch-ongoing-task-exists");
const app_branch_password_is_invalid_1 = require("./app-branch-password-is-invalid");
const app_branch_license_key_is_required_1 = require("./app-branch-license-key-is-required");
const app_branch_license_key_is_invalid_1 = require("./app-branch-license-key-is-invalid");
const app_branch_license_key_is_blocked_1 = require("./app-branch-license-key-is-blocked");
const app_branch_downloading_access_token_is_invalid_1 = require("./app-branch-downloading-access-token-is-invalid");
const app_branch_downloading_access_token_is_expired_1 = require("./app-branch-downloading-access-token-is-expired");
const app_branch_root_dir_does_not_have_enough_free_space_for_repair_task_1 = require("./app-branch-root-dir-does-not-have-enough-free-space-for-repair-task");
const app_access_token_is_invalid_1 = require("./app-access-token-is-invalid");
const app_access_token_is_expired_1 = require("./app-access-token-is-expired");
const app_branch_does_not_exist_1 = require("./app-branch-does-not-exist");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.checkAppBranchRepairTaskRequirementsDef = _1.__.defineManagedFunc()
    .name(`checkAppBranchRepairTaskRequirements`)
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
    app_branch_password_is_invalid_1.AppBranchPasswordIsInvalid,
    app_branch_license_key_is_required_1.AppBranchLicenseKeyIsRequired,
    app_branch_license_key_is_invalid_1.AppBranchLicenseKeyIsInvalid,
    app_branch_license_key_is_blocked_1.AppBranchLicenseKeyIsBlocked,
    app_branch_downloading_access_token_is_invalid_1.AppBranchDownloadingAccessTokenIsInvalid,
    app_branch_downloading_access_token_is_expired_1.AppBranchDownloadingAccessTokenIsExpired,
    app_branch_root_dir_does_not_have_enough_free_space_for_repair_task_1.AppBranchRootDirDoesNotHaveEnoughFreeSpaceForRepairTask,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().checkAppBranchRepairTaskRequirementsDef;
//# sourceMappingURL=check-app-branch-repair-task-requirements-def.js.map