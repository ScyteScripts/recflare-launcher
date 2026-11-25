"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchOngoingInstallUnityEditorTaskState = void 0;
const _1 = require("./$");
const ongoing_install_unity_editor_task_state_1 = require("./ongoing-install-unity-editor-task-state");
exports.fetchOngoingInstallUnityEditorTaskState = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchOngoingInstallUnityEditorTaskStateDef)
    .execute(async () => {
    return ongoing_install_unity_editor_task_state_1.ONGOING_INSTALL_UNITY_EDITOR_TASK_STATE.value;
}).fetchOngoingInstallUnityEditorTaskState;
//# sourceMappingURL=fetch-ongoing-install-unity-editor-task-state.js.map