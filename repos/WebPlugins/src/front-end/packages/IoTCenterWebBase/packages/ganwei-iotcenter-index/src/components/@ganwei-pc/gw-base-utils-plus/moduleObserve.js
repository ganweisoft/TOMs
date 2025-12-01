import { useI18n } from 'vue-i18n'
export default {
    data () {
        return {
            isShow: false,
            i18nInstance: null,
            locale: {}
        }
    },
    async created () {
        window.onmessage = (event) => {
            if (event.data.changeTheme) {
                this.setTheme(event.data.theme, event.data.version)
            }
        }
        this.i18nInstance = useI18n()
        if(!this.isNoRequestObserveGetPublic)
        await this.getPublic()
    },
    watch: {
        '$route' () {
            const { packageId, pluginName, menuName, languageType } = this.$route.query
            if (languageType) {
                sessionStorage.languageType = localStorage.languageType = languageType
            }
            if (packageId) {
                window.getLanguage(pluginName, menuName, packageId, this)
            }
        }
    },
    methods: {
        setTheme (theme, version = '') {
            let frameTheme = window.document.querySelector("#themeStyle" + version)
            if (frameTheme) {
                frameTheme.href = `/static/themes/${theme + version}.css`
                localStorage.theme = theme
            }
        },
        async getPublic () {
            await window.getLanguage('ganwei-iotcenter-login', 'login', 'Ganweisoft.IoTCenter.Module.Login', this)
            let languagePackage = JSON.parse(sessionStorage?.languagePackage || '{}')
            let elmentLanguage = languagePackage?.[localStorage.languageType]?.ele
            if (elmentLanguage) {
                this.locale = elmentLanguage
            }
        }
    }
}
