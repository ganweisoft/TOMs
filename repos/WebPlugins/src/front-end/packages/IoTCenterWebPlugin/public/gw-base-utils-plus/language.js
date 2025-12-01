export default {
    data () {
        return {
            pluginName: '',
            menuName: '',
            packageId: '',
            menuPackageId: {},
            manuTipsName: ''
        }
    },
    created () {
        // 获取当前模块插件名（pluginName）以及页面名（menuName）
        let arr = this.$route.path.split('/jumpIframe/').pop().split('/');
        this.pluginName = arr[0]; this.menuName = arr.pop();
        // 获取左侧菜单栏列表，循环遍历当前模块packageId
        let asideList = JSON.parse(window.sessionStorage.asideList);
        this.getPackgeId(asideList, this.menuName)
        window.i18n = this.$i18n
    },

    mounted () {
        let languageType = window.sessionStorage.languageType
        this.setAttribute(languageType)
    },
    methods: {
        // 获取模块packageId，moreTab:是否是在index.vue中，且多tab情况下，进行语言切换
        getPackgeId (arr, menuName, moreTab) {
            for (let index = 0; index < arr.length; index++) {
                if (arr[index].route && arr[index].route.includes(menuName)) {
                    if (moreTab) {
                        this.menuPackageId[menuName] = arr[index].packageId
                    } else {
                        this.packageId = arr[index].packageId;
                    }
                    this.manuTipsName = arr[index].name
                    break;
                } else {
                    if (arr[index].children) {
                        this.getPackgeId(arr[index].children, menuName, moreTab)
                    }
                }
            }
        },
        getLanguage (languageType, pluginName, menuName, packageId) {
            if (!this.packageId) {
                let asideList = JSON.parse(window.sessionStorage.asideList, this.menuName);
                this.getPackgeId(asideList)
            }
            return new Promise((resolve, reject) => {
                let data = {
                    pluginName: pluginName || this.pluginName,
                    menuName: menuName || this.menuName,
                    packageId: (packageId || this.packageId)
                }

                this.$api.getjsontranslationfile(data).then(res => {
                    if (res.data.code == 200) {
                        resolve(res.data.data)
                    } else {
                        this.$message.error(res.data.message, res, this.manuTipsName)
                        reject(false)
                    }
                }).catch(err => {
                    reject(false)
                    this.groupLoad = false
                    this.$message.error(err.data, err, this.manuTipsName)
                })
            })
        },

        // 获取模块语言包
        async asyncGetLanguage (languageType, pluginName, menuName, packageId) {

            this.groupLoad = true
            let language
            try {
                language = await this.getLanguage(languageType, pluginName, menuName.toLowerCase(), packageId);
            } catch (error) {
                language = false
            }
            if (language) {  // 当获取得到值
                try {
                    window.i18n._vm.messages[languageType][menuName] = JSON.parse(language)
                } catch (error) {
                    console.log(error);
                }
            }

            this.groupLoad = false
        },

        // 初始化语言包、当不存在时请求语言包
        async initLanguage (languageType) {
            let lang = languageType || window.sessionStorage.languageType;
            if (!window.i18n.messages || (!window.i18n.messages[lang][this.menuName])) {
                try {
                    await this.asyncGetLanguage(lang, this.pluginName, this.menuName, this.packageId);
                } catch (error) {
                    console.log(error);
                }

                this.setAttribute(lang)
            }
        },

        // 设置语言
        setAttribute (languageType) {
            this.$i18n.locale = languageType
            let iframe = document.getElementsByClassName("jumpIframe");
            for (let item of iframe) {
                item.contentWindow.document.documentElement.setAttribute("data-languagetype", languageType);
                item.contentWindow.postMessage({ languageChange: true })
            }
        }
    },

}
