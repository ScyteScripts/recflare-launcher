"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.writeTraceLog = exports.IS_WRITING_TRACE_LOG_ALLOWED = exports.EXECUTE_WRITE_TRACE_LOG = void 0;
const box_1 = require("./box");
exports.EXECUTE_WRITE_TRACE_LOG = (0, box_1.declareGlobalMutableBox)(() => {
});
exports.IS_WRITING_TRACE_LOG_ALLOWED = (0, box_1.declareGlobalMutableBox)(false);
const writeTraceLog = (traceLogMsg) => {
    if (exports.IS_WRITING_TRACE_LOG_ALLOWED.value) {
        exports.EXECUTE_WRITE_TRACE_LOG.value(traceLogMsg);
    }
};
exports.writeTraceLog = writeTraceLog;
//# sourceMappingURL=write-trace-log.js.map