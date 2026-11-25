"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.deserializeValueFromJson = void 0;
const _1 = require("./$");
exports.deserializeValueFromJson = _1.__.declareManagedFunc()
    .name(`deserializeValueFromJson`)
    .namespace(_1.$namespace)
    .errorTypes([])
    .sync()
    .execute(({ json, }) => {
    const jsonReviver = (_objectFieldName, objectFieldValueAsJson) => {
        const isoDateRegex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$/;
        const msAjaxDateRegex = /^\/Date\((d|-|.*)\)[\/|\\]$/;
        if (typeof objectFieldValueAsJson === `string`) {
            let a = isoDateRegex.exec(objectFieldValueAsJson);
            // tslint:disable-next-line: strict-boolean-expressions
            if (a) {
                return new Date(objectFieldValueAsJson);
            }
            a = msAjaxDateRegex.exec(objectFieldValueAsJson);
            // tslint:disable-next-line: strict-boolean-expressions
            if (a) {
                // @ts-expect-error
                const b = a[1].split(/[-+,.]/);
                // @ts-expect-error
                // tslint:disable-next-line: strict-boolean-expressions
                return new Date(b[0] ? +b[0] : 0 - +b[1]);
            }
        }
        return objectFieldValueAsJson;
    };
    return JSON.parse(json, jsonReviver);
}).deserializeValueFromJson;
exports.deserializeValueFromJson.config.isLoggingEnabled = false;
//# sourceMappingURL=deserialize-value-from-json.js.map