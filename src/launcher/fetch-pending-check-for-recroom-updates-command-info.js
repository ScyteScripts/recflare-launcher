"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchPendingCheckForRecRoomUpdatesCommandInfo = void 0;
const _1 = require("./$");
const get_pending_check_for_recroom_updates_command_info_file_store_info_1 = require("./get-pending-check-for-recroom-updates-command-info-file-store-info");
exports.fetchPendingCheckForRecRoomUpdatesCommandInfo = _1.__.declareManagedFunc()
    .name(`fetchPendingCheckForRecRoomUpdatesCommandInfo`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const pendingCheckForRecRoomUpdatesCommandInfoFileStoreContent = await _1.__.fetchFileStoreContent({
        fileStoreInfo: (0, get_pending_check_for_recroom_updates_command_info_file_store_info_1.getPendingCheckForRecRoomUpdatesCommandInfoFileStoreInfo)(),
    });
    if (pendingCheckForRecRoomUpdatesCommandInfoFileStoreContent === undefined) {
        return undefined;
    }
    if (pendingCheckForRecRoomUpdatesCommandInfoFileStoreContent.version === undefined) {
        _1.__.checkInvalidAssertion();
    }
    if (pendingCheckForRecRoomUpdatesCommandInfoFileStoreContent.version === 1) {
        return pendingCheckForRecRoomUpdatesCommandInfoFileStoreContent.data;
    }
    _1.__.checkInvalidAssertion();
}).fetchPendingCheckForRecRoomUpdatesCommandInfo;
//# sourceMappingURL=fetch-pending-check-for-recroom-updates-command-info.js.map