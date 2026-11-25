"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createWinRegHandle = void 0;
// eslint-disable-next-line @typescript-eslint/tslint/config
const $winreg = require("winreg");
const _1 = require("./$");
exports.createWinRegHandle = _1.__.declareManagedFunc()
    .name(`createWinRegHandle`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    _1.__.checkAssertion(_1.__.PatchKitLauncher2.CONFIG.value !== undefined);
    const winRegHandle = new $winreg({
        hive: $winreg.HKCU,
        key: `\\Software\\Rec Room Inc.\\${_1.__.PatchKitLauncher2.CONFIG.value.name}`,
    });
    await new Promise((resolvePromise, rejectPromise) => {
        try {
            winRegHandle.create((e) => {
                if (e === undefined || e === null) {
                    resolvePromise();
                }
                else {
                    rejectPromise(e);
                }
            });
        }
        catch (e) {
            rejectPromise(e);
        }
    });
    return winRegHandle;
}).createWinRegHandle;
//# sourceMappingURL=create-winreg-handle.js.map