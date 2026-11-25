"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchFileStoreContent = void 0;
const _1 = require("./$");
exports.fetchFileStoreContent = _1.__.declareManagedFunc()
    .name(`fetchFileStoreContent`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ fileStoreInfo, }) => {
    if (_1.__.$fs.existsSync(fileStoreInfo.contentFilePath)) {
        const fileStoreContentAsJson = await _1.__.$fspromises.readFile(fileStoreInfo.contentFilePath, `utf8`);
        return _1.__.deserializeValueFromJson({
            json: fileStoreContentAsJson,
        });
    }
    if (fileStoreInfo.unversionedContentDataFilePath !== undefined) {
        if (_1.__.$fs.existsSync(fileStoreInfo.unversionedContentDataFilePath)) {
            const fileStoreContentDataAsJson = await _1.__.$fspromises.readFile(fileStoreInfo.unversionedContentDataFilePath, `utf8`);
            const fileStoreContentData = _1.__.deserializeValueFromJson({
                json: fileStoreContentDataAsJson,
            });
            const fileStoreContent = {
                data: fileStoreContentData,
            };
            return fileStoreContent;
        }
    }
    return undefined;
}).fetchFileStoreContent;
exports.fetchFileStoreContent.config.isLoggingEnabled = false;
//# sourceMappingURL=fetch-file-store-content.js.map