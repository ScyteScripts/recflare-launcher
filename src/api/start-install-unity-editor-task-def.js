"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.startInstallUnityEditorTaskDef = void 0;
const _1 = require("./$");
const ongoing_install_unity_editor_task_already_exists_1 = require("./ongoing-install-unity-editor-task-already-exists");
exports.startInstallUnityEditorTaskDef = _1.__.defineManagedFunc()
    .name(`startInstallUnityEditorTask`)
    .namespace(_1.$namespace)
    .errorTypes([
    ongoing_install_unity_editor_task_already_exists_1.OngoingInstallUnityEditorTaskAlreadyExists,
])
    .async()
    .executeType().startInstallUnityEditorTaskDef;
//# sourceMappingURL=start-install-unity-editor-task-def.js.map