"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.initLibpkapps = void 0;
const _1 = require("./$");
const convert_string_or_undefined_to_libpkapps_binding_nullable_string_1 = require("./convert-string-or-undefined-to-libpkapps-binding-nullable-string");
const convert_path_to_libpkapps_binding_path_1 = require("./convert-path-to-libpkapps-binding-path");
const convert_string_to_libpkapps_binding_string_1 = require("./convert-string-to-libpkapps-binding-string");
const libpkapps_binding_1 = require("./libpkapps-binding");
const libpkapps_dir_path_1 = require("./libpkapps-dir-path");
exports.initLibpkapps = _1.__.declareManagedFunc()
    .name(`initLibpkapps`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ libpkappsLogsDirPath, }) => {
    _1.__.checkAssertion(libpkapps_binding_1.LIBPKAPPS_BINDING.value !== undefined);
    libpkapps_binding_1.LIBPKAPPS_BINDING.value.initLibpkapps((0, convert_string_to_libpkapps_binding_string_1.convertStringToLibpkappsBindingString)({
        string: _1.__.invokeSyncExp(() => {
            _1.__.checkAssertion(libpkapps_dir_path_1.LIBPKAPPS_DIR_PATH.value !== undefined);
            const libpkappsDirNormalizedPath = _1.__.$path.normalize(libpkapps_dir_path_1.LIBPKAPPS_DIR_PATH.value);
            switch (_1.__.getCurrentOperatingSystemPlatform()) {
                case _1.__.OperatingSystemPlatform.Windows:
                    return libpkappsDirNormalizedPath.replace(`/`, `\\`);
                case _1.__.OperatingSystemPlatform.MacOS:
                case _1.__.OperatingSystemPlatform.Linux:
                    return libpkappsDirNormalizedPath;
            }
        }),
    }), (0, convert_path_to_libpkapps_binding_path_1.convertPathToLibpkappsBindingPath)({
        path: libpkappsLogsDirPath,
    }), _1.__.invokeSyncExp(() => {
        switch (_1.__.getCurrentOperatingSystemPlatform()) {
            case _1.__.OperatingSystemPlatform.Windows:
                return 2;
            case _1.__.OperatingSystemPlatform.MacOS:
                return 1;
            case _1.__.OperatingSystemPlatform.Linux:
                return 0;
        }
    }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
        string: _1._.API_CLIENT_ID.value,
    }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
        string: _1._.MAIN_SERVICE_API_URL.value,
    }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
        string: _1._.MAIN_SERVICE_API_CACHE_URL.value,
    }), (0, convert_string_or_undefined_to_libpkapps_binding_nullable_string_1.convertStringOrUndefinedToLibpkappsBindingNullableString)({
        string: _1._.KEYS_SERVICE_API_URL.value,
    }));
}).initLibpkapps;
//# sourceMappingURL=init-libpkapps.js.map