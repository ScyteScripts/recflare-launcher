"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppBranchUninstallTaskDef = void 0;
const _1 = require("./$");
const app_branch_is_not_registered_1 = require("./app-branch-is-not-registered");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
const app_branch_is_preinstalled_1 = require("./app-branch-is-preinstalled");
const app_branch_ongoing_task_exists_1 = require("./app-branch-ongoing-task-exists");
const app_branch_root_dir_permissions_are_insufficient_1 = require("./app-branch-root-dir-permissions-are-insufficient");
exports.startAppBranchUninstallTaskDef = _1.__.defineManagedFunc()
    .name(`startAppBranchUninstallTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_branch_is_not_registered_1.AppBranchIsNotRegistered,
    app_branch_is_preinstalled_1.AppBranchIsPreinstalled,
    app_branch_ongoing_task_exists_1.AppBranchOngoingTaskExists,
    app_branch_root_dir_permissions_are_insufficient_1.AppBranchRootDirPermissionsAreInsufficient,
])
    .async()
    .executeType().startAppBranchUninstallTaskDef;
//# sourceMappingURL=start-app-branch-uninstall-task-def.js.map