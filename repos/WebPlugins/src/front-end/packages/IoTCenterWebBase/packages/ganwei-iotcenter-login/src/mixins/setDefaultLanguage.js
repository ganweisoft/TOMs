
export default {
    data () {
        return {
            isShow: false,
            defaultConfigLanguage: '',
            languageActive: ''
        }
    },
    async created () {
        // 先请求配置、语言列表，再根据languageType 请求语言
        this.defaultConfig()
    },
    methods: {
        defaultConfig () {
            window.document.title = sessionStorage.platName || ''
            window?.getConfigInfoData().then(webConfig => {
                if (webConfig.img.platLogo) {
                    let link = document.createElement('link')
                    link.href = webConfig.img.platLogo
                    link.setAttribute('rel', 'icon')
                    link.setAttribute('type', 'image/x-icon')
                    window.document.head.prepend(link)
                }
                this.getAuthName(webConfig.titleConfig.platName)
                sessionStorage.defaultConfigLanguage = webConfig.showLangSelect  // 是否显示中英文选择框
                this.defaultConfigLanguage = webConfig.defaultLanguageType
                this.getsupportedcultures()
                sessionStorage.mainImg = webConfig.img.loginImg
                window.document.documentElement.setAttribute('data-theme', localStorage.theme || webConfig?.theme?.default)
                localStorage.setItem('theme', localStorage.theme || webConfig?.theme?.default)
            }).catch((err) => {
                console.log(err)
            })
        },
        getAuthName (platName) {
                window.document.title = platName;
                sessionStorage.setItem('platName', platName)
        },
        getsupportedcultures () {

            let that = this, langString = localStorage.languageType
            window.getLanguageOptions().then((data) => {
                let dat = data || []
                window.localStorage.langOptions = JSON.stringify(dat)

                if (data.length > 0) {
                    // 存在操作记录 且 在操作项在列表中
                    if (langString && data.some((item) => { return item.value.toLowerCase().includes(langString.toLowerCase()) })) {
                        that.languageActive = localStorage.languageType
                    } else { // 没有操作记录
                        langString = that.convertLanguage(navigator.language).toLowerCase()
                        if (data.some((item) => { return item.value.toLowerCase().includes(langString.toLowerCase()) })) {  // 浏览器默认值存在于列表中
                            that.languageActive = that.convertLanguage(navigator.language)
                        } else { // 使用web配置默认值
                            that.languageActive = that.defaultConfigLanguage || "zh-CN"
                        }
                    }
                    // 请求语言使用
                    localStorage.languageType = sessionStorage.languageType = that.languageActive
                }
            }).catch(err => {
                 console.log(err)
                 that.languageActive = that.defaultConfigLanguage || "zh-CN"
            }).finally(err=>{
                // 每次设置完首页中英文选项后，自动进入中英文获取，内部根据是否有缓存进行请求
                // this.getPublic()
            })
        },
        convertLanguage (str) {
            let result;
            switch (str) {
                case "en": result = "en-US"; break;
                case "zh": result = "zh-CN"; break;
                default:
                    result = str; break;
            }
            return result;
        },

    }
}
