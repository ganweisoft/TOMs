
/* eslint-disable */
import { createApp } from "vue";
import Element from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import Axios from "axios";
import dayjs from 'dayjs';
import * as api from "./request/api";
import App from "./App.vue";
import router from "./router";
import { PartialNotify, installMessage } from "@components/@ganwei-pc/gw-base-utils-plus/notification";
import hostMap from "./hostMap";
import loading from '@components/@ganwei-pc/gw-base-components-plus/loading/index.vue'
// import i18n from "@components/@ganwei-pc/gw-base-utils-plus/i18n.js";
import { createI18n } from 'vue-i18n';

import en from './language/en-us/en'
import zh from './language/zh-cn/zh'
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
    messages: { 'zh-CN': zh, 'en-US': en }
});

import { adapterInstall } from '@components/@ganwei/element-plus-adapter/res/ElementPlusAdapter.js'
import '@components/@ganwei/element-plus-adapter/res/ElementPlusAdapter.css'
import { configInfoData, getCurrentDate, generateUUID } from '@components/@ganwei-pc/gw-base-utils-plus/commonutils';

const app = createApp(App);

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component)
}
app.component('loading', loading)
app.use(Element).use(adapterInstall)
app.use(i18n)
app.config.globalProperties.Axios = Axios;
app.config.globalProperties.$moment = dayjs;
app.config.globalProperties.$api = api;

app.config.globalProperties.$message = PartialNotify(app, 'ganwei-iotcenter-eq-linkage')
app.config.globalProperties.$hostMap = hostMap
app.config.globalProperties.myUtils = {
    configInfoData,
    getCurrentDate,
    generateUUID
};

app.use(router).use(installMessage).mount("#app"); 
