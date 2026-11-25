"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.destroyOauth2RedirectLocalhostEndpointDef = void 0;
const _1 = require("./$");
const oauth2_redirect_localhost_endpoint_does_not_exist_1 = require("./oauth2-redirect-localhost-endpoint-does-not-exist");
exports.destroyOauth2RedirectLocalhostEndpointDef = _1.__.defineManagedFunc()
    .name(`destroyOauth2RedirectLocalhostEndpoint`)
    .namespace(_1.$namespace)
    .errorTypes([
    oauth2_redirect_localhost_endpoint_does_not_exist_1.Oauth2RedirectLocalhostEndpointDoesNotExist,
])
    .async()
    .executeType().destroyOauth2RedirectLocalhostEndpointDef;
//# sourceMappingURL=destroy-oauth2-redirect-localhost-endpoint-def.js.map