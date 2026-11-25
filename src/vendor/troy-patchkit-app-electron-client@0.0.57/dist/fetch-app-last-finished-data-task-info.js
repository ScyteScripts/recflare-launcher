"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppLastFinishedDataTaskInfo = void 0;
const _1 = require("./$");
const app_downloading_access_token_is_expired_1 = require("./app-downloading-access-token-is-expired");
const app_downloading_access_token_is_invalid_1 = require("./app-downloading-access-token-is-invalid");
const app_last_finished_data_task_does_not_exist_1 = require("./app-last-finished-data-task-does-not-exist");
const app_license_key_is_blocked_1 = require("./app-license-key-is-blocked");
const app_license_key_is_invalid_1 = require("./app-license-key-is-invalid");
const app_license_key_is_required_1 = require("./app-license-key-is-required");
const app_password_is_invalid_1 = require("./app-password-is-invalid");
const app_repair_task_has_been_cancelled_1 = require("./app-repair-task-has-been-cancelled");
const app_root_dir_does_not_exist_1 = require("./app-root-dir-does-not-exist");
const app_root_dir_does_not_have_enough_free_space_for_repair_task_1 = require("./app-root-dir-does-not-have-enough-free-space-for-repair-task");
const app_root_dir_does_not_have_enough_free_space_for_update_task_1 = require("./app-root-dir-does-not-have-enough-free-space-for-update-task");
const app_root_dir_permissions_are_insufficient_1 = require("./app-root-dir-permissions-are-insufficient");
const app_task_type_1 = require("./app-task-type");
const app_update_task_has_been_cancelled_1 = require("./app-update-task-has-been-cancelled");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.fetchAppLastFinishedDataTaskInfo = _1.__.declareManagedFunc()
    .name(`fetchAppLastFinishedDataTaskInfo`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_root_dir_does_not_exist_1.AppRootDirDoesNotExist,
    app_root_dir_permissions_are_insufficient_1.AppRootDirPermissionsAreInsufficient,
    app_last_finished_data_task_does_not_exist_1.AppLastFinishedDataTaskDoesNotExist,
])
    .async()
    .execute(async ({ appRootDirPath, }) => {
    const libpkappsBindingFuncExecResultValueDef = _1.__.$refstructdi(_1.__.$refnapi)({
        hasPatchKitAppLastInstallTaskStateField: _1.__.$refnapi.types.bool,
        patchKitAppLastInstallTaskState: _1.__.$refstructdi(_1.__.$refnapi)({
            hasProgressField: _1.__.$refnapi.types.bool,
            progress: _1.__.$refstructdi(_1.__.$refnapi)({
                packagesProcessedBytesCount: _1.__.$refnapi.types.int64,
                packagesTotalBytesCount: _1.__.$refnapi.types.int64,
                packagesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
                filesProcessedBytesCount: _1.__.$refnapi.types.int64,
                filesTotalBytesCount: _1.__.$refnapi.types.int64,
                filesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
            }),
            hasErrorField: _1.__.$refnapi.types.bool,
            error: _1.__.$refnapi.types.int32,
        }),
        hasPatchKitAppLastUpdateTaskStateField: _1.__.$refnapi.types.bool,
        patchKitAppLastUpdateTaskState: _1.__.$refstructdi(_1.__.$refnapi)({
            hasProgressField: _1.__.$refnapi.types.bool,
            progress: _1.__.$refstructdi(_1.__.$refnapi)({
                packagesProcessedBytesCount: _1.__.$refnapi.types.int64,
                packagesTotalBytesCount: _1.__.$refnapi.types.int64,
                packagesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
                filesProcessedBytesCount: _1.__.$refnapi.types.int64,
                filesTotalBytesCount: _1.__.$refnapi.types.int64,
                filesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
            }),
            hasErrorField: _1.__.$refnapi.types.bool,
            error: _1.__.$refnapi.types.int32,
        }),
        hasPatchKitAppLastRepairTaskStateField: _1.__.$refnapi.types.bool,
        patchKitAppLastRepairTaskState: _1.__.$refstructdi(_1.__.$refnapi)({
            hasProgressField: _1.__.$refnapi.types.bool,
            progress: _1.__.$refstructdi(_1.__.$refnapi)({
                packagesProcessedBytesCount: _1.__.$refnapi.types.int64,
                packagesTotalBytesCount: _1.__.$refnapi.types.int64,
                packagesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
                filesProcessedBytesCount: _1.__.$refnapi.types.int64,
                filesTotalBytesCount: _1.__.$refnapi.types.int64,
                filesProcessedBytesCountPerSecond: _1.__.$refnapi.types.int64,
            }),
            hasErrorField: _1.__.$refnapi.types.bool,
            error: _1.__.$refnapi.types.int32,
        }),
    });
    const libpkappsBindingFuncExecResultDef = _1.__.$refstructdi(_1.__.$refnapi)({
        hasValueField: _1.__.$refnapi.types.bool,
        value: libpkappsBindingFuncExecResultValueDef,
    });
    const libpkappsBindingFuncExecResultPointer = _1.__.$refnapi.alloc(libpkappsBindingFuncExecResultDef);
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.getPatchKitAppLastFilesOperationTaskState.async((0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
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
    if (!libpkappsBindingFuncExecResult.hasValueField) {
        throw new app_last_finished_data_task_does_not_exist_1.AppLastFinishedDataTaskDoesNotExist();
    }
    const libpkappsBindingFuncExecResultValue = libpkappsBindingFuncExecResult.value;
    if (libpkappsBindingFuncExecResultValue.hasPatchKitAppLastInstallTaskStateField) {
        const libpkappsBindingPatchKitAppLastInstallTaskState = libpkappsBindingFuncExecResultValue.patchKitAppLastInstallTaskState;
        return {
            type: app_task_type_1.AppTaskType.UpdateTask,
            progress: libpkappsBindingPatchKitAppLastInstallTaskState.hasProgressField
                ? {
                    downloadingProcessedBytesCount: libpkappsBindingPatchKitAppLastInstallTaskState.progress.packagesProcessedBytesCount,
                    downloadingTotalBytesCount: libpkappsBindingPatchKitAppLastInstallTaskState.progress.packagesTotalBytesCount,
                    downloadingProcessedBytesCountPerSecond: libpkappsBindingPatchKitAppLastInstallTaskState.progress.packagesProcessedBytesCountPerSecond,
                    processedBytesCount: libpkappsBindingPatchKitAppLastInstallTaskState.progress.filesProcessedBytesCount,
                    totalBytesCount: libpkappsBindingPatchKitAppLastInstallTaskState.progress.filesTotalBytesCount,
                    processedBytesCountPerSecond: libpkappsBindingPatchKitAppLastInstallTaskState.progress.filesProcessedBytesCountPerSecond,
                }
                : undefined,
            errorTypeName: libpkappsBindingPatchKitAppLastInstallTaskState.hasErrorField
                ? _1.__.invokeSyncExp(() => {
                    switch (libpkappsBindingPatchKitAppLastInstallTaskState.error) {
                        case 0:
                            return _1.__.FatalError.name;
                        case 1:
                            return app_update_task_has_been_cancelled_1.AppUpdateTaskHasBeenCancelled.name;
                        case 2:
                            return app_root_dir_does_not_have_enough_free_space_for_update_task_1.AppRootDirDoesNotHaveEnoughFreeSpaceForUpdateTask.name;
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
        };
    }
    if (libpkappsBindingFuncExecResultValue.hasPatchKitAppLastUpdateTaskStateField) {
        const libpkappsBindingPatchKitAppLastUpdateTaskState = libpkappsBindingFuncExecResultValue.patchKitAppLastUpdateTaskState;
        return {
            type: app_task_type_1.AppTaskType.UpdateTask,
            progress: libpkappsBindingPatchKitAppLastUpdateTaskState.hasProgressField
                ? {
                    downloadingProcessedBytesCount: libpkappsBindingPatchKitAppLastUpdateTaskState.progress.packagesProcessedBytesCount,
                    downloadingTotalBytesCount: libpkappsBindingPatchKitAppLastUpdateTaskState.progress.packagesTotalBytesCount,
                    downloadingProcessedBytesCountPerSecond: libpkappsBindingPatchKitAppLastUpdateTaskState.progress.packagesProcessedBytesCountPerSecond,
                    processedBytesCount: libpkappsBindingPatchKitAppLastUpdateTaskState.progress.filesProcessedBytesCount,
                    totalBytesCount: libpkappsBindingPatchKitAppLastUpdateTaskState.progress.filesTotalBytesCount,
                    processedBytesCountPerSecond: libpkappsBindingPatchKitAppLastUpdateTaskState.progress.filesProcessedBytesCountPerSecond,
                }
                : undefined,
            errorTypeName: libpkappsBindingPatchKitAppLastUpdateTaskState.hasErrorField
                ? _1.__.invokeSyncExp(() => {
                    switch (libpkappsBindingPatchKitAppLastUpdateTaskState.error) {
                        case 0:
                            return _1.__.FatalError.name;
                        case 1:
                            return app_update_task_has_been_cancelled_1.AppUpdateTaskHasBeenCancelled.name;
                        case 2:
                            return app_root_dir_does_not_have_enough_free_space_for_update_task_1.AppRootDirDoesNotHaveEnoughFreeSpaceForUpdateTask.name;
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
        };
    }
    if (libpkappsBindingFuncExecResultValue.hasPatchKitAppLastRepairTaskStateField) {
        const libpkappsBindingPatchKitAppLastRepairTaskState = libpkappsBindingFuncExecResultValue.patchKitAppLastRepairTaskState;
        return {
            type: app_task_type_1.AppTaskType.RepairTask,
            progress: libpkappsBindingPatchKitAppLastRepairTaskState.hasProgressField
                ? {
                    downloadingProcessedBytesCount: libpkappsBindingPatchKitAppLastRepairTaskState.progress.packagesProcessedBytesCount,
                    downloadingTotalBytesCount: libpkappsBindingPatchKitAppLastRepairTaskState.progress.packagesTotalBytesCount,
                    downloadingProcessedBytesCountPerSecond: libpkappsBindingPatchKitAppLastRepairTaskState.progress.packagesProcessedBytesCountPerSecond,
                    processedBytesCount: libpkappsBindingPatchKitAppLastRepairTaskState.progress.filesProcessedBytesCount,
                    totalBytesCount: libpkappsBindingPatchKitAppLastRepairTaskState.progress.filesTotalBytesCount,
                    processedBytesCountPerSecond: libpkappsBindingPatchKitAppLastRepairTaskState.progress.filesProcessedBytesCountPerSecond,
                }
                : undefined,
            errorTypeName: libpkappsBindingPatchKitAppLastRepairTaskState.hasErrorField
                ? _1.__.invokeSyncExp(() => {
                    switch (libpkappsBindingPatchKitAppLastRepairTaskState.error) {
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
        };
    }
    _1.__.checkInvalidAssertion();
}).fetchAppLastFinishedDataTaskInfo;
//# sourceMappingURL=fetch-app-last-finished-data-task-info.js.map