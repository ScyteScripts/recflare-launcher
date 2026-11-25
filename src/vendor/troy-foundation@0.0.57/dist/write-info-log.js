"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.writeInfoLog = exports.EXECUTE_WRITE_INFO_LOG = void 0;
const box_1 = require("./box");
exports.EXECUTE_WRITE_INFO_LOG = (0, box_1.declareGlobalMutableBox)(() => {
});
const writeInfoLog = (infoLogMsg) => {
    exports.EXECUTE_WRITE_INFO_LOG.value(infoLogMsg);
};
exports.writeInfoLog = writeInfoLog;
//# sourceMappingURL=write-info-log.js.map