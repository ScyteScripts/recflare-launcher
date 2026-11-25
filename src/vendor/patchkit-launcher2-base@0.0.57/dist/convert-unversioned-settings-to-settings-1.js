"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.convertUnversionedSettingsToSettings1 = void 0;
const _1 = require("./$");
exports.convertUnversionedSettingsToSettings1 = _1.__.declareManagedFunc()
    .name(`convertUnversionedSettingsToSettings1`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ unversionedSettings, }) => {
    const appsSettings1 = {};
    if (unversionedSettings.apps !== undefined) {
        for (const [appId, appUnversionedSettings] of Object.entries(unversionedSettings.apps)) {
            if (appUnversionedSettings !== undefined) {
                const appSettings1 = {
                    branches: {},
                };
                if (appUnversionedSettings.branches !== undefined) {
                    for (const [appBranchId, appBranchUnversionedSettings] of Object.entries(appUnversionedSettings.branches)) {
                        if (appBranchUnversionedSettings !== undefined) {
                            appSettings1.branches[appBranchId] = {
                                rootDirPath: appBranchUnversionedSettings.dirPath,
                            };
                        }
                    }
                }
                else {
                    if (appUnversionedSettings.dirPath !== undefined) {
                        appSettings1.branches[_1._.APP_MAIN_BRANCH_ID.value] = {
                            rootDirPath: appUnversionedSettings.dirPath,
                        };
                    }
                }
                appsSettings1[appId] = appSettings1;
            }
        }
    }
    return {
        prefferedLangTag: unversionedSettings.languageTag,
        apps: appsSettings1,
    };
}).convertUnversionedSettingsToSettings1;
//# sourceMappingURL=convert-unversioned-settings-to-settings-1.js.map