export default {
    data() {
        return {
            menuNames: [], //多tab切换
            menuPackageId: {},
            pluginName: '',
            menuName: '',
            packageId: ''
        }
    },
    async created() {
        // 获取登录页存储的左侧菜单栏
        if (window.menuJson) {
            this.$i18n._vm.messages[sessionStorage.languageType]['menuJson'] = window.menuJson
        }
        this.$i18n.locale = sessionStorage.languageType;
        // 获取外框翻译包
        await this.getLoginJson();

    },
    mounted() {
        // 设置监听是否切换语言类型----start
        let config = { attributes: true, attributeFilter: ['data-languagetype'] };
        let observer = new MutationObserver(async (mutations) => {
            let asideList = JSON.parse(window.sessionStorage.asideList);
            let languageType = document.getElementsByTagName('html')[0].attributes['data-languagetype'].value;
            if (window.sessionStorage.editableTabs != 'null') {   //当存在tab页时
                this.$i18n.locale = languageType
                window.sessionStorage.setItem("languageType", languageType);
                this.menuNames = []
                this.menuPackageId = {}
                await this.getLoginJson()
                this.getMenuNames()
                this.menuNames.forEach(item => {
                    this.getPackgeId(asideList, item, true)
                })

                for (const item in this.menuPackageId) {
                    let menuName = item.split('/')[1]
                    let pluginName = item.split('/')[0]
                    await this.asyncGetLanguage(languageType, pluginName, menuName, this.menuPackageId[item])
                }

            } else {
                await this.getLoginJson()        //当非tab页时
                this.getPluginNameAndMenuName()
                this.$i18n.locale = languageType
                window.sessionStorage.setItem("languageType", languageType);
                this.getPackgeId(asideList, this.menuName)
                if (!window.i18n.messages || (!window.i18n.messages[languageType][this.menuName])) {
                    await this.asyncGetLanguage(languageType, this.pluginName, this.menuName, this.packageId)
                }
            }
            this.setAttribute(languageType)
        })

        observer.observe(window.document.documentElement, config) // 监听的 元素 和 配置项
        // 设置监听是否切换语言类型----end
    },
    methods: {

        // 获取插件名以及页面名
        getPluginNameAndMenuName() {
            let arr = this.$route.path.split('/jumpIframe/').pop().split('/');
            this.pluginName = arr[0]; this.menuName = arr.pop();
        },

        // 获取登录翻译包
        async getLoginJson() {
            let language = {}
            try {
                language = await this.getLanguage(sessionStorage.languageType, 'ganwei-iotcenter-login', 'login', 'Ganweisoft.IoTCenter.Module.Login');
            } catch (error) {
                console.log(error);
                language = false
            }
            if (language) {  // 当获取得到值
                try {
                    this.$i18n._vm.messages[sessionStorage.languageType]['login'] = JSON.parse(language)
                    this.$i18n._vm.messages[sessionStorage.languageType].menuJson = JSON.parse(language).menuJson
                    this.$i18n._vm.messages[sessionStorage.languageType].publics = JSON.parse(language).publics
                    this.$i18n._vm.messages[sessionStorage.languageType]['noAccess'] = JSON.parse(language).noAccess
                } catch (error) {
                    console.log(error);
                }
            }
        },
        // 获取模块packageId，moreTab:是否是在index.vue中，且多tab情况下，进行语言切换
        getPackgeId(arr, menuName, moreTab) {
            for (let index = 0; index < arr.length; index++) {
                if (arr[index].route && arr[index].route.includes(menuName)) {
                    if (moreTab) {
                        this.menuPackageId[menuName] = arr[index].packageId
                    } else {
                        this.packageId = arr[index].packageId;
                    }
                    break;
                } else {
                    if (arr[index].children) {
                        this.getPackgeId(arr[index].children, menuName, moreTab)
                    }
                }
            }
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
            if (language) {  // 当获取得到值
                try {
                    this.$i18n._vm.messages[languageType][menuName] = JSON.parse(language)
                } catch (error) {
                    console.log(error);
                }
            }

            this.groupLoad = false
        },
        // 获取页面名
        getMenuNames() {
            let tabs = JSON.parse(window.sessionStorage.editableTabs);
            tabs.forEach(item => {
                this.menuNames.push(item.route.split('/jumpIframe/').pop())
            })
        },

        getLanguage(languageType, pluginName, menuName, packageId) {
            return new Promise((resolve, reject) => {
                let data = {
                    pluginName: pluginName || this.pluginName,
                    menuName: menuName || this.menuName,
                    packageId: packageId || this.packageId
                }

                this.$api.getjsontranslationfile(data).then(res => {
                    if (res.data.code == 200) {
                        resolve(res.data.data)
                    } else {
                        this.$message.error(res.data.message, res)
                        reject(false)
                    }
                }).catch(err => {
                    reject(false)
                    this.groupLoad = false
                    this.$message.error(err.data, err)
                })
            })
        },

        // 设置语言
        setAttribute(languageType) {
            this.$i18n.locale = languageType
            window.i18n = this.$i18n
            let iframe = document.getElementsByClassName("jumpIframe");
            for (let item of iframe) {
                item.contentWindow.document.documentElement.setAttribute("data-languagetype", languageType);
                item.contentWindow.postMessage({ languageChange: true })
            }
        }
    },
}