"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUpdateStateDef = void 0;
const _1 = require("./$");
const update_does_not_exist_1 = require("./update-does-not-exist");
exports.fetchUpdateStateDef = _1.__.defineManagedFunc()
    .name(`fetchUpdateState`)
    .namespace(_1.$namespace)
    .errorTypes([
    update_does_not_exist_1.UpdateDoesNotExist,
])
    .async()
    .executeType().fetchUpdateStateDef;
//# sourceMappingURL=fetch-update-state-def.js.map