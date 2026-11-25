"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.enqueueStatsGeneralEntry = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const enqueue_stats_entry_1 = require("./enqueue-stats-entry");
const stats_general_entry_type_1 = require("./stats-general-entry-type");
exports.enqueueStatsGeneralEntry = _1.__.declareManagedFunc()
    .name(`enqueueStatsGeneralEntry`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ statsGeneralEntryInfo, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const { statsEntryTypeId, } = _1.__.invokeSyncExp(() => {
        switch (statsGeneralEntryInfo.type) {
            case stats_general_entry_type_1.StatsGeneralEntryType.StatsStartedEntry: {
                return {
                    statsEntryTypeId: `mp_started`,
                };
            }
            case stats_general_entry_type_1.StatsGeneralEntryType.StatsQuittedEntry: {
                return {
                    statsEntryTypeId: `mp_closed`,
                };
            }
        }
    });
    (0, enqueue_stats_entry_1.enqueueStatsEntry)({
        statsEntryTypeId,
        statsEntryPayload: {
            apps_catalog_id: config_1.CONFIG.value.appsCatalogId,
        },
    });
}).enqueueStatsGeneralEntry;
//# sourceMappingURL=enqueue-stats-general-entry.js.map