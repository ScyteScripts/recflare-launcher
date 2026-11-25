"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getManagedErrorFullname = exports.getManagedErrorTypeFullname = exports.declareManagedErrorType = exports.ManagedError = void 0;
class ManagedError extends Error {
    constructor() {
        super();
    }
}
exports.ManagedError = ManagedError;
const declareManagedErrorType = () => {
    return {
        name: (managedErrorTypeName) => {
            return {
                namespace: (managedErrorTypeNamespace) => {
                    const managedErrorType = class extends ManagedError {
                        constructor() {
                            super();
                            this.name = managedErrorTypeName;
                            Object.setPrototypeOf(this, managedErrorType.prototype);
                        }
                    };
                    Object.defineProperty(managedErrorType, `name`, {
                        writable: false,
                        value: managedErrorTypeName,
                    });
                    Object.defineProperty(managedErrorType, `namespace`, {
                        writable: false,
                        value: managedErrorTypeNamespace,
                    });
                    return {
                        [managedErrorTypeName]: managedErrorType,
                    };
                },
            };
        },
    };
};
exports.declareManagedErrorType = declareManagedErrorType;
const getManagedErrorTypeFullname = (managedErrorType) => {
    if (managedErrorType.namespace === undefined) {
        return managedErrorType.name;
    }
    return `${managedErrorType.namespace}.${managedErrorType.name}`;
};
exports.getManagedErrorTypeFullname = getManagedErrorTypeFullname;
const getManagedErrorFullname = (managedError) => {
    return (0, exports.getManagedErrorTypeFullname)(managedError.constructor);
};
exports.getManagedErrorFullname = getManagedErrorFullname;
//# sourceMappingURL=managed-error.js.map