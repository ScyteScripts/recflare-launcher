"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.wait = void 0;
const wait = async (ms) => {
    return new Promise((resolve) => {
        setTimeout(resolve, ms);
    });
};
exports.wait = wait;
//# sourceMappingURL=wait.js.map