"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppBranchStateDef = void 0;
const _1 = require("./$");
const app_branch_is_not_registered_1 = require("./app-branch-is-not-registered");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
exports.retrieveAppBranchStateDef = _1.__.defineManagedFunc()
    .name(`retrieveAppBranchState`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
    app_branch_is_not_registered_1.AppBranchIsNotRegistered,
])
    .async()
    .executeType().retrieveAppBranchStateDef;
//# sourceMappingURL=retrieve-app-branch-state-def.js.map