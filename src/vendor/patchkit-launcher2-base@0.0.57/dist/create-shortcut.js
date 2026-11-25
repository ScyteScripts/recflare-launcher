"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createShortcut = void 0;
const _1 = require("./$");
const config_1 = require("./config");
const get_user_data_dir_path_1 = require("./get-user-data-dir-path");
exports.createShortcut = _1.__.declareManagedFunc()
    .async()
    .def(_1._.createShortcutDef)
    .execute(async ({ shortcutPathTemplate, shortcutIconSource, shortcutUrl, }) => {
    _1.__.checkAssertion(config_1.CONFIG.value !== undefined);
    const shortcutIconArrayBuffer = await _1.__.invokeAsyncExp(async () => {
        switch (shortcutIconSource.type) {
            case _1._.ShortcutIconSourceType.ArrayBuffer: {
                return shortcutIconSource.value;
            }
            case _1._.ShortcutIconSourceType.Url: {
                const shortcutIconRequestResponse = await _1.__.$crossfetch.fetch(shortcutIconSource.value);
                _1.__.checkAssertion(shortcutIconRequestResponse.status === 200);
                return await shortcutIconRequestResponse.arrayBuffer();
            }
        }
    });
    const shortcutIcon = _1.__.$electron.nativeImage.createFromBuffer(Buffer.from(shortcutIconArrayBuffer));
    const shortcutIconAsPngBuffer = shortcutIcon.toPNG();
    switch (config_1.CONFIG.value.targetOperatingSystemPlatform) {
        case _1._.TargetOperatingSystemPlatform.Windows: {
            const shortcutIconFilePath = _1.__.$path.resolve((0, get_user_data_dir_path_1.getUserDataDirPath)(), `shortcut-icons`, `${_1.__.$uuid.v4()}.ico`);
            const shortcutIconAsIcoBuffer = _1.__.$png2icons.createICO(shortcutIconAsPngBuffer, _1.__.$png2icons.BILINEAR, 0, true);
            _1.__.checkAssertion(shortcutIconAsIcoBuffer !== null);
            _1.__.$fs.mkdirSync(_1.__.$path.dirname(shortcutIconFilePath), {
                recursive: true,
            });
            _1.__.$fs.writeFileSync(shortcutIconFilePath, shortcutIconAsIcoBuffer);
            const shortcutFilePath = _1.__.$path.resolve([
                shortcutPathTemplate
                    .split(`{defaultShortcutsDirPath}`).join(_1.__.$electron.app.getPath(`desktop`))
                    .split(`{desktopDirPath}`).join(_1.__.$electron.app.getPath(`desktop`)),
                `.url`,
            ].join(``));
            _1.__.$fs.mkdirSync(_1.__.$path.dirname(shortcutFilePath), {
                recursive: true,
            });
            _1.__.$fs.writeFileSync(shortcutFilePath, [
                `[{000214A0-0000-0000-C000-000000000046}]`,
                `Prop3=19,0`,
                `[InternetShortcut]`,
                `IDList=`,
                `IconIndex=0`,
                `URL=${shortcutUrl}`,
                `IconFile=${shortcutIconFilePath}`,
            ].join(`\n`));
            break;
        }
        case _1._.TargetOperatingSystemPlatform.MacOS: {
            const shortcutBundlePath = _1.__.$path.resolve([
                shortcutPathTemplate
                    .split(`{defaultShortcutsDirPath}`).join(`/Applications`)
                    .split(`{desktopDirPath}`).join(_1.__.$electron.app.getPath(`desktop`)),
                `.app`,
            ].join(``));
            const shortcutIconFilePath = _1.__.$path.resolve(shortcutBundlePath, `Contents`, `Resources`, `Icon.icns`);
            const shortcutIconAsIcnsBuffer = _1.__.$png2icons.createICNS(shortcutIconAsPngBuffer, _1.__.$png2icons.BILINEAR, 0);
            _1.__.checkAssertion(shortcutIconAsIcnsBuffer !== null);
            _1.__.$fs.mkdirSync(_1.__.$path.dirname(shortcutIconFilePath), {
                recursive: true,
            });
            _1.__.$fs.writeFileSync(shortcutIconFilePath, shortcutIconAsIcnsBuffer);
            const shortcutScriptFilePath = _1.__.$path.resolve(shortcutBundlePath, `Contents`, `MacOS`, `run.sh`);
            _1.__.$fs.mkdirSync(_1.__.$path.dirname(shortcutScriptFilePath), {
                recursive: true,
            });
            _1.__.$fs.writeFileSync(shortcutScriptFilePath, [
                `#!/bin/bash`,
                `open ${shortcutUrl}`,
            ].join(`\n`));
            _1.__.$fs.chmodSync(shortcutScriptFilePath, 0o755);
            const shortcutInfoPlistFilePath = _1.__.$path.resolve(shortcutBundlePath, `Contents`, `Info.plist`);
            _1.__.$fs.mkdirSync(_1.__.$path.dirname(shortcutInfoPlistFilePath), {
                recursive: true,
            });
            _1.__.$fs.writeFileSync(shortcutInfoPlistFilePath, [
                `<?xml version="1.0" encoding="UTF-8"?>`,
                `<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">`,
                `<plist version="1.0">`,
                `<dict>`,
                `  <key>CFBundleExecutable</key>`,
                `  <string>run.sh</string>`,
                `  <key>CFBundleIconFile</key>`,
                `  <string>Icon.icns</string>`,
                `  <key>CFBundleInfoDictionaryVersion</key>`,
                `  <string>1.0</string>`,
                `  <key>CFBundlePackageType</key>`,
                `  <string>APPL</string>`,
                `  <key>CFBundleSignature</key>`,
                `  <string>????</string>`,
                `  <key>CFBundleVersion</key>`,
                `  <string>1.0</string>`,
                `</dict>`,
                `</plist>`,
            ].join(`\n`));
            break;
        }
        case _1._.TargetOperatingSystemPlatform.Linux: {
            _1.__.checkInvalidAssertion(); // TODO:
        }
    }
}).createShortcut;
exports.createShortcut.config.isLoggingExecArgsEnabled = false;
//# sourceMappingURL=create-shortcut.js.map