"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchCurrentLangTag = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const fetch_settings_1 = require("./fetch-settings");
exports.fetchCurrentLangTag = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchCurrentLangTagDef)
    .execute(async () => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const settings = await (0, fetch_settings_1.fetchSettings)();
    let currentLangTag;
    if (settings.prefferedLangTag !== undefined) {
        currentLangTag = _1.__.$bcp47match.lookup(Object.keys(config_1.CONFIG.value.availableLangs), settings.prefferedLangTag);
    }
    if (currentLangTag === undefined) {
        currentLangTag = _1.__.$bcp47match.lookup(Object.keys(config_1.CONFIG.value.availableLangs), _1.__.$electron.app.getLocale());
    }
    return currentLangTag !== null && currentLangTag !== void 0 ? currentLangTag : config_1.CONFIG.value.fallbackLangTag;
}).fetchCurrentLangTag;
//# sourceMappingURL=fetch-current-lang-tag.js.map