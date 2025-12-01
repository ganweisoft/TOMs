import Vue from "vue";
import Main from "./index.vue";
const ContextMenuConstructor = Vue.extend(Main);

let instance;
let seed = 1;

function clone(cloneObj, source) {
    for (let prop in source) {
        if (source.hasOwnProperty(prop)) {
            let value = source[prop];
            if (value !== undefined) {
                cloneObj[prop] = value;
            }
        }
    }
    return cloneObj;
}

// options
/*
    {
        left: 0,
        top: 0,
        width: 0,
        onClose: Function,
        commands: Array<{
            name: 'close',
            label: '关闭',
            icon: 'icon-close',
        }>,
    }
*/
const ContextMenu = function(options) {
    if (Vue.prototype.$isServer) return;
    options = clone({root: document.body}, options);
    const id = "ContextMenu_" + seed++;
    const userOnClose = options.onClose;

    options.onClose = function() {
        if (typeof userOnClose === "function") {
            instance ? userOnClose(instance) : userOnClose();
        }
    };

    if (instance) {
        instance.close();
        instance = null;
    }

    instance = new ContextMenuConstructor({
        data: options
    });
    instance.id = id;
    instance.$mount();
    options.root.appendChild(instance.$el);
    instance.visible = true;
    instance.dom = instance.$el;
    instance.dom.style.zIndex = 99999;

    return instance;
};

ContextMenu.close = function(userOnClose) {
    instance && instance.close();
};

export default ContextMenu;
