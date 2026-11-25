"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.cancelAppRepairTask = void 0;
const _1 = require("./$");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_binding_fatal_error_name_1 = require("./libpkapps_binding_fatal_error_name");
exports.cancelAppRepairTask = _1.__.declareManagedFunc()
    .name(`cancelAppRepairTask`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ appRepairTaskId, }) => {
    const libpkappsBindingFuncExecErrorName = await new Promise((resolve, reject) => {
        _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
        libpkapps_binding_1.LIBPKAPPS_BINDING.value.cancelPatchKitAppRepairTask.async(appRepairTaskId, (e, r) => {
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
}).cancelAppRepairTask;
//# sourceMappingURL=cancel-app-repair-task.js.map