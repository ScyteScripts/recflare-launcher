"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.reportErrorIfAllowed = exports.IS_REPORTING_ERROR_ALLOWED = exports.EXECUTE_REPORT_ERROR = void 0;
const box_1 = require("./box");
exports.EXECUTE_REPORT_ERROR = (0, box_1.declareGlobalMutableBox)(() => {
});
exports.IS_REPORTING_ERROR_ALLOWED = (0, box_1.declareGlobalMutableBox)(false);
const reportErrorIfAllowed = (error) => {
    if (exports.IS_REPORTING_ERROR_ALLOWED.value) {
        exports.EXECUTE_REPORT_ERROR.value(error);
    }
};
exports.reportErrorIfAllowed = reportErrorIfAllowed;
//# sourceMappingURL=report-error.js.map