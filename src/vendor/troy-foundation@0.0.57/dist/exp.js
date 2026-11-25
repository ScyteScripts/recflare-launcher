"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.invokeAsyncExp = exports.invokeSyncExp = void 0;
const invokeSyncExp = (syncExp) => {
    return syncExp();
};
exports.invokeSyncExp = invokeSyncExp;
const invokeAsyncExp = async (asyncExp) => {
    return await asyncExp();
};
exports.invokeAsyncExp = invokeAsyncExp;
//# sourceMappingURL=exp.js.map