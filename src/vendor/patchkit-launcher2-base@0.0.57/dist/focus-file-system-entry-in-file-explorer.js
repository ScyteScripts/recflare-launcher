"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.focusFileSystemEntryInFileExplorer = void 0;
const _1 = require("./$");
exports.focusFileSystemEntryInFileExplorer = _1.__.declareManagedFunc()
    .async()
    .def(_1._.focusFileSystemEntryInFileExplorerDef)
    .execute(async ({ fileSystemEntryPath, }) => {
    _1.__.$electron.shell.showItemInFolder(fileSystemEntryPath);
}).focusFileSystemEntryInFileExplorer;
//# sourceMappingURL=focus-file-system-entry-in-file-explorer.js.map