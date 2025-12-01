/* eslint-disable */
import Vue from 'vue'
import Element from 'element-ui'

import loading from 'gw-base-components-plus/loading'
import { PartialNotify } from 'gw-base-utils-plus/notification'

import store from '@/store'
import Axios from 'axios'
import App from './App'
import router from './router.js'
import { getCurrentDate, setDate } from 'gw-base-utils-plus/commonutils'
import api from './src/request/api'

import dayjs from 'dayjs';

import "gw-base-style-plus/style.scss";

import VueI18n from 'vue-i18n'

import en from './src/language/en-us/en'
import zh from './src/language/zh-cn/zh'

const i18n = new VueI18n({
    locale: localStorage.languageType || 'zh-CN',
    messages: { 'zh-CN': zh, 'en-US': en },
    // messages,
    silentTranslationWarn: true //去掉控制台i18n warning
})

Vue.use(Element)
Vue.use(loading)

const bus = new Vue()

Vue.config.productionTip = false
Vue.prototype.Axios = Axios
Vue.prototype.$Store = store
Vue.prototype.$moment = dayjs


/* 将 myUtils 挂载到 vue 的原型上 */
Vue.prototype.myUtils = { getCurrentDate, setDate };

/* 将api 挂载到 vue 的原型上 */
Vue.prototype.$api = api;
window.vm = new Vue({
    router,
    store,
    i18n,
    render: h => h(App),
    data: {
        bus
    }
}).$mount('#app')
Vue.prototype.$message = PartialNotify(window.vm)
