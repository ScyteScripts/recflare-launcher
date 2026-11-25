"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.declareManagedFunc = exports.getManagedFuncFullname = exports.defineManagedFunc = void 0;
const fatal_error_1 = require("./fatal-error");
const report_error_1 = require("./report-error");
const write_error_log_1 = require("./write-error-log");
const write_trace_log_1 = require("./write-trace-log");
const defineManagedFunc = () => {
    return {
        name: (managedFuncName) => {
            return {
                namespace: (managedFuncNamespace) => {
                    return {
                        errorTypes: (managedFuncErrorTypes) => {
                            return {
                                sync: () => {
                                    return {
                                        executeType: () => {
                                            const managedSyncFuncDef = {
                                                name: managedFuncName,
                                                namespace: managedFuncNamespace,
                                                errorTypes: managedFuncErrorTypes,
                                            };
                                            return {
                                                [`${String(managedFuncName)}Def`]: managedSyncFuncDef,
                                            };
                                        },
                                    };
                                },
                                async: () => {
                                    return {
                                        executeType: () => {
                                            const managedAsyncFuncDef = {
                                                name: managedFuncName,
                                                namespace: managedFuncNamespace,
                                                errorTypes: managedFuncErrorTypes,
                                            };
                                            return {
                                                [`${String(managedFuncName)}Def`]: managedAsyncFuncDef,
                                            };
                                        },
                                    };
                                },
                            };
                        },
                    };
                },
            };
        },
    };
};
exports.defineManagedFunc = defineManagedFunc;
const getManagedFuncFullname = (managedFuncName, managedFuncNamespace) => {
    if (managedFuncNamespace === undefined) {
        return managedFuncName;
    }
    return `${managedFuncNamespace}.${managedFuncName}`;
};
exports.getManagedFuncFullname = getManagedFuncFullname;
const onManagedFuncExecStarted = (managedFuncFullname, managedFuncExecArgs, managedFuncConfig) => {
    if (managedFuncConfig.isLoggingEnabled) {
        (0, write_trace_log_1.writeTraceLog)({
            kind: `on-managed-func-exec-started`,
            managedFuncFullname,
            managedFuncExecArgs: managedFuncConfig.isLoggingExecArgsEnabled ? managedFuncExecArgs : undefined,
        });
    }
};
const onManagedFuncExecSucceeded = (managedFuncFullname, managedFuncExecResult, managedFuncConfig) => {
    if (managedFuncConfig.isLoggingEnabled) {
        (0, write_trace_log_1.writeTraceLog)({
            kind: `on-managed-func-exec-succeeded`,
            managedFuncFullname,
            managedFuncExecResult: managedFuncConfig.isLoggingExecResultEnabled ? managedFuncExecResult : undefined,
        });
    }
};
const getIsManagedFuncExecErrorValid = (managedFuncExecError, managedFuncErrorTypes) => {
    if (managedFuncExecError instanceof fatal_error_1.FatalError) {
        return true;
    }
    for (const managedFuncErrorType of managedFuncErrorTypes) {
        if (managedFuncExecError instanceof managedFuncErrorType) {
            return true;
        }
    }
    return false;
};
const onManagedFuncExecFailed = (managedFuncFullname, managedFuncExecThrownObject, managedFuncConfig, managedFuncErrorTypes) => {
    const managedFuncExecError = managedFuncExecThrownObject instanceof Error
        ? managedFuncExecThrownObject
        : new Error(`Unknown error: ${String(managedFuncExecThrownObject)}`);
    const isManagedFuncExecErrorValid = getIsManagedFuncExecErrorValid(managedFuncExecError, managedFuncErrorTypes);
    if (isManagedFuncExecErrorValid) {
        if (managedFuncConfig.isLoggingEnabled) {
            (0, write_trace_log_1.writeTraceLog)({
                kind: `on-managed-func-exec-failed`,
                managedFuncFullname,
                managedFuncExecErrorStack: managedFuncExecError.stack,
            });
        }
        throw managedFuncExecError;
    }
    (0, report_error_1.reportErrorIfAllowed)(managedFuncExecError);
    (0, write_error_log_1.writeErrorLog)({
        kind: `on-managed-func-exec-failed`,
        managedFuncFullname,
        managedFuncExecErrorStack: managedFuncExecError.stack,
    });
    throw new fatal_error_1.FatalError();
};
const constructManagedFunc = (managedFunc, managedFuncDef) => {
    Object.defineProperty(managedFunc, `name`, {
        value: managedFuncDef.name,
        configurable: false,
        writable: false,
    });
    Object.defineProperty(managedFunc, `def`, {
        value: managedFuncDef,
        configurable: false,
        writable: false,
    });
    const managedFuncConfig = {
        isLoggingEnabled: true,
        isLoggingExecArgsEnabled: true,
        isLoggingExecResultEnabled: true,
    };
    Object.defineProperty(managedFunc, `config`, {
        value: managedFuncConfig,
        configurable: false,
        writable: false,
    });
};
const declareManagedFunc = () => {
    const setSyncExecute = (managedSyncFuncDef, executeManagedSyncFunc) => {
        const managedSyncFunc = ((managedSyncFuncExecArgs) => {
            const managedSyncFuncFullname = (0, exports.getManagedFuncFullname)(managedSyncFunc.def.name, managedSyncFunc.def.namespace);
            try {
                onManagedFuncExecStarted(managedSyncFuncFullname, managedSyncFuncExecArgs, managedSyncFunc.config);
                const managedSyncFuncExecResult = executeManagedSyncFunc(managedSyncFuncExecArgs);
                onManagedFuncExecSucceeded(managedSyncFuncFullname, managedSyncFuncExecResult, managedSyncFunc.config);
                return managedSyncFuncExecResult;
            }
            catch (managedSyncFuncExecThrownObject) {
                onManagedFuncExecFailed(managedSyncFuncFullname, managedSyncFuncExecThrownObject, managedSyncFunc.config, managedSyncFunc.def.errorTypes);
            }
        });
        constructManagedFunc(managedSyncFunc, managedSyncFuncDef);
        return {
            [managedSyncFunc.def.name]: managedSyncFunc,
        };
    };
    const setAsyncExecute = (managedAsyncFuncDef, executeManagedAsyncFunc) => {
        const managedAsyncFunc = (async (managedAsyncFuncExecArgs) => {
            const managedAsyncFuncFullname = (0, exports.getManagedFuncFullname)(managedAsyncFunc.def.name, managedAsyncFunc.def.namespace);
            try {
                onManagedFuncExecStarted(managedAsyncFuncFullname, managedAsyncFuncExecArgs, managedAsyncFunc.config);
                const managedAsyncFuncExecResult = await executeManagedAsyncFunc(managedAsyncFuncExecArgs);
                onManagedFuncExecSucceeded(managedAsyncFuncFullname, managedAsyncFuncExecResult, managedAsyncFunc.config);
                return managedAsyncFuncExecResult;
            }
            catch (managedAsyncFuncExecThrownObject) {
                onManagedFuncExecFailed(managedAsyncFuncFullname, managedAsyncFuncExecThrownObject, managedAsyncFunc.config, managedAsyncFunc.def.errorTypes);
            }
        });
        constructManagedFunc(managedAsyncFunc, managedAsyncFuncDef);
        return {
            [managedAsyncFunc.def.name]: managedAsyncFunc,
        };
    };
    return {
        name: (managedFuncName) => {
            return {
                namespace: (managedFuncNamespace) => {
                    return {
                        errorTypes: (managedFuncErrorTypes) => {
                            return {
                                sync: () => {
                                    return {
                                        execute: (executeManagedSyncFunc) => setSyncExecute({
                                            name: managedFuncName,
                                            namespace: managedFuncNamespace,
                                            errorTypes: managedFuncErrorTypes,
                                        }, executeManagedSyncFunc),
                                    };
                                },
                                async: () => {
                                    return {
                                        execute: (executeManagedAsyncFunc) => setAsyncExecute({
                                            name: managedFuncName,
                                            namespace: managedFuncNamespace,
                                            errorTypes: managedFuncErrorTypes,
                                        }, executeManagedAsyncFunc),
                                    };
                                },
                            };
                        },
                    };
                },
            };
        },
        sync: () => {
            return {
                def: (managedSyncFuncDef) => {
                    return {
                        execute: (executeManagedSyncFunc) => setSyncExecute(managedSyncFuncDef, executeManagedSyncFunc),
                    };
                },
            };
        },
        async: () => {
            return {
                def: (managedAsyncFuncDef) => {
                    return {
                        execute: (executeManagedAsyncFunc) => setAsyncExecute(managedAsyncFuncDef, executeManagedAsyncFunc),
                    };
                },
            };
        },
    };
};
exports.declareManagedFunc = declareManagedFunc;
//# sourceMappingURL=managed-func.js.map