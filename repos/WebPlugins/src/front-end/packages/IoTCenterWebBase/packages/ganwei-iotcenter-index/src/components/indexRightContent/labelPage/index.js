import ContextMenu from '@/components/contextMenu'
const firstTab = '1'
export default {
    props: {
        allMenus: {
            type: Object,
            default: () => []
        },
    },
    inject: ['routesToName', 'outerLinkMap', 'routesToIcon'],

    data () {
        return {
            editableTabs: [],
            refreshName: '',
            editableTabsValue: '',
            isActive: false
        }
    },
    mounted () {
        window.addEventListener('message', e => {
            if (e.data && typeof e.data == 'object' && 'setMenu' in e.data) {
                // 传递被删除的菜单项的route到Index组件中
                this.resetMenu({ route: e.data.route, setMenu: e.data.setMenu })
            }
        })
        window.addEventListener('keyup', e => {
            this.keyEvent(e)
        })
        this.$bus.on('openPage', data => {
            const { changePath, callback } = data
            this.$router.push(changePath)
            if (callback) {
                this.$nextTick(() => {
                    callback()
                })
            }
        })
        // 外挂链接
        window.openModulePage = this.openModulePage
        window.refreshModulePage = this.refreshModulePage
        window.closeModulePage = this.closeModulePage
    },
    watch: {
        editableTabsValue (val) {
            localStorage.editableTabsValue = val
        },
        allMenus: {
            handler (val) {
                if (!val.length) {
                    return;
                }
                this.init()
            },
            immediate: true
        },
        $route () {
            try { ContextMenu.close() } catch (e) {
                console.log(e)
            }

            this.init()
        }
    },
    methods: {
        init(){
            let { customize, title, icon, url } = this.$route.query
            if(customize) {
                this.openModulePageHandle(title, icon, url)
            } else{
                this.updateTabs()
            }
        },
        updateTabs () {
            this.$nextTick(() => {
                let fullPath = decodeURIComponent(this.$route.fullPath)
                let path = decodeURIComponent(this.$route.path)
                let icon = this.routesToIcon[fullPath + 'icon'] || this.routesToIcon[path + 'icon']
                if (this.$route.fullPath != '/Index') {
                    this.$nextTick(() => {
                        if (this.$route?.query?.currentPageName) {
                            this.setTableTabs(this.$route.query.currentPageName, icon)
                            return
                        }
                        let name = this.routesToName[fullPath] || this.routesToName[path]
                        this.setTableTabs(name || this.$t('login.noAccess.noPage'), icon)
                    })
                }
            })
        },
        resetMenu (data) {
            // 判断是否删除标签页
            if (data.route) {
                // 找到标签页中被删除的菜单
                this.editableTabs.forEach(item => {
                    if (data.route == item.route) {
                        this.removeTab(item.name)
                    }
                })
            }
        },

        tabClick (event) {
            this.refreshName = event.props.label
            let result = (this.editableTabs instanceof Object) ? JSON.parse(JSON.stringify(this.editableTabs)) : []
            result = result.filter(item => {
                return item.title == event.props.label
            })
            let route = result[0].route
            this.menuActive = route
            this.$router.push(route)
        },
        openContextMenu (e) {
            function getTabName (element) {
                if (!e.currentTarget.contains(element)) {
                    return ''
                }
                if (element.classList.contains('el-tabs__item')) {
                    return element.id.split('-')[1]
                }
                return getTabName(element.parentElement)
            }
            let targetName = getTabName(e.target)
            if (targetName === '') return
            let startIndex = this.editableTabs.findIndex(i => i.name == targetName)
            ContextMenu({
                width: '150',
                root: e.currentTarget,
                left: e.clientX,
                top: e.clientY,
                commands: [
                    {
                        name: 'closeCurrent',
                        label: this.$t('login.contextMenu.close'),
                        icon: 'icon-tubiao24_guanbi',
                        onclick: () => {
                            this.removeTab(targetName)
                        },
                        disabled: this.editableTabs.length <= 1
                    },
                    {
                        name: 'closeOther',
                        label: this.$t('login.contextMenu.closeOther'),
                        onclick: async () => {
                            let toBeCloses = this.editableTabs.filter(i => i.name != targetName).map(i => i.name)
                            for (let item of toBeCloses) {
                                await this.removeTab(item)
                            }
                        },
                        disabled: this.editableTabs.length <= 1
                    },
                    {
                        name: 'closeLeft',
                        label: this.$t('login.contextMenu.closeLeft'),
                        onclick: async () => {
                            let toBeCloses = this.editableTabs.slice(0, startIndex).map(i => i.name)
                            for (let item of toBeCloses) {
                                await this.removeTab(item)
                            }
                        },
                        disabled: startIndex == 0
                    },
                    {
                        name: 'closeRight',
                        label: this.$t('login.contextMenu.closeRight'),
                        onclick: async () => {
                            let toBeCloses = this.editableTabs.slice(startIndex + 1).map(i => i.name)
                            for (let item of toBeCloses) {
                                await this.removeTab(item)
                            }
                        },
                        disabled: startIndex == this.editableTabs.length - 1
                    },
                    {
                        name: 'reload',
                        label: this.$t('login.contextMenu.reload'),
                        icon: 'icon-tubiao24_qiehuan',
                        onclick: () => {
                            this.refreshTab(startIndex)
                        },
                        disabled: false
                    }
                ]
            })
        },
        // 关闭标签页
        removeTab (targetName) {

            if (this.editableTabs.length == 1) return;
            let tabs = JSON.parse(JSON.stringify(this.editableTabs))
            let activeNameIndex = this.editableTabsValue
            this.editableTabs.forEach(item => {
                if (item.name == targetName) {
                    item.disabled = true
                }
            })
            return new Promise((resolve => {
                tabs.forEach((tab, index) => {
                    if (tab.name === targetName) {
                        if (activeNameIndex === targetName) {
                            let nextTab = tabs[(index != 0 ? index - 1 : index + 1)]
                            if (nextTab) {
                                activeNameIndex = nextTab.name
                                localStorage.menuActiveName = nextTab.title
                                this.menuActive = nextTab.route
                                this.$router.push(nextTab.route)
                                this.refreshName = nextTab.title
                            }
                            this.editableTabs[index].disabled = true
                        }
                        let routerPath = this.editableTabs[index].route
                        if (routerPath && this.$refs[routerPath]?.[0]?.executeIframeFun) {
                            this.$refs[routerPath][0].executeIframeFun(() => {
                                let tagIndex = tabs.findIndex(i => i.name === targetName)
                                this.editableTabs.splice(tagIndex, 1)
                                resolve(true)
                            })
                        }
                    }
                })
            }))

        },

        // 刷新标签页
        refresh() {
            try{
                let dom = document.querySelectorAll('.tabs-page-container .el-tabs__item.is-active')
                let index = dom[0].children[1].attributes.res.value
                this.refreshTab(index)
            } catch(e){
                console.log(e)
            }
        },
        refreshTab (index) {
            this?.$store?.commit("setLoadingStatus", true)// 防止多次刷新
            let routerPath = this.editableTabs[index].route
            if (this.$refs[routerPath]?.[0]?.executeIframeFun) {
                this.$refs[routerPath][0].executeIframeFun()
            }
            this.editableTabs[index].route = ''
            this.isActive = true
            setTimeout(() => {
                this.editableTabs[index].route = routerPath
                this.isActive = false
            }, 100)
        },

        setTableTabs (title, icon) {

            let len = this.editableTabs.length
            // 判断标签页数组是否存在当前选中导航项
            // this.editableTabs[index]
            let isHave = false
            let fullPath = this.$route.fullPath
            let noPage = this.$t('login.noAccess.noPage')
            
            this.editableTabs.forEach(item => {
                let isRefreshTab = item.route.includes(this.$route.path) && fullPath.includes('refreshTab=true')
                if (item.route == fullPath || isRefreshTab) {
                    isHave = true
                }
                if (isRefreshTab) {
                    item.route = fullPath
                }
                if (item.title == title && noPage == title) {
                    isHave = true
                }
            })

            // 如果不存在则增加一项进数组，存在则只做标签页选中
            if (!isHave) {
                this.editableTabs.push({
                    title: title,
                    route: this.$route.fullPath,
                    name: len ? String(Number(this.editableTabs[len - 1].name) + 1) : firstTab,
                    disabled: false,
                    icon: icon
                })
                len = this.editableTabs.length
                this.editableTabsValue = this.editableTabs[len - 1].name
                let n = this.editableTabs[0].name

                if (len > 6) {
                    this.removeTab(n)
                    len--
                }
            } else {
                for (let i = 0; i < len; i++) {
                    const { route, name } = this.editableTabs[i]
                    let isRefreshTab = route.includes(this.$route.path) && this.$route.fullPath.includes('refreshTab=true')
                    if (this.$route.fullPath == route || isRefreshTab) {
                        this.editableTabsValue = name
                        if (isRefreshTab) {
                            this.refreshTab(i)
                        }
                        break
                    }
                }
            }
            this.refreshName = localStorage.menuActiveName = title
        },

        keyupJump (num) {
            let length = this.editableTabs.length;
            for (let i = 0; i < length; i++) {
                if (num == -1 && i != 0 && this.editableTabs[i].name == this.editableTabsValue) {
                    this.$router.push(this.editableTabs[i - 1].route);
                } else if (num == 1 && i != length - 1 && this.editableTabs[i].name == this.editableTabsValue) {
                    this.$router.push(this.editableTabs[i + 1].route);
                }
            }
        },

        keyEvent (e) {
            if (e.altKey && e.keyCode == 188) {
                this.keyupJump(-1);
            } else if (e.altKey && e.keyCode == 190) {
                this.keyupJump(1);
            }
        },

        openModulePage(hasPage = false, url, title = "模板管理", icon = "icon-caidan_shebeiliebiao", packageId = "", pluginName = "", menuName = "", customParameters = "") {
            let query = hasPage ? {} : {
                customize: true,
                title: title,
                icon: icon,
                url: url,
                packageId: packageId,
                pluginName: pluginName,
                menuName: menuName,
                customParameters: customParameters
            }
            this.$router.push({ path: url, query: query })
        },
        openModulePageHandle(title, icon, url){
            let len = this.editableTabs.length
            let isHave = false// 判断标签页数组是否存在当前选中导航项
            let fullPath = url
            this.editableTabs.forEach(item => {
                let isRefreshTab = item.route.includes(fullPath) 
                if (item.route == fullPath) {
                    isHave = true
                }
                if (isRefreshTab) {
                    item.route = fullPath
                }
            })

            // 如果不存在则增加一项进数组，存在则只做标签页选中
            if (!isHave) {
                this.editableTabs.push({
                    title: title,
                    route: fullPath,
                    name: len ? String(Number(this.editableTabs[len - 1].name) + 1) : firstTab,
                    disabled: false,
                    icon: icon
                })
 
                len = this.editableTabs.length
                this.editableTabsValue = this.editableTabs[len - 1].name
                let n = this.editableTabs[0].name
                if (len > 6) {
                    this.removeTab(n)
                    len--
                }
            } else {
                for (let i = 0; i < len; i++) {
                    const { route, name } = this.editableTabs[i]
                    let isRefreshTab = route.includes(fullPath) 
                    if (fullPath == route) {
                        this.editableTabsValue = name
                        if (isRefreshTab) {
                            this.refreshTab(i)
                        }
                        break
                    }
                }
            }
            this.refreshName = localStorage.menuActiveName = title
        },
        refreshModulePage() {
            this.refresh()
        },
        closeModulePage() {
            this.removeTab(this.editableTabsValue)
        }
    }
}
