"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.trySendingStatsNextQueuedEntry = void 0;
const _1 = require("./$");
const is_sending_stats_entry_1 = require("./is-sending-stats-entry");
const send_stats_entry_1 = require("./send-stats-entry");
const stats_queued_entries_info_1 = require("./stats-queued-entries-info");
exports.trySendingStatsNextQueuedEntry = _1.__.declareManagedFunc()
    .name(`trySendingStatsNextQueuedEntry`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    if (is_sending_stats_entry_1.IS_SENDING_STATS_ENTRY.value) {
        return;
    }
    const statsEntryInfo = stats_queued_entries_info_1.STATS_QUEUED_ENTRIES_INFO.value.shift();
    if (statsEntryInfo === undefined) {
        return;
    }
    is_sending_stats_entry_1.IS_SENDING_STATS_ENTRY.value = true;
    try {
        let wasRequestSuccessful = false;
        while (!wasRequestSuccessful) {
            try {
                await (0, send_stats_entry_1.sendStatsEntry)({
                    statsEntryInfo,
                });
                wasRequestSuccessful = true;
            }
            catch (_a) {
            }
            if (!wasRequestSuccessful) {
                await _1.__.wait(5000);
            }
        }
    }
    finally {
        is_sending_stats_entry_1.IS_SENDING_STATS_ENTRY.value = false;
    }
}).trySendingStatsNextQueuedEntry;
exports.trySendingStatsNextQueuedEntry.config.isLoggingEnabled = false;
//# sourceMappingURL=try-sending-stats-next-queued-entry.js.map