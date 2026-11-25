"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchRecRoomBranchLastStudioUnityProjectDirPath = void 0;
// eslint-disable-next-line @typescript-eslint/tslint/config
const $winreg = require("winreg");
const _1 = require("./$");
const create_winreg_handle_1 = require("./create-winreg-handle");
exports.fetchRecRoomBranchLastStudioUnityProjectDirPath = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchRecRoomBranchLastStudioUnityProjectDirPathDef)
    .execute(async ({ recRoomBranchId, }) => {
    const winRegHandle = await (0, create_winreg_handle_1.createWinRegHandle)();
    return await new Promise((resolvePromise, rejectPromise) => {
        try {
            winRegHandle.get(`recroom_branch_${recRoomBranchId}_last_studio_unity_project_dir_path`, (e, winRegItem) => {
                if (e === undefined || e === null) {
                    if (winRegItem !== null
                        &&
                            winRegItem.type === $winreg.REG_SZ
                        &&
                            winRegItem.value !== ``) {
                        resolvePromise(winRegItem.value);
                    }
                    else {
                        resolvePromise(undefined);
                    }
                }
                else {
                    resolvePromise(undefined);
                }
            });
        }
        catch (e) {
            rejectPromise(e);
        }
    });
}).fetchRecRoomBranchLastStudioUnityProjectDirPath;
//# sourceMappingURL=fetch-recroom-branch-last-studio-unity-project-dir-path.js.map