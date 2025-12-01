/* eslint-disable */
import Vue from 'vue'
import Element from 'element-ui'

import Axios from 'axios'
import App from './App'

import router from './router.js'
import api from './src/request/api'
import moment from 'dayjs';

import loading from 'gw-base-components-plus/loading'
import { PartialNotify } from 'gw-base-utils-plus/notification'
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

Vue.config.productionTip = false
Vue.prototype.Axios = Axios

Vue.prototype.$moment = moment


/* 将api 挂载到 vue 的原型上 */
Vue.prototype.$api = api;

async function initApp() {
    /* 创建vue实例--根据自己模块内部加载，以下为示例 start*/
    window.vm = new Vue({
        router,
        i18n,
        render: h => h(App)
    }).$mount('#app')
    Vue.prototype.$message = PartialNotify(window.vm)
    /* 创建vue实例--根据自己模块内部加载，以下为示例 end*/
}
initApp()
