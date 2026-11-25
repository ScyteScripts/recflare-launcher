"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __exportStar = (this && this.__exportStar) || function(m, exports) {
    for (var p in m) if (p !== "default" && !Object.prototype.hasOwnProperty.call(exports, p)) __createBinding(exports, m, p);
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.$fspromises = exports.$fs = exports.$uuid = void 0;
/* eslint-disable */
__exportStar(require("troy-foundation"), exports);
__exportStar(require("troy-native-foundation"), exports);
__exportStar(require("troy-operating-system-native-client"), exports);
exports.$electron = require("electron");
exports.$uuid = require("uuid");
exports.$path = require("path");
exports.$fs = require("fs");
exports.$fspromises = require("fs/promises");
//# sourceMappingURL=$global-deps.js.map