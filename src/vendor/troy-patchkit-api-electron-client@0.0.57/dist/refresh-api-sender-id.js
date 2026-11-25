"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.refreshApiSenderId = void 0;
const _1 = require("./$");
exports.refreshApiSenderId = _1.__.declareManagedFunc()
    .name(`refreshApiSenderId`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .async()
    .execute(async () => {
    const apiSenderIdFileDirPath = _1.__.invokeSyncExp(() => {
        const currentOperatingSystemPlatform = _1.__.getCurrentOperatingSystemPlatform();
        if (currentOperatingSystemPlatform === _1.__.OperatingSystemPlatform.Windows) {
            _1.__.checkAssertion(typeof process.env[`LOCALAPPDATA`] === `string`);
            return _1.__.$path.resolve(process.env[`LOCALAPPDATA`], `PatchKit`);
        }
        else if (currentOperatingSystemPlatform === _1.__.OperatingSystemPlatform.MacOS) {
            return _1.__.$path.resolve(_1.__.$electron.app.getPath(`home`), `Application Support/PatchKit`);
        }
        else if (currentOperatingSystemPlatform === _1.__.OperatingSystemPlatform.Linux) {
            return _1.__.$path.resolve(_1.__.$electron.app.getPath(`userData`), `../PatchKit`);
        }
        else {
            _1.__.checkInvalidAssertion();
        }
    });
    const apiSenderIdFilePath = _1.__.$path.resolve(apiSenderIdFileDirPath, `sender_id`);
    let apiSenderIdFileContents;
    if (_1.__.$fs.existsSync(apiSenderIdFilePath)) {
        apiSenderIdFileContents = await _1.__.$fspromises.readFile(apiSenderIdFilePath, `utf8`);
    }
    if (apiSenderIdFileContents !== undefined
        &&
            apiSenderIdFileContents.length === 32) {
        _1._.API_SENDER_ID.value = apiSenderIdFileContents;
    }
    else {
        _1._.API_SENDER_ID.value = _1.__.$uuid.v4().replace(new RegExp(`-`, `g`), ``);
        await _1.__.$fspromises.mkdir(apiSenderIdFileDirPath, {
            recursive: true,
        });
        await _1.__.$fspromises.writeFile(apiSenderIdFilePath, _1._.API_SENDER_ID.value, `utf8`);
    }
}).refreshApiSenderId;
//# sourceMappingURL=refresh-api-sender-id.js.map