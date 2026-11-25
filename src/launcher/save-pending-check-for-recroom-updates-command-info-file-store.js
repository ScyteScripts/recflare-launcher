"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.savePendingCheckForRecRoomUpdatesCommandInfoFileStore = void 0;
const _1 = require("./$");
const get_pending_check_for_recroom_updates_command_info_file_store_info_1 = require("./get-pending-check-for-recroom-updates-command-info-file-store-info");
exports.savePendingCheckForRecRoomUpdatesCommandInfoFileStore = _1.__.declareManagedFunc()
    .name(`savePendingCheckForRecRoomUpdatesCommandInfoFileStore`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async ({ pendingCheckForRecRoomUpdatesCommandInfo, }) => {
    await _1.__.saveFileStore({
        fileStoreInfo: (0, get_pending_check_for_recroom_updates_command_info_file_store_info_1.getPendingCheckForRecRoomUpdatesCommandInfoFileStoreInfo)(),
        fileStoreData: pendingCheckForRecRoomUpdatesCommandInfo,
    });
}).savePendingCheckForRecRoomUpdatesCommandInfoFileStore;
//# sourceMappingURL=save-pending-check-for-recroom-updates-command-info-file-store.js.map