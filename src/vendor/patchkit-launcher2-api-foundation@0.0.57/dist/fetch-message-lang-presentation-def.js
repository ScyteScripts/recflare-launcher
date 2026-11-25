"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchMessageLangPresentationDef = void 0;
const _1 = require("./$");
const message_does_not_exist_1 = require("./message-does-not-exist");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
exports.fetchMessageLangPresentationDef = _1.__.defineManagedFunc()
    .name(`fetchMessageLangPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    message_does_not_exist_1.MessageDoesNotExist,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchMessageLangPresentationDef;
//# sourceMappingURL=fetch-message-lang-presentation-def.js.map