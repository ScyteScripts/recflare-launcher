"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.initializeLogging = void 0;
const _1 = require("./$");
const get_logs_dir_path_1 = require("./get-logs-dir-path");
exports.initializeLogging = _1.__.declareManagedFunc()
    .name(`initializeLogging`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    _1.__.IS_WRITING_TRACE_LOG_ALLOWED.value = true;
    const consoleLog = console.log;
    const consoleError = console.error;
    try {
        const logsDirPath = (0, get_logs_dir_path_1.getLogsDirPath)();
        _1.__.$fs.mkdirSync(logsDirPath, {
            recursive: true,
        });
        const logsFilePath = _1.__.$path.resolve(logsDirPath, `logs.yml`);
        const logsFileWriteStream = _1.__.$fs.createWriteStream(logsFilePath);
        await new Promise((resolvePromise) => logsFileWriteStream.once(`ready`, resolvePromise));
        _1.__.EXECUTE_WRITE_TRACE_LOG.value = (traceLogMsg) => {
            try {
                const traceLogInfo = {
                    writedDate: new Date(),
                    type: `trace`,
                    msg: traceLogMsg,
                };
                consoleLog(traceLogInfo);
                logsFileWriteStream.write(_1.__.$jsyaml.dump([
                    traceLogInfo,
                ]));
            }
            catch (_a) {
            }
        };
        _1.__.EXECUTE_WRITE_INFO_LOG.value = (infoLogMsg) => {
            try {
                const infoLogInfo = {
                    writedDate: new Date(),
                    type: `info`,
                    msg: infoLogMsg,
                };
                consoleLog(infoLogInfo);
                logsFileWriteStream.write(_1.__.$jsyaml.dump([
                    infoLogInfo,
                ]));
            }
            catch (_a) {
            }
        };
        _1.__.EXECUTE_WRITE_ERROR_LOG.value = (errorLogMsg) => {
            try {
                const errorLogInfo = {
                    writedDate: new Date(),
                    type: `error`,
                    msg: errorLogMsg,
                };
                consoleError(errorLogInfo);
                logsFileWriteStream.write(_1.__.$jsyaml.dump([
                    errorLogInfo,
                ]));
            }
            catch (_a) {
            }
        };
        console.trace = (trace) => {
            _1.__.writeTraceLog({
                kind: `on-console-trace`,
                trace,
            });
        };
        console.log = (log) => {
            _1.__.writeTraceLog({
                kind: `on-console-log`,
                log,
            });
        };
        console.warn = (warn) => {
            _1.__.writeTraceLog({
                kind: `on-console-warn`,
                warn,
            });
        };
        console.error = (error) => {
            _1.__.writeTraceLog({
                kind: `on-console-error`,
                error,
            });
        };
    }
    catch (e) {
        consoleError(e);
    }
}).initializeLogging;
//# sourceMappingURL=initialize-logging.js.map