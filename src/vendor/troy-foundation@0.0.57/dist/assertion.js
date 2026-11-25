"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.checkInvalidAssertion = exports.checkAssertion = exports.AssertionIsInvalid = void 0;
const _1 = require("./$");
const managed_error_1 = require("./managed-error");
exports.AssertionIsInvalid = (0, managed_error_1.declareManagedErrorType)()
    .name(`AssertionIsInvalid`)
    .namespace(_1.$namespace).AssertionIsInvalid;
function checkAssertion(isAssertionValid) {
    if (!isAssertionValid) {
        throw new exports.AssertionIsInvalid();
    }
}
exports.checkAssertion = checkAssertion;
function checkInvalidAssertion() {
    throw new exports.AssertionIsInvalid();
}
exports.checkInvalidAssertion = checkInvalidAssertion;
//# sourceMappingURL=assertion.js.map