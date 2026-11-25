"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.declareGlobalImmutableBox = exports.declareGlobalMutableBox = void 0;
const declareGlobalMutableBox = (globalMutableBoxInitialValue) => {
    return {
        value: globalMutableBoxInitialValue,
    };
};
exports.declareGlobalMutableBox = declareGlobalMutableBox;
const declareGlobalImmutableBox = (globalImmutableBoxValue) => {
    return {
        value: globalImmutableBoxValue,
    };
};
exports.declareGlobalImmutableBox = declareGlobalImmutableBox;
//# sourceMappingURL=box.js.map