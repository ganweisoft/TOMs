import { createVNode, render } from 'vue';

import Main from './index.vue';

let instance;
let seed = 1;
const container = document.createElement('div')

function clone (cloneObj, source) {
    for (let prop in source) {
        if (Object.prototype.hasOwnProperty.call(source, prop)) {
            let value = source[prop];
            if (value !== undefined) {
                cloneObj[prop] = value;
            }
        }
    }
    return cloneObj;
}

const Toast = function (options) {
    options = clone({ visible: true }, options);
    const id = 'Toast_' + seed++;
    const userOnClose = options.onClose;

    options.onClose = function () {
        render(null, container);
        if (typeof userOnClose === 'function') {
            instance ? userOnClose(instance) : userOnClose();
        }
        instance = null;
    };

    if (instance) {
        instance.close();
        instance = null;
    }
    const ToastConstructor = {
        extends: Main,
        data: () => options
    };
    instance = createVNode(ToastConstructor);
    render(instance, container)
    document.body.appendChild(container.firstElementChild)

    return instance;
};

['success', 'error', 'warning', 'info'].forEach(type => {
    Toast[type] = options => {
        if (typeof options === 'string') {
            options = {
                message: options
            };
        }
        options.type = type;
        return Toast(options);
    };
});

Toast.close = function (userOnClose) {
    instance && instance.component.ctx.close();
};

export default Toast;
