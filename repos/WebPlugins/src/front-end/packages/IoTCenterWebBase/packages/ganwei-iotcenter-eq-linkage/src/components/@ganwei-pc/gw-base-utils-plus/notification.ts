import { ElMessage, MessageOptions } from 'element-plus';
import { App, inject, InjectionKey } from 'vue'

let defaultOptions = {
    duration: 1500,
}

export interface INotify {
    (options: MessageOptions): void;
    success(options: MessageOptions | string): void;
    warning(options: MessageOptions | string): void;
    error(options: MessageOptions | string): void;
    info(options: MessageOptions | string): void;
    closeAll(): void
}

export interface IPartialNotify extends Omit<INotify, 'error'> {
    (options: MessageOptions & { title: string }): void;
    error(ElMessage, err?: any, manuTipsName?: string): void;
    closeAll(): void
}

let notify = function (options) {
    let defaultClass = options.type;
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    ElMessage(options);
} as INotify

notify.success = function (options) {
    let defaultClass = 'success';
    if (typeof options == 'string') {
        options = { message: options };
    }
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    ElMessage.success(options);
}
notify.warning = function (options) {
    let defaultClass = 'warning';
    if (typeof options == 'string') {
        options = { message: options };
    }
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    ElMessage.warning(options);
}
notify.error = function (options) {
    let defaultClass = 'error';
    if (typeof options == 'string') {
        options = { message: options };
    }
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    ElMessage.error(options);
}
notify.info = function (options) {
    let defaultClass = 'info';
    if (typeof options == 'string') {
        options = { message: options };
    }
    options = Object.assign({}, defaultOptions, options);
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    ElMessage.info(options);
}
notify.closeAll = function () {
    return ElMessage.closeAll()
}

export default notify

export function PartialNotify(app, moduleName) {
    let i18n = app?.config?.globalProperties
    let initModuleName = moduleName
    let newNotify = function (options) {
        let defaultClass = options.type;
        options = Object.assign({}, defaultOptions, options)
        options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
        options.message = options.title || options.message || ''
        ElMessage(options);
    } as IPartialNotify;
    ['success', 'warning', 'info'].forEach(function (type) {
        newNotify[type] = notify[type];
    })

    newNotify.error = function (ElMessage, err, manuTipsName) {
        let options = {}
        if (err == undefined) {
            options = {
                type: 'error',
                message: ElMessage,
            }
            notify.error(options)
        } else if (typeof err == "object") {
            if (typeof ElMessage == "object") {
                ElMessage = ElMessage.message
            }
            if (!err.config || err.status == 404) {
                console.log(err)
                return;
            }
            let moduleName = manuTipsName || i18n.$t(initModuleName) || sessionStorage.menuActiveName;
            if (!moduleName) {
                let arr = window.top.location.href.split('/');
                moduleName = arr.filter(item => item.includes('ganwei-iotcenter-'))[0]
            }
            options = {
                type: 'error',
                dangerouslyUseHTMLString: true,
                message: `<p>${(err.data && err.data.message) || ''}</p><p>${i18n.$t('login.interface.public.moduleName')}：${moduleName}</p><p>${i18n.$t('login.interface.public.interfaceName')}：${err?.config?.url} </p>`
            };
            notify.error(options)
        }
    };

    newNotify.closeAll = function () {
        return ElMessage.closeAll()
    }

    return newNotify
}
export const $messageKey: InjectionKey<IPartialNotify> = Symbol('$message')

export function installMessage(app: App) {
    app.provide($messageKey, app.config.globalProperties.$message)
}
export function useMessage() {
    const $message = inject($messageKey)
    if ($message === undefined) {
        throw new Error('[useMessage] Cannot use $message without [installMessage].')
    }
    return $message
}
