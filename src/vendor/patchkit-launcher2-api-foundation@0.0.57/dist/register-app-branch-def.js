"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.registerAppBranchDef = void 0;
const _1 = require("./$");
const app_access_token_is_invalid_1 = require("./app-access-token-is-invalid");
const app_access_token_is_expired_1 = require("./app-access-token-is-expired");
const app_branch_does_not_exist_1 = require("./app-branch-does-not-exist");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_is_preinstalled_1 = require("./app-branch-is-preinstalled");
const app_branch_root_dir_is_conflicting_with_other_file_system_entry_1 = require("./app-branch-root-dir-is-conflicting-with-other-file-system-entry");
const app_branch_root_dir_is_not_empty_1 = require("./app-branch-root-dir-is-not-empty");
const app_branch_root_dir_permissions_are_insufficient_1 = require("./app-branch-root-dir-permissions-are-insufficient");
const app_branch_is_already_registered_1 = require("./app-branch-is-already-registered");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.registerAppBranchDef = _1.__.defineManagedFunc()
    .name(`registerAppBranch`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_access_token_is_invalid_1.AppAccessTokenIsInvalid,
    app_access_token_is_expired_1.AppAccessTokenIsExpired,
    app_branch_does_not_exist_1.AppBranchDoesNotExist,
    app_branch_is_preinstalled_1.AppBranchIsPreinstalled,
    app_branch_is_already_registered_1.AppBranchIsAlreadyRegistered,
    app_branch_root_dir_permissions_are_insufficient_1.AppBranchRootDirPermissionsAreInsufficient,
    app_branch_root_dir_is_conflicting_with_other_file_system_entry_1.AppBranchRootDirIsConflictingWithOtherFileSystemEntry,
    app_branch_root_dir_is_not_empty_1.AppBranchRootDirIsNotEmpty,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().registerAppBranchDef;
//# sourceMappingURL=register-app-branch-def.js.map