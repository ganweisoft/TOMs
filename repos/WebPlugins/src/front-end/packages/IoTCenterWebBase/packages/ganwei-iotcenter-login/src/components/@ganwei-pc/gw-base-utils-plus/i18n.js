import { createI18n } from 'vue-i18n';
let languagePackage = sessionStorage.languagePackage || '{}'
try {
    if (top.location.href.toLowerCase().indexOf('login') != -1) {
        languagePackage = '{}'
    }
} catch (error) {
    languagePackage = '{}'
}

sessionStorage.languagePackage = languagePackage

let message = {}
try {
    message = JSON.parse(languagePackage)
} catch (error) {
    console.log(error)
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
});
export default i18n;
