
import { computed, defineAsyncComponent } from 'vue'
const indexLeftContent = defineAsyncComponent(() => { return import('@/components/indexLeftContent/index.vue') })
const indexRightContent = defineAsyncComponent(() => { return import('@/components/indexRightContent/index.vue') })
import gwEquipCache from '@components/@ganwei-pc/gw-base-components-plus/equipProcessing/gwEquipCache'
import exportHistory from '@components/@ganwei-pc/gw-base-utils-plus/historyExport'

import sceneMonitor from '@/components/sceneMonitor/index.js'
const menuColorList = ['#6763EF', '#2FA9E6', '#B945CE', '#27AE79', '#F68309', '#9872EF', '#0FD3D4', '#F37071', '#FDB03D']
import getMenuesList from "./menu"
export default {
    mixins: [exportHistory, sceneMonitor],
    components: {
        indexLeftContent,
        indexRightContent,
    },
    data () {
        return {
            config: {
                showSwitchTheme: false,
                img: {
                    indexSmallImg: ''
                },
                theme: {
                    default: '',
                    supportThemes: []
                }
            },
            theme: {
                indexSmallImg: ''
            },
            allMenus: [],
            loading: false,
            navTopActive: -1,
            outerLinkMap: {},
            routesMap: {},
            routesToName: {},
            routesToIcon: {},
            routesToPackageId: {},
            onlyContent: false,
            isHideMenu: false,
            isHideHead: false
        }
    },

    provide () {
        return {
            theme: computed(() => this.theme),
            config: computed(() => this.config),
            routesMap: computed(() => this.routesMap),
            routesToPackageId: computed(() => this.routesToPackageId),
            routesToName: computed(() => this.routesToName),
            routesToIcon: computed(() => this.routesToIcon),
            outerLinkMap: computed(() => this.outerLinkMap)
        }
    },

    beforeCreate () {
        document.title = sessionStorage.platName || ''
    },

    mounted () {
        // 获取配置以及菜单
        this.getWebConfigAndMenu()
        // 设备数据
        // eslint-disable-next-line new-cap
        let equipDataSource = new gwEquipCache()
        equipDataSource.Init()
        // 设备列表--历史曲线导出、菜单新增等事件通信
        window.addEventListener('message', e => {
            this.curveSignalR(e, true)
            if (e.data && typeof e.data == 'object' && 'setMenu' in e.data) {
                // 传递被删除的菜单项的route到Index组件中
                this.resetMenu({ route: e.data.route, setMenu: e.data.setMenu })
            }
            if(e?.data?.parentRouter) {
                this.$router.push(e.data.parentRouter)
            }

            if(e.data && typeof e.data == 'object' && 'pub' in e.data){
                const allSpans = document.querySelectorAll('.jumpPage iframe');
                allSpans.forEach(iframe => {
                    iframe.contentWindow.postMessage({ pub: e.data.pub})
                });
            }
        })
        // 主题切换事件监听
        this.$bus.on('themeChange', () => {
            this.theme = JSON.parse(JSON.stringify(this.config.theme.supportThemes.find(item => item.value === sessionStorage.theme)))
            if (process.env.NODE_ENV === 'development') {
                const searchParams = new URL(window.location).searchParams
                searchParams.set('theme', this.theme.value)
                window.location.search = '?' + searchParams.toString()
            }
        })

    },

    methods: {
        getWebConfigAndMenu () {
            this.myUtils
                .configInfoData(this)
                .then(data => {
                    // 设置平台LOGO
                    if (data?.img?.platLogo) {
                        this.setPlatLogo(data.img.platLogo)
                    }
                    // // 使用个性主题替换配置默认主题
                    let result = data
                    this.config = result
                    this.theme = JSON.parse(JSON.stringify(this?.config?.theme?.supportThemes.find(item => item.value === sessionStorage.theme)))
                })
                .catch(err => {
                    console.log(err)
                }).finally(() => {
                    //获取菜单
                    this.getMenus()
                    // 获取平台名称
                    this.getAuthName()
                })
        },
        setPlatLogo (platLogo) {
            let link = document.createElement('link')
            link.setAttribute('rel', 'icon')
            link.href = platLogo
            link.setAttribute('type', 'image/x-icon')
            window.document.head.prepend(link)
        },
        getAuthName () {
            this.$api.getAuthName().then(res => {
                window.top.document.title = res?.data?.data || this.config?.titleConfig?.platName || ''
            }).catch(err => {
                console.log(err)
                window.top.document.title = this.config?.titleConfig?.platName || ''
            })
        },
        getMenus () {
            let data = getMenuesList
            if (data) {
                data = this.filterPCMenu(data || [])
                this.setMenuBackgroundColor(data)
                data.forEach((group, index) => {
                    this.traverse(group, index)
                })
                this.allMenus = data
                this.navTopActive = 0;
                window.sessionStorage.asideList = JSON.stringify(this.allMenus)
            }
        },

        filterPCMenu (menus) {
            console.log(menus)
            let newarr = [];
            menus.forEach(element => {
                let menuItem = {}
                if (element.menuOwner == 0 || element.menuOwner == 1) { // 判断条件
                    menuItem = {
                        ...element,
                        children: element.children ? [] : null
                    }
                    newarr.push(menuItem)
                }
                if (element.children && element.children.length > 0) {
                    let redata = this.filterPCMenu(element.children);
                    menuItem.children = redata.length ? redata : null
                }
            });
            return newarr;
        },
        setMenuBackgroundColor (list) {
            let colorIndex = 0
            let length = menuColorList.length
            list.forEach(menuItem => {
                if (colorIndex + 1 > length) {
                    colorIndex = 0
                }
                menuItem.backgroundColor = menuColorList[colorIndex]
                if (menuItem.children) {
                    this.setMenuBackgroundColor(menuItem.children)
                }
                colorIndex++
            })
        },
        traverse (data, index) {
            if (data && !data.route && data.children) {
                for (let item of data.children) {
                    this.traverse(item, index)
                }
            } else {
                if (/^http/.test(data.route)) {
                    if (!data.route.includes('target=_blank')) {
                        let route = `/Index/jumpIframe/custom/${data.name}?outerLink=true`
                        let parameter; // 外部链接有参数，则拼接到outerLink=true后面
                        if (data.route && data.route.includes('?')) {
                            parameter = data.route.split('?')[1];
                        }
                        let keyVal = route + (parameter ? '&' + parameter : '');
                        this.outerLinkMap[keyVal] = data.route;
                        data.route = keyVal; //作为URL传输，通过this.$route.fullPath获取
                    }
                }
                if (data.route) {

                    this.routesMap[data.route] = index
                    this.routesToPackageId[data.route] = data.packageId
                    this.routesToPackageId[data.route.split('?').pop()] = data.packageId
                    this.routesToName[data.route] = data.name
                    this.routesToIcon[data.route + 'icon'] = data.icon
                }
            }
        },

        resetMenu (data) {
            // 判断是否重新获取菜单
            if (data.setMenu) {
                this.getMenus()
            }
        }
    },
    watch: {
        '$route': {
            handler: function (val, oldVal) {
                this.onlyContent = window.getParameterMap().get("content") === 'true'
                this.isHideMenu = window.getParameterMap().get("isHideMenu") === 'true'
                this.isHideHead = window.getParameterMap().get("isHideHead") === 'true'
            },
            deep: true,
            immediate: true
        }
    },
}
