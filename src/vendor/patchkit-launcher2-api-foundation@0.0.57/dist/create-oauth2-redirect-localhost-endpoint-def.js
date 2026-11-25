"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createOauth2RedirectLocalhostEndpointDef = void 0;
const _1 = require("./$");
const oauth2_redirect_localhost_endpoint_port_is_not_available_1 = require("./oauth2-redirect-localhost-endpoint-port-is-not-available");
exports.createOauth2RedirectLocalhostEndpointDef = _1.__.defineManagedFunc()
    .name(`createOauth2RedirectLocalhostEndpoint`)
    .namespace(_1.$namespace)
    .errorTypes([
    oauth2_redirect_localhost_endpoint_port_is_not_available_1.Oauth2RedirectLocalhostEndpointPortIsNotAvailable,
])
    .async()
    .executeType().createOauth2RedirectLocalhostEndpointDef;
//# sourceMappingURL=create-oauth2-redirect-localhost-endpoint-def.js.map