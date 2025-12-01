import i18nFunction from 'gw-base-utils-plus/gwI18n.js'
import Element from 'element-ui'

let gwI18n = new i18nFunction()
Vue.prototype.i18n = gwI18n


let i18n = gwI18n.buildWithApiUrl('/api/localization/getjsontranslationfile')
Vue.use(Element, { i18n: (key, value) => i18n.t(key, value) });
export default i18n;