"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.fetchRecRoomStudioDefaultBranchId = void 0;
const _1 = require("./$");
const recroom_studio_default_branch_id_1 = require("./recroom-studio-default-branch-id");
exports.fetchRecRoomStudioDefaultBranchId = _1.__.declareManagedFunc()
    .async()
    .def(_1._.fetchRecRoomStudioDefaultBranchIdDef)
    .execute(async () => {
    return recroom_studio_default_branch_id_1.REC_ROOM_STUDIO_DEFAULT_BRANCH_ID.value;
}).fetchRecRoomStudioDefaultBranchId;
//# sourceMappingURL=fetch-recroom-studio-default-branch-id.js.map