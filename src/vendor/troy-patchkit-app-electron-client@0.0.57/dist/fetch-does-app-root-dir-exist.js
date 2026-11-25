"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesAppRootDirExist = void 0;
const _1 = require("./$");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.fetchDoesAppRootDirExist = _1.__.declareManagedFunc()
    .name(`fetchDoesAppRootDirExist`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient,
])
    .async()
    .execute(async ({ appRootDirPath, }) => {
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(_1.__.$refnapi.types.bool);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.getDoesPatchKitAppDirExist.async((0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
            path: appRootDirPath,
        }), libpkappsBindingFuncExecResultPointer, (e, r) => {
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
            libpkapps_binding_fatal_error_name_1.LIBPKAPPS_BINDING_FATAL_ERROR_NAME.value) {
        throw new _1.__.FatalError();
    }
    _1.__.checkAssertion(libpkappsBindingFuncExecErrorName === null);
    const libpkappsBindingFuncExecResult = _1.__.$refnapi.deref(libpkappsBindingFuncExecResultPointer);
    return libpkappsBindingFuncExecResult;
}).fetchDoesAppRootDirExist;
//# sourceMappingURL=fetch-does-app-root-dir-exist.js.map