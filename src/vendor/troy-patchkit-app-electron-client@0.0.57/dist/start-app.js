"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startApp = void 0;
const _1 = require("./$");
const app_is_not_installed_1 = require("./app-is-not-installed");
const app_needs_repairing_1 = require("./app-needs-repairing");
const app_root_dir_does_not_exist_1 = require("./app-root-dir-does-not-exist");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const convert_string_or_undefined_to_libpkapps_binding_nullable_string_1 = require("./convert-string-or-undefined-to-libpkapps-binding-nullable-string");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.startApp = _1.__.declareManagedFunc()
    .name(`startApp`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_root_dir_does_not_exist_1.AppRootDirDoesNotExist,
    app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient,
    app_is_not_installed_1.AppIsNotInstalled,
    app_needs_repairing_1.AppNeedsRepairing,
])
    .async()
    .execute(async ({ appRootDirPath, appProcessExtraArgsAsText, }) => {
    const libpkappsBindingFuncExecResultDef = _1.__.$refstructdi(_1.__.$refnapi)({
        patchKitAppProcessId: _1.__.$refnapi.types.int32,
    });
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(libpkappsBindingFuncExecResultDef);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.startPatchKitApp.async((0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
            path: appRootDirPath,
        }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
            string: appProcessExtraArgsAsText,
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
            `patchkit_app_is_not_installed`) {
        throw new app_is_not_installed_1.AppIsNotInstalled();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            `patchkit_app_needs_repair`) {
        throw new app_needs_repairing_1.AppNeedsRepairing();
    }
    if (libpkappsBindingFuncExecErrorName
        ===
            libpkapps_binding_fatal_error_name_1.LIBPKAPPS_BINDING_FATAL_ERROR_NAME.value) {
        throw new _1.__.FatalError();
    }
    _1.__.checkAssertion(libpkappsBindingFuncExecErrorName === null);
    const libpkappsBindingFuncExecResult = _1.__.$refnapi.deref(libpkappsBindingFuncExecResultPointer);
    return {
        appProcessId: libpkappsBindingFuncExecResult.patchKitAppProcessId,
    };
}).startApp;
//# sourceMappingURL=start-app.js.map