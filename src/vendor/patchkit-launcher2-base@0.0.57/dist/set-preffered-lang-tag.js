"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.setPreferredLangTag = void 0;
const _1 = require("./$");
const fetch_settings_1 = require("./fetch-settings");
const save_settings_file_store_1 = require("./save-settings-file-store");
exports.setPreferredLangTag = _1.__.declareManagedFunc()
    .async()
    .def(_1._.setPreferredLangTagDef)
    .execute(async ({ preferredLangTag, }) => {
    const settings = await (0, fetch_settings_1.fetchSettings)();
    await (0, save_settings_file_store_1.saveSettingsFileStore)({
        settings: _1.__.$deepmerge(settings, {
            preferredLangTag,
        }),
    });
}).setPreferredLangTag;
//# sourceMappingURL=set-preffered-lang-tag.js.map