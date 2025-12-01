
import { defineAsyncComponent, getCurrentInstance, inject, nextTick, ref, toRaw, watch } from 'vue'
import { useRouter } from 'vue-router'
const asideMenu = defineAsyncComponent(() => { return import('@/components/asideMenu/menu.vue') })
const contractMenu = defineAsyncComponent(() => { return import('@/components/asideMenu/contractMenu.vue') })
const widthSetting = defineAsyncComponent(() => { return import('@components/@ganwei-pc/gw-base-components-plus/widthSetting/widthSetting.vue') })

export default {
    props: {
        allMenus: {
            type: Array,
            default: () => []
        },
        loading: {
            type: Boolean,
            default: false
        },
        navTopActive: {
            type: Number || String,
            default: -1
        }
    },
    components: {
        asideMenu,
        widthSetting,
        contractMenu
    },

    setup (props) {
        const config = inject('config')
        const theme = inject('theme')
        const menuSearch = ref('')
        const isCollapse = ref(false)
        const transition = ref(false)
        const renderMenu = ref([])
        const menus = ref([])
        const showOrganizationalStructure = ref(config.value.showOrganization)
        const organizationName = ref('')
        let router = useRouter()
        const menuActive = ref(router.currentRoute.value.fullPath)
        const { appContext: { config: { globalProperties } } } = getCurrentInstance()
        globalProperties.$bus.on('organizationName', (value) => {
            organizationName.value = value
        })
        globalProperties.$bus.on('jumpUrl', (route) => {
            jump(route)
        })
        const initMenu = () => {
            menuSearch.value = ''
            if (!config.value.showTopNav) {
                menus.value = JSON.parse(JSON.stringify(props.allMenus))
            } else if (props.navTopActive != -1) {
                if (props.allMenus?.[props.navTopActive]?.children) {
                    menus.value = toRaw([...props.allMenus[props.navTopActive].children])
                } else {
                    menus.value = []
                }
            }
            renderMenu.value = JSON.parse(JSON.stringify(menus.value))
            jumpMenu()
        }
        const jumpMenu = () => {
            let route = router.currentRoute.value.fullPath
            if (route.toLowerCase() == '/index') {
                if (renderMenu.value.length) {
                    jumpFirstMenu()
                } else if (props.navTopActive != -1) {
                    const { nodeType, route, name } = props.allMenus[props.navTopActive] || {}
                    if (nodeType == 2) {
                        jump(route, name)
                    }
                }
            } else {
                let navTopItem = props.allMenus?.[props.navTopActive]
                if (navTopItem && navTopItem.nodeType == 2) {
                    const { route, name } = navTopItem || {}
                    jump(route, name)
                }
            }
        }
        const jumpFirstMenu = () => {
            let firstMenu = getFirstMenu(renderMenu.value)
            const { route, name } = firstMenu
            jump(route, name)
        }

        const jump = (route) => {
            if (!route) {
                return;
            }
            nextTick(() => {
                if (route.includes('http') && route.includes('target=_blank')) {
                    let jumpUrl = route.replace(/\??target=_blank/, '')
                    window.open(jumpUrl)
                } else {
                    router.push(route)
                }
            })
        }
        const getFirstMenu = (menus) => {
            let firstMenu = {}
            if (menus) {
                for (let item of menus) {
                    if (item.nodeType == 2 && item.route) {
                        firstMenu.route = item.route;
                        firstMenu.name = item.name;
                        break
                    } else if (item.children) {
                        firstMenu = getFirstMenu(item.children)
                        if (firstMenu.route && firstMenu.name) { break }
                    }
                }
            }
            return firstMenu
        }
        const searchMenu = () => {
            renderMenu.value = findMenu(menuSearch.value, toRaw(menus.value))
        }

        // 导航菜单折叠显示
        const onAsideListShow = () => {
            if (transition.value) {
                return
            }
            transition.value = true
            isCollapse.value = !isCollapse.value
            setTimeout(() => {
                transition.value = false
            }, 600)
        }
        const findMenu = (value, arr) => {
            let newarr = [];
            arr.forEach(element => {
                if (element.name.toLowerCase().indexOf(String(value).toLowerCase()) > -1) { // 判断条件
                    newarr.push(element);
                } else {
                    if (element.children && element.children.length > 0) {
                        let redata = findMenu(value, element.children || []);
                        if (redata && redata.length > 0) {
                            let obj = {
                                ...element,
                                children: redata
                            };
                            newarr.push(obj);
                        }
                    }
                }
            });
            return newarr;
        }
        const logoJump = () => {
            let logoJumpPath = config?.value?.logoJumpPath?.url
            if (logoJumpPath) {
                jump(logoJumpPath)
            }
        }
        const setActiveMenu = () => {
            let route = router.currentRoute.value.fullPath
            if (route.toLowerCase() != '/index') {
                menuActive.value = route
            }
        }

        const resizeEnd = () => {
            globalProperties.$bus.emit('topResize')
        }

        watch(() => props.navTopActive, () => {
            initMenu()
        }, { immediate: true })

        watch(() => props.allMenus, () => {
            initMenu()
            setActiveMenu()
        }, { immediate: true })

        watch(menuSearch, (val) => {
            if (!val) {
                searchMenu()
            }
        })
        watch(() => router.currentRoute.value.fullPath, () => {
            setActiveMenu()
        }, { immediate: true })

        globalProperties.$bus.on('navTopSelectChange', () => {
            jumpFirstMenu()
        })

        const showImg = () => {
            if (process.env.NODE_ENV === "development") { theme.value.logo = `/static/images/${localStorage.theme == 'dark' ? 'index-logo-src-dark' : 'index-logo-src-light'}.svg` }
        }

        return {
            logoJump,
            resizeEnd,
            onAsideListShow,
            searchMenu,
            menuSearch,
            isCollapse,
            transition, renderMenu, menus, menuActive, config, theme, showImg, showOrganizationalStructure, organizationName
        }
    },
}
