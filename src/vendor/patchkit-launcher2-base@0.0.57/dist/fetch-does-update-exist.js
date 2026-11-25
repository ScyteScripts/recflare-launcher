"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesUpdateExist = void 0;
const _1 = require("./$");
const update_state_1 = require("./update-state");
exports.fetchDoesUpdateExist = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchDoesUpdateExistDef)
    .execute(async () => {
    return update_state_1.UPDATE_STATE.value !== undefined;
}).fetchDoesUpdateExist;
exports.fetchDoesUpdateExist.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-does-update-exist.js.map