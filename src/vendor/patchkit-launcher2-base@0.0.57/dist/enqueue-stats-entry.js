"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.enqueueStatsEntry = void 0;
const _1 = require("./$");
const config_1 = require("./config");
exports.enqueueStatsEntry = _1.__.declareManagedFunc()
    .name(`enqueueStatsEntry`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ statsEntryTypeId, statsEntryPayload, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    if (!config_1.CONFIG.value.areStatsEntriesAnonymous) {
        return;
    }
    _1.__.PatchKit.enqueueStatsEntry({
        statsEntryInfo: {
            typeId: statsEntryTypeId,
            isAnonymous: true,
            payload: statsEntryPayload,
        },
    });
}).enqueueStatsEntry;
//# sourceMappingURL=enqueue-stats-entry.js.map