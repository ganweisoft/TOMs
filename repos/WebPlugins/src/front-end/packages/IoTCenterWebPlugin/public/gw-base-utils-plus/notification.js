import { Notification } from 'element-ui';

let defaultOptions = {
    duration: 1000,
}

let notify = function (options) {
    let defaultClass = options.type;
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    Notification(options);
}

notify.success = function (options) {
    let defaultClass = 'success';
    if (typeof options == 'string') {
        options = { title: options };
    }
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    Notification.success(options);
}
notify.warning = function (options) {
    let defaultClass = 'warning';
    if (typeof options == 'string') {
        options = { title: options };
    }
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    Notification.warning(options);
}
notify.error = function (options) {
    let defaultClass = 'error';
    if (typeof options == 'string') {
        options = { title: options };
    }
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    Notification.error(options);
}
notify.info = function (options) {
    let defaultClass = 'info';
    if (typeof options == 'string') {
        options = { title: options };
    }
    options = Object.assign({}, defaultOptions, options);
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    Notification.info(options);
}
notify.closeAll = function () {
    return Notification.closeAll()
}

export default notify

export function PartialNotify (i18n, moduleName) {
    let initModuleName = moduleName
    let newNotify = function (options) {
        let defaultClass = options.type;
        options = Object.assign({}, defaultOptions, options)
        options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
        Notification(options);
    };
    ['success', 'warning', 'info'].forEach(function (type) {
        newNotify[type] = notify[type];
    })

    newNotify.error = function (message, err, manuTipsName) {
        if (!message) {
            return;
        }
        let options = {}
        if (err == undefined) {
            options = {
                type: 'error',
                title: message,
            }
            notify.error(options)
        } else if (typeof err == "object") {
            if (typeof message == "object") {
                message = message.message
            }
            if (!err.config || err.status == 404) {
                throw (err);
            }
            let moduleName = manuTipsName || i18n.$t(initModuleName) || sessionStorage.menuActiveName;
            if (!moduleName) {
                let arr = window.top.location.href.split('/');
                moduleName = arr.filter(item => item.includes('ganwei-iotcenter-'))[0]
            }
            options = {
                type: 'error',
                title: message || err.data && err.data.message,
                dangerouslyUseHTMLString: true,
                message: `<p>${i18n.$t('login.interface.public.moduleName')}：${moduleName}</p><p>${i18n.$t('login.interface.public.interfaceName')}：${err.config.url} </p>`
            };
            notify.error(options)
        }
    };

    newNotify.closeAll = function () {
        return Notification.closeAll()
    }

    return newNotify
}