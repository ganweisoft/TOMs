import { ElMessage } from 'element-plus';

let defaultOptions = {
    duration: 1500,
}

let notify = function (options) {
    let defaultClass = options.type;
    options = Object.assign({}, defaultOptions, options)
    options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
    ElMessage(options);
}

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

export default notify;

let useMessage = notify;
export {useMessage} ;

export function PartialNotify (app, moduleName) {
    let i18n = app?.config?.globalProperties
    let initModuleName = moduleName
    let newNotify = function (options) {
        let defaultClass = options.type;
        options = Object.assign({}, defaultOptions, options)
        options.customClass = options.customClass ? `${options.customClass} ${defaultClass}` : defaultClass;
        ElMessage(options);
    };
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
                ElMessage = ElMessage.ElMessage
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
                message: `<p>${(err.data && err.data.ElMessage || err.data && err.data.ElMessage) || ''}</p><p>${i18n.$t('login.interface.public.moduleName')}：${moduleName}</p><p>${i18n.$t('login.interface.public.interfaceName')}：${err?.config?.url} </p>`
            };
            notify.error(options)
        }
    };

    newNotify.closeAll = function () {
        return ElMessage.closeAll()
    }

    return newNotify
}