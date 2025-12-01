/**
 * 语言包获取及语言切换
 */

const language = {
    //获取当前可切换的语言列表
    getsupportedcultures () {
        return this.get('/api/localization/getsupportedcultures');
    },
    //获取语言包
    getjsontranslationfile (data) {
        return this.get('/api/localization/getjsontranslationfile', data);
    },
    //切换语言
    switchingculture (cultureName) {
        return this.post(`/api/localization/switchingculture?culture=${cultureName}`)
    }
}

export default language;