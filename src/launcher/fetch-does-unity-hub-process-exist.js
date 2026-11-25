"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchDoesUnityHubProcessExist = void 0;
// eslint-disable-next-line @typescript-eslint/tslint/config
const $pslist = require("ps-list");
const _1 = require("./$");
exports.fetchDoesUnityHubProcessExist = _1.__.declareManagedFunc()
    .name(`fetchDoesUnityHubProcessExist`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const psListHandle = await $pslist();
    return psListHandle.some(p => /^(?:Unity Hub)(?:\.exe)?$/i.test(p.name));
}).fetchDoesUnityHubProcessExist;
//# sourceMappingURL=fetch-does-unity-hub-process-exist.js.map
