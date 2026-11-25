"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createAppRootDir = void 0;
const _1 = require("./$");
const app_root_dir_is_conflicting_with_other_file_system_entry_1 = require("./app-root-dir-is-conflicting-with-other-file-system-entry");
const app_root_dir_is_not_empty_1 = require("./app-root-dir-is-not-empty");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.createAppRootDir = _1.__.declareManagedFunc()
    .name(`createAppRootDir`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_root_dir_is_conflicting_with_other_file_system_entry_1.AppRootDirIsConflictingWithOtherFileSystemEntry,
    app_root_dir_is_not_empty_1.AppRootDirIsNotEmpty,
    app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient,
])
    .async()
    .execute(async ({ appRootDirPath, isAppNotEmptyRootDirAllowed, }) => {
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.createPatchKitAppDir.async((0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
            path: appRootDirPath,
        }), false, isAppNotEmptyRootDirAllowed, (e, r) => {
            if (e !== null) {
                reject(e);
            }
            resolve(r);
        });
    });
    if (libpkappsBindingFuncExecErrorName
        ===
            `unauthorized_access_to_patchkit_app_dir`) {
        throw new app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `file_is_conflicting_with_patchkit_app_dir`) {
        throw new app_root_dir_is_conflicting_with_other_file_system_entry_1.AppRootDirIsConflictingWithOtherFileSystemEntry();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `other_patchkit_app_dir_is_conflicting_with_patchkit_app_dir`) {
        throw new app_root_dir_is_conflicting_with_other_file_system_entry_1.AppRootDirIsConflictingWithOtherFileSystemEntry();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `patchkit_app_dir_is_not_empty`) {
        throw new app_root_dir_is_not_empty_1.AppRootDirIsNotEmpty();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            libpkapps_binding_fatal_error_name_1.LIBPKAPPS_BINDING_FATAL_ERROR_NAME.value) {
        throw new _1.__.FatalError();
    }
    _1.__.checkAssertion(libpkappsBindingFuncExecErrorName === null);
}).createAppRootDir;
//# sourceMappingURL=create-app-root-dir.js.map