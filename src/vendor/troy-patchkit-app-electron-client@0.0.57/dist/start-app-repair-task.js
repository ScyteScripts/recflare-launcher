"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startAppRepairTask = void 0;
const _1 = require("./$");
const app_root_dir_does_not_exist_1 = require("./app-root-dir-does-not-exist");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const convert_string_or_undefined_to_libpkapps_binding_nullable_string_1 = require("./convert-string-or-undefined-to-libpkapps-binding-nullable-string");
const convert_string_to_libpkapps_binding_string_1 = require("./convert-string-to-libpkapps-binding-string");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.startAppRepairTask = _1.__.declareManagedFunc()
    .name(`startAppRepairTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_root_dir_does_not_exist_1.AppRootDirDoesNotExist,
    app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient,
])
    .async()
    .execute(async ({ appRootDirPath, appSecret, appPassword, appLicenseKey, appAccessToken, }) => {
    const libpkappsBindingFuncExecResultDef = _1.__.$refstructdi(_1.__.$refnapi)({
        patchKitAppRepairTaskId: _1.__.$refnapi.types.int32,
    });
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(libpkappsBindingFuncExecResultDef);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.startPatchKitAppRepairTask.async((0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
            path: appRootDirPath,
        }), (0, convert_string_to_libpkapps_binding_string_1.convertStringToLibpkappsBindingString)({
            string: appSecret,
        }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
            string: appLicenseKey,
        }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
            string: appAccessToken,
        }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
            string: appPassword,
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
            libpkapps_binding_fatal_error_name_1.LIBPKAPPS_BINDING_FATAL_ERROR_NAME.value) {
        throw new _1.__.FatalError();
    }
    _1.__.checkAssertion(libpkappsBindingFuncExecErrorName === null);
    const libpkappsBindingFuncExecResult = _1.__.$refnapi.deref(libpkappsBindingFuncExecResultPointer);
    return {
        appRepairTaskId: libpkappsBindingFuncExecResult.patchKitAppRepairTaskId,
    };
}).startAppRepairTask;
//# sourceMappingURL=start-app-repair-task.js.map