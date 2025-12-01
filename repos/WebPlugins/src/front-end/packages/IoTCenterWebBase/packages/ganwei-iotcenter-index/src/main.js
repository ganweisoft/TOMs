
/* eslint-disable */
import { createApp } from "vue";
import Element from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import store from "./store";
import Axios from "axios";
import App from "./App";
import router from "./router";
import api from "./request/api";
import getCode from "@components/@ganwei-pc/gw-base-api-plus/encrypt.js";
import VirtualList from 'vue-virtual-list-v3';
import mitt from 'mitt'
import notification from "@components/@ganwei-pc/gw-base-utils-plus/notification";
import hostMap from "./hostMap";
import loading from '@components/@ganwei-pc/gw-base-components-plus/loading/index.vue'
import { configInfoData, changeStyle } from '@components/@ganwei-pc/gw-base-utils-plus/commonutils';
// 中英文翻译
import en from './language/en-us/en'
import zh from './language/zh-cn/zh'
import { createI18n } from "vue-i18n";
const message = {
    'en-US': en,
    'zh-CN': zh
}
const i18n = createI18n({
    // 使用composition API
    legacy: false,
    // 全局使用t函数
    globalInjection: true,
    // 关闭控制台警告
    silentFallbackWarn: true,
    silentTranslationWarn: true,
    locale: localStorage.languageType || 'zh-CN', // 优先从本地存储获取语言设置，如果没有则使用浏览器默认语言
    // fallbackLocale: 'en-US', // 无法匹配时，备份
    messages: message
})
window.axios = Axios;
const app = createApp(App);

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component)
}
window.hasIframe = true
app.component('loading', loading)
app.config.productionTip = false;
app.config.globalProperties.$bus = mitt()
app.config.globalProperties.Axios = Axios;
app.config.globalProperties.$store = store
app.config.globalProperties.i18n = i18n
app.config.globalProperties.$message = notification
app.config.globalProperties.$hostMap = hostMap
app.config.globalProperties.$api = api
app.config.globalProperties.$getCode = getCode
app.config.globalProperties.myUtils = {
    configInfoData,
    changeStyle
};

app.provide('$message', app.config.globalProperties.$message)

app.use(VirtualList).use(i18n).use(router).use(Element).mount("#app");

