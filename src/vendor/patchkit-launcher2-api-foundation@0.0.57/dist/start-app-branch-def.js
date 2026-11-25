"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchDef = void 0;
const _1 = require("./$");
const app_branch_is_not_registered_1 = require("./app-branch-is-not-registered");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_process_already_exists_1 = require("./app-branch-process-already-exists");
const app_branch_ongoing_task_exists_1 = require("./app-branch-ongoing-task-exists");
const app_branch_root_dir_permissions_are_insufficient_1 = require("./app-branch-root-dir-permissions-are-insufficient");
const app_branch_is_not_installed_1 = require("./app-branch-is-not-installed");
const app_branch_needs_repairing_1 = require("./app-branch-needs-repairing");
exports.startAppBranchDef = _1.__.defineManagedFunc()
    .name(`startAppBranch`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_branch_is_not_registered_1.AppBranchIsNotRegistered,
    app_branch_process_already_exists_1.AppBranchProcessAlreadyExists,
    app_branch_ongoing_task_exists_1.AppBranchOngoingTaskExists,
    app_branch_root_dir_permissions_are_insufficient_1.AppBranchRootDirPermissionsAreInsufficient,
    app_branch_is_not_installed_1.AppBranchIsNotInstalled,
    app_branch_needs_repairing_1.AppBranchNeedsRepairing,
])
    .async()
    .executeType().startAppBranchDef;
//# sourceMappingURL=start-app-branch-def.js.map