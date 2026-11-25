"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.sendStatsEntry = void 0;
const _1 = require("./$");
exports.sendStatsEntry = _1.__.declareManagedFunc()
    .name(`sendStatsEntry`)
    .namespace(_1.$namespace)
    .errorTypes([
    _1._.ServiceConnectionIsNotAvailable,
])
    .async()
    .execute(async ({ statsEntryInfo, }) => {
    const requestUrl = `${_1._.STATS_SERVICE_API_URL.value}/v1/entry`;
    const requestBodyOperatingSystemFamily = _1.__.invokeSyncExp(() => {
        const currentOperatingSystemPlatform = _1.__.getCurrentOperatingSystemPlatform();
        if (currentOperatingSystemPlatform === _1.__.OperatingSystemPlatform.Windows) {
            return `windows`;
        }
        else if (currentOperatingSystemPlatform === _1.__.OperatingSystemPlatform.MacOS) {
            return `mac`;
        }
        else if (currentOperatingSystemPlatform === _1.__.OperatingSystemPlatform.Linux) {
            return `linux`;
        }
        else {
            _1.__.checkInvalidAssertion();
        }
    });
    const requestBody = {
        ...statsEntryInfo.payload,
        event_name: statsEntryInfo.typeId,
        caller: _1._.API_CLIENT_ID.value,
        sender_id: statsEntryInfo.isAnonymous
            ? `00000000000000000000000000000000`
            : _1._.API_SENDER_ID.value,
        operating_system_family: requestBodyOperatingSystemFamily,
        operating_system_version: _1.__.$os.release(),
    };
    const requestResponse = await _1.__.invokeAsyncExp(async () => {
        try {
            return await _1.__.$crossfetch.fetch(requestUrl, {
                method: `POST`,
                body: JSON.stringify(requestBody),
                headers: {
                    [`Content-Type`]: `application/json`,
                },
            });
        }
        catch (e) {
            _1.__.writeTraceLog({
                kind: `on-patchkit-api-service-request-failed`,
                patchKitApiServiceRequestErrorStack: e instanceof Error ? e.stack : undefined,
            });
            throw new _1._.ServiceConnectionIsNotAvailable();
        }
    });
    if (!requestResponse.ok) {
        throw new _1._.ServiceConnectionIsNotAvailable();
    }
}).sendStatsEntry;
//# sourceMappingURL=send-stats-entry.js.map