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
exports.PatchKit = exports.$fspromises = exports.$fs = exports.$zipafolder = exports.$uuid = exports.$png2icons = exports.$jsyaml = exports.$isrunning = exports.$electronupdater = exports.$electronlocalshortcut = exports.$electronappuniversalprotocolclient = exports.$crossfetch = exports.$bcp47match = void 0;
/* eslint-disable */
__exportStar(require("troy-file-store-native-client"), exports);
__exportStar(require("troy-foundation"), exports);
__exportStar(require("troy-json"), exports);
exports.$bcp47match = require("bcp-47-match");
exports.$crossfetch = require("cross-fetch");
exports.$commandlineargs = require("command-line-args");
exports.$deepmerge = require("deepmerge");
exports.$electronappuniversalprotocolclient = require("electron-app-universal-protocol-client");
exports.$electronlocalshortcut = require("electron-localshortcut");
exports.$electronupdater = require("electron-updater");
exports.$electron = require("electron");
// @ts-expect-error
exports.$isrunning = require("is-running");
exports.$jsyaml = require("js-yaml");
exports.$png2icons = require("png2icons");
exports.$treekill = require("tree-kill");
exports.$uuid = require("uuid");
exports.$zipafolder = require("zip-a-folder");
exports.$path = require("path");
exports.$fs = require("fs");
exports.$fspromises = require("fs/promises");
exports.PatchKit = require("./$patchkit-deps");
//# sourceMappingURL=$global-deps.js.map