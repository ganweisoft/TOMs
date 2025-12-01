import Axios from 'axios';
export default class language {
    getPluginId () {
        let arr = window.top.location.href.split('jump/')
        if (arr.length > 1) {
            return arr.pop().split('/')[0]
        }
        return ''
    }
    static getLanguageByPage (languageType, page, pluginId) {
        let baseUrl = import.meta.env.NODE_ENV === 'production' ? "/APP" : ""
        if (!pluginId) {
            pluginId = this.getPluginId()
        }
        let url = pluginId ? `${baseUrl}/${pluginId}` : baseUrl;

        return new Promise((resolve, reject) => {
            Axios.get(`${url}/static/language/${languageType}/${page}.json`).then(res => {
                if (res.status == 200) {
                    resolve(res.data)
                } else {
                    reject(res)
                }
            }).catch(err => {
                reject(err)
            })
        })
    }
    static async getLanguage (languageType, pageItem, vm) {
        if (vm) {
            if (!vm.$i18n.messages[languageType]) {
                vm.$i18n.messages[languageType] = {}
            }
            if (vm.$i18n.messages[languageType][pageItem]) {
                return;
            }
            await this.getLanguageByPage(languageType, pageItem).then(res => {
                vm.$i18n.messages[languageType][pageItem] = res
            }).catch(err => {
                console.log(err)
            })
        }

    }
}