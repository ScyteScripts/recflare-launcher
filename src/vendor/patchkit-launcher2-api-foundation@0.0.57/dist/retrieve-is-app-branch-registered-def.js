"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveIsAppBranchRegisteredDef = void 0;
const _1 = require("./$");
const app_branch_is_not_allowed_1 = require("./app-branch-is-not-allowed");
exports.retrieveIsAppBranchRegisteredDef = _1.__.defineManagedFunc()
    .name(`retrieveIsAppBranchRegistered`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_is_not_allowed_1.AppBranchIsNotAllowed,
])
    .async()
    .executeType().retrieveIsAppBranchRegisteredDef;
//# sourceMappingURL=retrieve-is-app-branch-registered-def.js.map