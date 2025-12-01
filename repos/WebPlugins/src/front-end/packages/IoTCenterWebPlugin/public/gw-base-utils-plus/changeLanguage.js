export default {
    data() {
        return {
            langOptions: [],
            groupLoad: true,
            languageSelected: sessionStorage.languageType || '中文',
        }
    },
    async created() {
        // 初始化登录翻译包、获取语言列表
        sessionStorage.languageType = sessionStorage.languageType || 'zh-CN'
        await this.initLanguage();
        try {
            this.loginCreated();
        }
        catch (e) {}

    },
    methods: {
        langChange(val) {
            // 语言切换
            this.setLanguage(val, true)
        },
        async initLanguage() {
            // 获取语言列表
            this.asyncGetLanguage(sessionStorage.languageType, 'ganwei-iotcenter-login', 'login', 'Ganweisoft.IoTCenter.Module.Login');
            this.getLanguageLogos();
            this.groupLoad = false
        },

        getLanguage(languageType, pluginName, menuName, packageId) {
            sessionStorage.LanguageRequestReCord = 1;
            return new Promise((resolve, reject) => {
                let data = {
                    pluginName: pluginName,
                    menuName: menuName,
                    packageId: packageId
                }

                this.$api.getjsontranslationfile(data).then(res => {
                    if (res.data.code == 200) {
                        resolve(res.data.data)
                    } else {
                        this.$message.error(res.data.message, res)
                        reject(false)
                    }
                }).catch(err => {
                    this.$message.error(err.data, err)
                    reject(err)
                })
            })
        },

        // 获取模块语言包
        async asyncGetLanguage(languageType, pluginName, menuName, packageId) {
            this.groupLoad = true
            let language
            try {
                language = await this.getLanguage(languageType, pluginName, menuName, packageId);
            } catch (error) {
                language = false
            }
            if (language) { // 当获取得到值
                try {
                    // 存储国际化翻译语言包
                    this.$i18n._vm.messages[languageType][menuName] = JSON.parse(language)
                    // 将menuJson存储在window对象，当登录时初始化首页，左侧菜单栏不会有闪烁问题
                    window.top.menuJson = JSON.parse(language).menuJson
                    // 登录页面用户规约文档
                    this.serviceTerms = JSON.parse(language).serviceTerm;
                    // 设置语言类型
                    if (window.sessionStorage.languageType) {
                        this.setLanguage(window.sessionStorage.languageType)
                    } else {
                        this.setLanguage(this.langOptions[0].value)
                    }
                } catch (error) {
                    console.log(error);
                }
            }
            this.groupLoad = false
        },
        // 获取语言列表
        getLanguageLogos() {
            this.$api.getsupportedcultures().then((res) => {
                if (res.data.code == 200) {
                    const data = res.data.data;
                    if (data) {
                        this.langOptions = data
                        window.localStorage.langOptions = JSON.stringify(this.langOptions)
                    }
                } else {
                    this.$message.error(res.data.message, res)
                }
            }).catch(err => {
                this.$message.error(err.data, err)
            })
        },
        // 设置语言类型
        async setLanguage(lang, change) {
            window.sessionStorage.languageType = lang
            // 如果是切换事件，则重新请求模块语言包
            if (change) {
                await this.asyncGetLanguage(lang, 'ganwei-iotcenter-login', 'login', 'Ganweisoft.IoTCenter.Module.Login')
            }
            this.$i18n.locale = lang

        },
    },
}