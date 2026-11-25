"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createLibpkappsBinding = void 0;
const _1 = require("./$");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_dir_path_1 = require("./libpkapps-dir-path");
exports.createLibpkappsBinding = _1.__.declareManagedFunc()
    .name(`createLibpkappsBinding`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(() => {
    _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value === undefined);
    _1.__.checkAssertion(libpkapps_dir_path_1.LIBPKAPPS_DIR_PATH.value !== undefined);
    libpkapps_binding_1.LIBPKAPPS_BINDING.value = require("@recovered/updater").createBinding();
}).createLibpkappsBinding;
