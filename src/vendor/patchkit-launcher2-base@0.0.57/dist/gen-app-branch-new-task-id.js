"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.genAppBranchNewTaskId = void 0;
const _1 = require("./$");
const apps_branches_last_task_id_box_1 = require("./apps-branches-last-task-id-box");
exports.genAppBranchNewTaskId = _1.__.declareManagedFunc()
    .name(`genAppBranchNewTaskId`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ appId, appBranchId, }) => {
    const appBranchLastTaskIdBox = _1.__.invokeSyncExp(() => {
        var _a, _b;
        var _c;
        if (((_a = apps_branches_last_task_id_box_1.APPS_BRANCHES_LAST_TASK_ID_BOX.value[appId]) === null || _a === void 0 ? void 0 : _a[appBranchId]) === undefined) {
            (_b = (_c = apps_branches_last_task_id_box_1.APPS_BRANCHES_LAST_TASK_ID_BOX.value)[appId]) !== null && _b !== void 0 ? _b : (_c[appId] = {});
            apps_branches_last_task_id_box_1.APPS_BRANCHES_LAST_TASK_ID_BOX.value[appId][appBranchId] = {
                value: undefined,
            };
        }
        return apps_branches_last_task_id_box_1.APPS_BRANCHES_LAST_TASK_ID_BOX.value[appId][appBranchId];
    });
    const appBranchNewTaskId = appBranchLastTaskIdBox.value === undefined
        ? 0
        : appBranchLastTaskIdBox.value + 1;
    appBranchLastTaskIdBox.value = appBranchNewTaskId;
    return appBranchNewTaskId;
}).genAppBranchNewTaskId;
//# sourceMappingURL=gen-app-branch-new-task-id.js.map