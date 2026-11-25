"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.retrieveAppStateDef = void 0;
const _1 = require("./$");
const app_is_not_registered_1 = require("./app-is-not-registered");
exports.retrieveAppStateDef = _1.__.defineManagedFunc()
    .name(`retrieveAppState`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_is_not_registered_1.AppIsNotRegistered,
])
    .async()
    .executeType().retrieveAppStateDef;
//# sourceMappingURL=retrieve-app-state-def.js.map