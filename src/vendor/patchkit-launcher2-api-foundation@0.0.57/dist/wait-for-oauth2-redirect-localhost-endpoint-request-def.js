"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.waitForOauth2RedirectLocalhostEndpointRequestDef = void 0;
const _1 = require("./$");
const oauth2_redirect_localhost_endpoint_does_not_exist_1 = require("./oauth2-redirect-localhost-endpoint-does-not-exist");
exports.waitForOauth2RedirectLocalhostEndpointRequestDef = _1.__.defineManagedFunc()
    .name(`waitForOauth2RedirectLocalhostEndpointRequest`)
    .namespace(_1.$namespace)
    .errorTypes([
    oauth2_redirect_localhost_endpoint_does_not_exist_1.Oauth2RedirectLocalhostEndpointDoesNotExist,
])
    .async()
    .executeType().waitForOauth2RedirectLocalhostEndpointRequestDef;
//# sourceMappingURL=wait-for-oauth2-redirect-localhost-endpoint-request-def.js.map