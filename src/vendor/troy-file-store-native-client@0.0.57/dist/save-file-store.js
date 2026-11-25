"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.saveFileStore = void 0;
const _1 = require("./$");
exports.saveFileStore = _1.__.declareManagedFunc()
    .name(`saveFileStore`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ fileStoreInfo, fileStoreData, }) => {
    const fileStoreContent = {
        version: fileStoreInfo.currentVersion,
        data: fileStoreData,
    };
    const fileStoreContentAsJson = _1.__.serializeValueToJson({
        value: fileStoreContent,
    });
    await _1.__.$fspromises.writeFile(fileStoreInfo.contentFilePath, fileStoreContentAsJson, `utf8`);
}).saveFileStore;
//# sourceMappingURL=save-file-store.js.map