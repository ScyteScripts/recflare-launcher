"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getDiskSpaceForPath = void 0;
const _1 = require("./$");
const check_disk_space_1 = require("check-disk-space");
exports.getDiskSpaceForPath = _1.__.declareManagedFunc()
    .async()
    .def(_1._.getDiskSpaceForPathDef)
    .execute(async ({ installPath }) => {
    const result = await new Promise((resolvePromise, _) => {
        (0, check_disk_space_1.default)(installPath).then((diskSpace) => {
            resolvePromise(diskSpace.free);
        });
    });
    return result;
}).getDiskSpaceForPath;
//# sourceMappingURL=get-disk-space-for-path.js.map