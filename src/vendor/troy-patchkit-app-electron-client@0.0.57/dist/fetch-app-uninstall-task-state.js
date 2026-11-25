"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppUninstallTaskState = void 0;
const _1 = require("./$");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.fetchAppUninstallTaskState = _1.__.declareManagedFunc()
    .name(`fetchAppUninstallTaskState`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ appUninstallTaskId, }) => {
    const libpkappsBindingFuncExecResultDef = _1.__.$refstructdi(_1.__.$refnapi)({
        id: _1.__.$refnapi.types.int32,
        hasFinished: _1.__.$refnapi.types.bool,
        hasErrorField: _1.__.$refnapi.types.bool,
        error: _1.__.$refnapi.types.int32,
    });
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(libpkappsBindingFuncExecResultDef);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.getPatchKitAppUninstallTaskState.async(appUninstallTaskId, libpkappsBindingFuncExecResultPointer, (e, r) => {
            if (e !== null) {
                reject(e);
            }
            resolve(r);
        });
    });
    if (libpkappsBindingFuncExecErrorName
        ===
            libpkapps_binding_fatal_error_name_1.LIBPKAPPS_BINDING_FATAL_ERROR_NAME.value) {
        throw new _1.__.FatalError();
    }
    _1.__.checkAssertion(libpkappsBindingFuncExecErrorName === null);
    const libpkappsBindingFuncExecResult = _1.__.$refnapi.deref(libpkappsBindingFuncExecResultPointer);
    return {
        errorTypeName: libpkappsBindingFuncExecResult.hasErrorField
            ? _1.__.invokeSyncExp(() => {
                switch (libpkappsBindingFuncExecResult.error) {
                    case 0:
                        return _1.__.FatalError.name;
                    case 1:
                        return _1.__.FatalError.name;
                    case 2:
                        return app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient.name;
                }
                _1.__.checkInvalidAssertion();
            })
            : undefined,
        isFinished: libpkappsBindingFuncExecResult.hasFinished,
    };
}).fetchAppUninstallTaskState;
exports.fetchAppUninstallTaskState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-app-uninstall-task-state.js.map