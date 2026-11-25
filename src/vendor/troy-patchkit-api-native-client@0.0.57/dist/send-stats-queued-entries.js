"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.sendStatsQueuedEntries = void 0;
const _1 = require("./$");
const stats_queued_entries_info_1 = require("./stats-queued-entries-info");
const try_sending_stats_next_queued_entry_1 = require("./try-sending-stats-next-queued-entry");
exports.sendStatsQueuedEntries = _1.__.declareManagedFunc()
    .name(`sendStatsQueuedEntries`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    while (stats_queued_entries_info_1.STATS_QUEUED_ENTRIES_INFO.value.length > 0) {
        await (0, try_sending_stats_next_queued_entry_1.trySendingStatsNextQueuedEntry)();
        await _1.__.wait(1000);
    }
}).sendStatsQueuedEntries;
//# sourceMappingURL=send-stats-queued-entries.js.map