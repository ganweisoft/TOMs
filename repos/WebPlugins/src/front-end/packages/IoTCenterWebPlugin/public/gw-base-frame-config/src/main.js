/* eslint-disable */
import Vue from 'vue'
import Element from 'element-ui'

import store from './store'
import Axios from 'axios'
import App from './App'
import router from './router'
import { configInfoData, changeStyle } from 'gw-base-utils-plus/commonutils'
import api from 'gw-base-api-plus/api'
import getCode from 'gw-base-api-plus/encrypt.js'
import 'gw-base-style-plus/style.scss'

import loadMore from 'gw-base-components-plus/loadMore/loadMore'

import loading from 'gw-base-components-plus/loading'
import { PartialNotify } from 'gw-base-utils-plus/notification'
import VueI18n from 'vue-i18n'

import en from './language/en-us/en'
import zh from './language/zh-cn/zh'

const i18n = new VueI18n({
    locale: localStorage.languageType || 'zh-CN',
    messages: { 'zh-CN': zh, 'en-US': en },
    // messages,
    silentTranslationWarn: true //去掉控制台i18n warning
})

import moment from 'dayjs'

Vue.use(loadMore)
Vue.use(Element)
Vue.use(loading)
const bus = new Vue()

Vue.config.productionTip = false
Vue.prototype.Axios = Axios
Vue.prototype.$Store = store
Vue.prototype.$moment = moment

/* 将 getCode 挂载到 vue 的原型上 */
Vue.prototype.$getCode = getCode

/* 将 myUtils 挂载到 vue 的原型上 */
Vue.prototype.myUtils = { configInfoData, changeStyle }

/* 将api 挂载到 vue 的原型上 */
Vue.prototype.$api = api

window.vm = new Vue({
    router,
    store,
    i18n,
    render: h => h(App),
    data: {
        bus
    }
}).$mount('#app')
const notify = PartialNotify(window.vm, 'login.interface.frame')
Vue.prototype.$notify = notify
Vue.prototype.$message = notify
