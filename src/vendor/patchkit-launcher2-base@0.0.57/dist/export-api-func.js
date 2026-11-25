"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.exportApiFunc = void 0;
const _1 = require("./$");
const exportApiFunc = (apiFunc) => {
    const apiFuncFullname = _1.__.getManagedFuncFullname(apiFunc.def.name, apiFunc.def.namespace);
    _1.__.$electron.ipcMain.handle(apiFuncFullname, async (_apiFuncRequestInfo, apiFuncExecArgs) => {
        try {
            const apiFuncExecResult = await apiFunc(apiFuncExecArgs);
            return {
                execResult: apiFuncExecResult,
            };
        }
        catch (e) {
            _1.__.checkAssertion(e instanceof _1.__.ManagedError);
            return {
                execErrorInfo: {
                    typeFullname: _1.__.getManagedErrorFullname(e),
                    message: e.message,
                },
            };
        }
    });
};
exports.exportApiFunc = exportApiFunc;
//# sourceMappingURL=export-api-func.js.map