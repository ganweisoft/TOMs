/* eslint-disable */
import Vue from "vue";
import Element from "element-ui";
import Axios from "axios";
import App from "./App";
import router from "./router";
import { configInfoData,getCurrentDate } from 'gw-base-utils-plus/commonutils'
import api from "gw-base-api-plus/api";
import getCode from "gw-base-api-plus/encrypt.js";
import "gw-base-style-plus/style.scss";
import loadMore from "gw-base-components-plus/loadMore/loadMore";

import loading from "gw-base-components-plus/loading";
import { PartialNotify } from "gw-base-utils-plus/notification";

import i18n from "gw-base-utils-plus/i18n.js";

Vue.use(loadMore);
Vue.use(Element);
Vue.use(loading);
const bus = new Vue();

Vue.config.productionTip = false;
Vue.prototype.Axios = Axios;

/* 将 getCode 挂载到 vue 的原型上 */
Vue.prototype.$getCode = getCode;

/* 将 myUtils 挂载到 vue 的原型上 */
Vue.prototype.myUtils = {
    configInfoData,getCurrentDate
};


/* 将api 挂载到 vue 的原型上 */
Vue.prototype.$api = api;

window.vm = new Vue({
    router,
    i18n,
    render: h => h(App),
    data: {
        bus
    }
}).$mount("#app");

const notify = PartialNotify(window.vm, "login.interface.login");
Vue.prototype.$notify = notify;
Vue.prototype.$message = notify;
