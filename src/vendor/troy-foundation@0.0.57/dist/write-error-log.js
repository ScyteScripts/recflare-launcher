"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.writeErrorLog = exports.EXECUTE_WRITE_ERROR_LOG = void 0;
const box_1 = require("./box");
exports.EXECUTE_WRITE_ERROR_LOG = (0, box_1.declareGlobalMutableBox)(() => {
});
const writeErrorLog = (errorLogMsg) => {
    exports.EXECUTE_WRITE_ERROR_LOG.value(errorLogMsg);
};
exports.writeErrorLog = writeErrorLog;
//# sourceMappingURL=write-error-log.js.map