export default {
    data () {
        return {
            packageId: '',
            pluginName: '',
            menuName: '',
            key: null,
        }
    },
    async created () {

        this.getPublic()

        setTimeout(async () => {
            const { packageId, pluginName, menuName } = this.$route.query
            if (packageId) {
                let moduleData = await this.i18n.getLanguage(packageId, pluginName, menuName, this)
                if (moduleData) {
                    try {
                        this.$delete(this.$i18n._vm.messages[sessionStorage.languageType], menuName)
                    } catch (error) {

                    }
                    this.$set(this.$i18n._vm.messages[sessionStorage.languageType], menuName, moduleData)
                    this.$nextTick(() => {
                        this.$forceUpdate()
                    })
                }

            }
        }, 30);



        let config = { attributes: true, childList: false, subtree: false, attributeFilter: ['languagetype'] };

        /*
        * 监听语言类型变化时，进行语言切换
        * 
        */
        let observer = new MutationObserver(mutations => {
            let languageType = document.getElementsByTagName('html')[0].attributes['languagetype'].value;
            this.$i18n.locale = languageType
            if (window.top.i18n && window.top.i18n.messages && window.top.i18n.messages[languageType]['login']) {
                this.$set(this.$i18n._vm.messages[sessionStorage.languageType], 'login', window.top.i18n.messages[languageType]['login'])
                this.$set(this.$i18n._vm.messages[sessionStorage.languageType], 'publics', window.top.i18n.messages[languageType]['login'].publics || {})
                this.$set(this.$i18n._vm.messages[sessionStorage.languageType], 'menuJson', window.top.i18n.messages[languageType]['login'].menuJson || {})
            }
        })
        observer.observe(window.document.getElementsByTagName('html')[0], config) // 监听的 元素 和 配置项

        this.key = this.generateUUID()

    },
    methods: {
        async getPublic () {
            this.key = this.generateUUID()
            let loginData = await this.i18n.getLanguage('Ganweisoft.IoTCenter.Module.Login', 'ganwei-iotcenter-login', 'login', this)
            if (loginData) {
                try {
                    this.$delete(this.$i18n._vm.messages[sessionStorage.languageType], 'login')
                } catch (error) {

                }

                this.$set(this.$i18n._vm.messages[sessionStorage.languageType], 'login', loginData)
            }
        },
        generateUUID () {
            let d = new Date().getTime();
            if (window.performance && typeof window.performance.now === 'function') {
                d += performance.now(); // use high-precision timer if available
            }
            let uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                let r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
            });
            return uuid;
        },

    }


}