"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppMoveTask = void 0;
const _1 = require("./$");
const app_new_root_dir_is_conflicting_with_other_file_system_entry_1 = require("./app-new-root-dir-is-conflicting-with-other-file-system-entry");
const app_new_root_dir_is_not_empty_1 = require("./app-new-root-dir-is-not-empty");
const app_new_root_dir_permissions_are_insufficient_1 = require("./app-new-root-dir-permissions-are-insufficient");
const app_root_dir_does_not_exist_1 = require("./app-root-dir-does-not-exist");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.startAppMoveTask = _1.__.declareManagedFunc()
    .name(`startAppMoveTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_root_dir_does_not_exist_1.AppRootDirDoesNotExist,
    app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient,
    app_new_root_dir_permissions_are_insufficient_1.AppNewRootDirPermissionsAreInsufficient,
    app_new_root_dir_is_conflicting_with_other_file_system_entry_1.AppNewRootDirIsConflictingWithOtherFileSystemEntry,
    app_new_root_dir_is_not_empty_1.AppNewRootDirIsNotEmpty,
])
    .async()
    .execute(async ({ appRootDirPath, appNewRootDirPath, }) => {
    const libpkappsBindingFuncExecResultDef = _1.__.$refstructdi(_1.__.$refnapi)({
        patchKitAppMoveTaskId: _1.__.$refnapi.types.int32,
    });
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(libpkappsBindingFuncExecResultDef);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.startPatchKitAppMoveTask.async((0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
            path: appRootDirPath,
        }), (0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
            path: appNewRootDirPath,
        }), libpkappsBindingFuncExecResultPointer, (e, r) => {
            if (e !== null) {
                reject(e);
            }
            resolve(r);
        });
    });
    if (libpkappsBindingFuncExecErrorName
        ===
            `patchkit_app_dir_does_not_exist`) {
        throw new app_root_dir_does_not_exist_1.AppRootDirDoesNotExist();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `unauthorized_access_to_patchkit_app_dir`) {
        throw new app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `unauthorized_access_to_patchkit_app_new_dir`) {
        throw new app_new_root_dir_permissions_are_insufficient_1.AppNewRootDirPermissionsAreInsufficient();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `file_is_conflicting_with_patchkit_app_new_dir`) {
        throw new app_new_root_dir_is_conflicting_with_other_file_system_entry_1.AppNewRootDirIsConflictingWithOtherFileSystemEntry();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `patchkit_app_new_dir_is_not_empty`) {
        throw new app_new_root_dir_is_not_empty_1.AppNewRootDirIsNotEmpty();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            libpkapps_binding_fatal_error_name_1.LIBPKAPPS_BINDING_FATAL_ERROR_NAME.value) {
        throw new _1.__.FatalError();
    }
    _1.__.checkAssertion(libpkappsBindingFuncExecErrorName === null);
    const libpkappsBindingFuncExecResult = _1.__.$refnapi.deref(libpkappsBindingFuncExecResultPointer);
    return {
        appMoveTaskId: libpkappsBindingFuncExecResult.patchKitAppMoveTaskId,
    };
}).startAppMoveTask;
//# sourceMappingURL=start-app-move-task.js.map