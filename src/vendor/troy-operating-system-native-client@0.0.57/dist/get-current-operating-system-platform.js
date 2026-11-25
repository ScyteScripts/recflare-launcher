"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getCurrentOperatingSystemPlatform = void 0;
const _1 = require("./$");
exports.getCurrentOperatingSystemPlatform = _1.__.declareManagedFunc()
    .name(`getCurrentOperatingSystemPlatform`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    if (_1.__.$os.platform() === `win32`) {
        return _1.__.OperatingSystemPlatform.Windows;
    }
    if (_1.__.$os.platform() === `darwin`) {
        return _1.__.OperatingSystemPlatform.MacOS;
    }
    if (_1.__.$os.platform() === `linux`) {
        return _1.__.OperatingSystemPlatform.Linux;
    }
    _1.__.checkInvalidAssertion();
}).getCurrentOperatingSystemPlatform;
//# sourceMappingURL=get-current-operating-system-platform.js.map