"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.enqueueStatsEntry = void 0;
const _1 = require("./$");
const stats_queued_entries_info_1 = require("./stats-queued-entries-info");
exports.enqueueStatsEntry = _1.__.declareManagedFunc()
    .name(`enqueueStatsEntry`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ statsEntryInfo, }) => {
    stats_queued_entries_info_1.STATS_QUEUED_ENTRIES_INFO.value.push(statsEntryInfo);
}).enqueueStatsEntry;
//# sourceMappingURL=enqueue-stats-entry.js.map