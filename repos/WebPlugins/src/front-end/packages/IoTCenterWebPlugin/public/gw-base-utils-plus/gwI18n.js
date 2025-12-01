import VueI18n from 'vue-i18n'
import axios from 'axios'
import Vue from 'vue'
import zhLocale from 'element-ui/lib/locale/lang/zh-CN'
import enLocale from 'element-ui/lib/locale/lang/en'
import ElementLocale from 'element-ui/lib/locale';

ElementLocale.i18n((key, value) => i18n.t(key, value))

let zh = {
    ...zhLocale
}
let en = {
    ...enLocale
}


Vue.use(VueI18n)

const GWI18n = function () {
    this.buildWithMessages = false
    this.packageId = ''
    this.pluginName = ''
    this.menuName = ''
    window.sessionStorage.languageType = window.localStorage.languageType || 'zh-CN'
    this.i18n = new VueI18n({
        locale: window.sessionStorage.languageType,
        messages: { 'zh-CN': zh, 'en-US': en },
        // messages,
        silentTranslationWarn: true //去掉控制台i18n warning
    })
    return this;
}

GWI18n.prototype.buildWithApiUrl = function (url) {
    this.url = url
    return this.i18n;
}

GWI18n.prototype.setLocal = function (languageType) {
    // this.i18n.locale = languageType
    sessionStorage.languageType = languageType
    localStorage.languageType = languageType

}

GWI18n.prototype.updateLanguageType = function () {
    document.getElementsByTagName('html')[0].setAttribute('languagetype', sessionStorage.languageType)
    if (window.top.document.getElementsByTagName('html')) {
        window.top.document.getElementsByTagName('html')[0].setAttribute('languagetype', sessionStorage.languageType)
    }
}

GWI18n.prototype.buildWithMessages = function (messages) {
    this.buildWithMessages = true
    this.i18n._vm.messages = messages
    return this.i18n
}

GWI18n.prototype.backupsGetLanguage = async function (packageId, pluginName, menuName, vm) {
    this.packageId = packageId
    this.pluginName = pluginName
    this.menuName = menuName
    if (window.top.i18n && window.top.i18n.messages[sessionStorage.languageType] && window.top.i18n.messages[sessionStorage.languageType][menuName]) {
        this.i18n._vm.messages[sessionStorage.languageType][menuName] = window.top.i18n.messages[sessionStorage.languageType][menuName]
        if (window.top.i18n.messages[sessionStorage.languageType]['login']) {
            this.i18n._vm.messages[sessionStorage.languageType]['publics'] = window.top.i18n.messages[sessionStorage.languageType]['login'].publics
            this.i18n._vm.messages[sessionStorage.languageType]['menuJson'] = window.top.i18n.messages[sessionStorage.languageType]['login'].menuJson
        } else {
            this.getLanguage('Ganweisoft.IoTCenter.Module.Login', 'ganwei-iotcenter-login', 'login', vm)
        }

        if (vm) {
            vm.$nextTick(() => {
                this.i18n._vm.locale = sessionStorage.languageType
            })
        }

    } else {
        let data = {
            packageId,
            pluginName,
            menuName
        }
        axios.defaults.withCredentials = true; // 让ajax携带cookie
        axios.defaults.headers['Accept-Language'] = window.sessionStorage.languageType || 'zh-CN'
        await axios({
            method: 'get',
            url: this.url,
            params: data,
            headers: {
                'Content-Type': 'application/json;charset=UTF-8'
            }
        }).then(res => {
            if (res.data.code == 200 && res.data.data) {
                try {
                    if (!window.top.i18n) {
                        window.top.i18n = {
                            messages: {}
                        }
                    }
                    if (!window.top.i18n.messages[sessionStorage.languageType]) {
                        window.top.i18n.messages[sessionStorage.languageType] = {}
                    }
                    window.top.i18n.messages[sessionStorage.languageType][menuName] = JSON.parse(res.data.data)
                    this.i18n._vm.messages[sessionStorage.languageType][menuName] = JSON.parse(res.data.data)
                    if (menuName.toLowerCase().includes('login')) {
                        this.i18n._vm.messages[sessionStorage.languageType]['publics'] = JSON.parse(res.data.data).publics || {}
                        this.i18n._vm.messages[sessionStorage.languageType]['menuJson'] = JSON.parse(res.data.data).menuJson || {}
                    }
                    vm.$nextTick(() => {
                        vm.$i18n.locale = sessionStorage.languageType
                    })
                } catch (error) {
                    throw (error)
                }
            }
        }).catch(err => {
            if (vm) {
                vm.$message.error(err.data, err)
            }
        })
    }

    let language = JSON.parse(JSON.stringify(this.i18n._vm.messages[sessionStorage.languageType || 'zh-CN'][menuName] || ''))

    return language
}

GWI18n.prototype.getLanguage = async function (packageId, pluginName, menuName, vm) {
    let language = {}
    try {
        language = await window.top.getLanguage(pluginName, menuName, packageId, vm, this.i18n, this.url)
    } catch (error) {
        language = await this.backupsGetLanguage(packageId, pluginName, menuName, vm)
    }
    this.updateLanguageType()
    return language
}

export default GWI18n