"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppRepairTaskState = void 0;
const _1 = require("./$");
const app_downloading_access_token_is_expired_1 = require("./app-downloading-access-token-is-expired");
const app_downloading_access_token_is_invalid_1 = require("./app-downloading-access-token-is-invalid");
const app_license_key_is_blocked_1 = require("./app-license-key-is-blocked");
const app_license_key_is_invalid_1 = require("./app-license-key-is-invalid");
const app_license_key_is_required_1 = require("./app-license-key-is-required");
const app_password_is_invalid_1 = require("./app-password-is-invalid");
const app_root_dir_does_not_have_enough_free_space_for_repair_task_1 = require("./app-root-dir-does-not-have-enough-free-space-for-repair-task");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const app_repair_task_has_been_cancelled_1 = require("./app-repair-task-has-been-cancelled");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.fetchAppRepairTaskState = _1.__.declareManagedFunc()
    .name(`fetchAppRepairTaskState`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ appRepairTaskId, }) => {
    const libpkappsBindingFuncExecResultDef = _1.__.$refstructdi(_1.__.$refnapi)({
        id: _1.__.$refnapi.types.int32,
        hasProgressField: _1.__.$refnapi.types.bool,
        progress: _1.__.$refstructdi(_1.__.$refnapi)({
            packagesProcessedBytesCount: _1.__.$refnapi.types.int64,
            packagesTotalBytesCount: _1.__.$refnapi.types.int64,
            packagesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
            filesProcessedBytesCount: _1.__.$refnapi.types.int64,
            filesTotalBytesCount: _1.__.$refnapi.types.int64,
            filesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
        }),
        hasFinished: _1.__.$refnapi.types.bool,
        hasErrorField: _1.__.$refnapi.types.bool,
        error: _1.__.$refnapi.types.int32,
    });
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(libpkappsBindingFuncExecResultDef);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.getPatchKitAppRepairTaskState.async(appRepairTaskId, libpkappsBindingFuncExecResultPointer, (e, r) => {
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
        progress: libpkappsBindingFuncExecResult.hasProgressField
            ? {
                downloadingProcessedBytesCount: libpkappsBindingFuncExecResult.progress.packagesProcessedBytesCount,
                downloadingTotalBytesCount: libpkappsBindingFuncExecResult.progress.packagesTotalBytesCount,
                downloadingProcessedBytesCountPerSecond: libpkappsBindingFuncExecResult.progress.packagesProcessedBytesCountPerSecond,
                processedBytesCount: libpkappsBindingFuncExecResult.progress.filesProcessedBytesCount,
                totalBytesCount: libpkappsBindingFuncExecResult.progress.filesTotalBytesCount,
                processedBytesCountPerSecond: libpkappsBindingFuncExecResult.progress.filesProcessedBytesCountPerSecond,
            }
            : undefined,
        errorTypeName: libpkappsBindingFuncExecResult.hasErrorField
            ? _1.__.invokeSyncExp(() => {
                switch (libpkappsBindingFuncExecResult.error) {
                    case 0:
                        return _1.__.FatalError.name;
                    case 1:
                        return app_repair_task_has_been_cancelled_1.AppRepairTaskHasBeenCancelled.name;
                    case 2:
                        return app_root_dir_does_not_have_enough_free_space_for_repair_task_1.AppRootDirDoesNotHaveEnoughFreeSpaceForRepairTask.name;
                    case 3:
                        return app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient.name;
                    case 4:
                        return app_license_key_is_required_1.AppLicenseKeyIsRequired.name;
                    case 5:
                        return app_license_key_is_invalid_1.AppLicenseKeyIsInvalid.name;
                    case 6:
                        return app_license_key_is_blocked_1.AppLicenseKeyIsBlocked.name;
                    case 7:
                        return app_downloading_access_token_is_invalid_1.AppDownloadingAccessTokenIsInvalid.name;
                    case 8:
                        return app_downloading_access_token_is_expired_1.AppDownloadingAccessTokenIsExpired.name;
                    case 9:
                        return app_password_is_invalid_1.AppPasswordIsInvalid.name;
                }
                _1.__.checkInvalidAssertion();
            })
            : undefined,
        isFinished: libpkappsBindingFuncExecResult.hasFinished,
    };
}).fetchAppRepairTaskState;
exports.fetchAppRepairTaskState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-app-repair-task-state.js.map