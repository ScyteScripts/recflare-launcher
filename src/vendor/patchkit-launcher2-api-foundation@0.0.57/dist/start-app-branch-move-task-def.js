"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchMoveTaskDef = void 0;
const _1 = require("./$");
const app_branch_is_not_registered_1 = require("./app-branch-is-not-registered");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_is_preinstalled_1 = require("./app-branch-is-preinstalled");
const app_branch_ongoing_task_exists_1 = require("./app-branch-ongoing-task-exists");
const app_branch_root_dir_permissions_are_insufficient_1 = require("./app-branch-root-dir-permissions-are-insufficient");
const app_branch_new_root_dir_permissions_are_insufficient_1 = require("./app-branch-new-root-dir-permissions-are-insufficient");
const app_branch_new_root_dir_is_conflicting_with_other_file_system_entry_1 = require("./app-branch-new-root-dir-is-conflicting-with-other-file-system-entry");
const app_branch_new_root_dir_is_not_empty_1 = require("./app-branch-new-root-dir-is-not-empty");
exports.startAppBranchMoveTaskDef = _1.__.defineManagedFunc()
    .name(`startAppBranchMoveTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_branch_is_not_registered_1.AppBranchIsNotRegistered,
    app_branch_is_preinstalled_1.AppBranchIsPreinstalled,
    app_branch_ongoing_task_exists_1.AppBranchOngoingTaskExists,
    app_branch_root_dir_permissions_are_insufficient_1.AppBranchRootDirPermissionsAreInsufficient,
    app_branch_new_root_dir_permissions_are_insufficient_1.AppBranchNewRootDirPermissionsAreInsufficient,
    app_branch_new_root_dir_is_conflicting_with_other_file_system_entry_1.AppBranchNewRootDirIsConflictingWithOtherFileSystemEntry,
    app_branch_new_root_dir_is_not_empty_1.AppBranchNewRootDirIsNotEmpty,
])
    .async()
    .executeType().startAppBranchMoveTaskDef;
//# sourceMappingURL=start-app-branch-move-task-def.js.map