"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.killAppBranchProcess = void 0;
const _1 = require("./$");
const apps_branches_process_info_box_1 = require("./apps-branches-process-info-box");
exports.killAppBranchProcess = _1.__.declareManagedFunc()
    .async()
    .def(_1._.killAppBranchProcessDef)
    .execute(async ({ appId, appBranchId, }) => {
    const appBranchProcessInfoBox = _1.__.invokeSyncExp(() => {
        var _a, _b;
        var _c;
        if (((_a = apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === undefined) {
            (_b = (_c = apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
            apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId][appBranchId] = {
                value: undefined,
            };
        }
        return apps_branches_process_info_box_1.APPS_BRANCHES_PROCESS_INFO_BOX.value[appId][appBranchId];
    });
    if (appBranchProcessInfoBox.value === undefined) {
        throw new _1._.AppBranchProcessDoesNotExist();
    }
    const appBranchProcessInfo = appBranchProcessInfoBox.value;
    await new Promise((resolvePromise, rejectPromise) => {
        _1.__.$treekill(appBranchProcessInfo.id, (e) => {
            if (e === undefined || e === null) {
                resolvePromise();
            }
            else {
                rejectPromise(e);
            }
        });
    });
    appBranchProcessInfoBox.value = undefined;
}).killAppBranchProcess;
//# sourceMappingURL=kill-app-branch-process.js.map