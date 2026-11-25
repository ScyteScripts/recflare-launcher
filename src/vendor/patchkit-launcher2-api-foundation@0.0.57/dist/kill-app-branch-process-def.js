"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.killAppBranchProcessDef = void 0;
const _1 = require("./$");
const app_branch_process_does_not_exist_1 = require("./app-branch-process-does-not-exist");
exports.killAppBranchProcessDef = _1.__.defineManagedFunc()
    .name(`killAppBranchProcess`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_branch_process_does_not_exist_1.AppBranchProcessDoesNotExist,
])
    .async()
    .executeType().killAppBranchProcessDef;
//# sourceMappingURL=kill-app-branch-process-def.js.map