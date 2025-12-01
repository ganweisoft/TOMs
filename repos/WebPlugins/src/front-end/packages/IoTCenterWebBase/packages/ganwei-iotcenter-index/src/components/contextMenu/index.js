
// import Vue from "vue";
import { createApp } from "vue";

import Main from "./index.vue";

let instance;

const createMount = options => {
    const mountNode = document.createElement("div");
    mountNode.id = "mountNode"
    mountNode.style.position = "relative"
    mountNode.style.zIndex = 9
    document.body.appendChild(mountNode);
    const app = createApp(Main, {
        ...options,
        remove () {
            app.unmount(mountNode);
            document.body.removeChild(document.getElementById("mountNode"));
        }
    });
    return app.mount(document.getElementById("mountNode"));
};

const ContextMenu = options => {
    instance = createMount(options);
    instance.visible = true;
    instance.commands = options.commands;
    instance.left = options.left;
    instance.top = options.top;
    instance.root = options.root;
    instance.width = options.width;
    return instance;
};

ContextMenu.close = () => {
    if (instance) {
        instance.closed = true;
    }
}

export default ContextMenu;
