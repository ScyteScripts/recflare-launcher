"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchAppScreenshotsLangPresentationDef = void 0;
const _1 = require("./$");
const app_is_not_allowed_1 = require("./app-is-not-allowed");
const service_connection_is_not_available_1 = require("./service-connection-is-not-available");
const app_does_not_exist_1 = require("./app-does-not-exist");
const app_access_token_is_invalid_1 = require("./app-access-token-is-invalid");
const app_access_token_is_expired_1 = require("./app-access-token-is-expired");
exports.fetchAppScreenshotsLangPresentationDef = _1.__.defineManagedFunc()
    .name(`fetchAppScreenshotsLangPresentation`)
    .namespace(_1.$namespace)
    .errorTypes([
    app_is_not_allowed_1.AppIsNotAllowed,
    app_does_not_exist_1.AppDoesNotExist,
    app_access_token_is_invalid_1.AppAccessTokenIsInvalid,
    app_access_token_is_expired_1.AppAccessTokenIsExpired,
    service_connection_is_not_available_1.ServiceConnectionIsNotAvailable,
])
    .async()
    .executeType().fetchAppScreenshotsLangPresentationDef;
//# sourceMappingURL=fetch-app-screenshots-lang-presentation-def.js.map