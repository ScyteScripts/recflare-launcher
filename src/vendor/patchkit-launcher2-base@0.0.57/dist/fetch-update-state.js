"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchUpdateState = void 0;
const _1 = require("./$");
const update_state_1 = require("./update-state");
exports.fetchUpdateState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchUpdateStateDef)
    .execute(async () => {
    if (update_state_1.UPDATE_STATE.value === undefined) {
        throw new _1._.UpdateDoesNotExist();
    }
    return update_state_1.UPDATE_STATE.value;
}).fetchUpdateState;
exports.fetchUpdateState.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-update-state.js.map