/* eslint-disable */
import Vue from 'vue'
import Router from 'vue-router'
const INDEX = () => import('./views/Index.vue')
import encrypt from 'gw-base-api-plus/encrypt.js'

const NOACCESS = () => import('gw-base-noAccess/noAccess.vue')
const JUMPIFRAMED = () => import('./views/jumpIframe/jumpIframePro.vue')
const JUMPiFRAMEDLOGIN = () => import('./views/jumpIframe/jumpIframeLogin.vue')

const noPage = () => import('gw-base-noAccess/noPage.vue')

const originalPush = Router.prototype.push
Router.prototype.push = function push (location, onResolve, onReject) {
    if (onResolve || onReject) return originalPush.call(this, location, onResolve, onReject)
    return originalPush.call(this, location).catch(err => err)
}

Vue.use(Router)
const router = new Router({
    mode: 'hash',
    base: process.env.BASE_URL,
    linkActiveClass: 'active',
    routes: [
        {
            path: '/',
            redirect: 'jumpIframeLogin/ganwei-iotcenter-login/Login'
        },
        {
            path: '/jumpIframeLogin/*',
            name: 'jumpIframeLogin',
            component: JUMPiFRAMEDLOGIN
        },
        {
            path: '/noPage',
            name: 'noPage',
            component: noPage
        },
        {
            path: '/Index',
            component: INDEX,
            children: [
                {
                    path: 'noAccess',
                    name: 'noAccess',
                    component: NOACCESS
                },
                {
                    path: 'jumpIframe/*',
                    name: 'jumpIframe',
                    component: JUMPIFRAMED
                }
            ]
        }
    ]
})
router.beforeEach((to, from, next) => {
    if (to.path === '/jumpIframeLogin/ganwei-iotcenter-login/Login') {
        window.sessionStorage.removeItem('asideList')
        encrypt.clearMyKey()
    }
    // 判断是否跳转去单点登录相关页面
    if (
        to.path.includes('/jumpIframeLogin/ganwei-iotcenter-login/ssoLogin') ||
        to.path.includes('/jumpIframeLogin/ganwei-iotcenter-login/ssoLogout') ||
        (from.path.includes('/jumpIframeLogin/ganwei-iotcenter-login/ssoLogin') && to.path == '/Index')
    ) {
        next()
        return
    }

    // 判断是否跳转去登录页、无权限页、许可文件页、从登录页到首页
    if (
        to.path == '/jumpIframeLogin/ganwei-iotcenter-login/Login' ||
        to.path == '/Index/noAccess' ||
        ((from.path == '/jumpIframeLogin/ganwei-iotcenter-login/Login' || from.path == '/jumpIframeLogin/ganwei-iotcenter-login/login') && to.path == '/Index') ||
        from.path == '/Index' ||
        to.path == '/jumpIframeLogin/ganwei-iotcenter-login/Maintain' ||
        to.path == '/jumpIframeLogin/ganwei-iotcenter-login/mainInfo' ||
        to.path.includes('noPage')
    ) {
        next()
        return
        // 判断是否从登录页到首页
    } else if ((from.path != '/jumpIframeLogin/ganwei-iotcenter-login/login' || from.path != '/jumpIframeLogin/ganwei-iotcenter-login/Login') && to.path == '/Index') {
        if (window.sessionStorage.asideList) {
            let asideList = JSON.parse(window.sessionStorage.asideList)

            // 兼容多层级菜单登录进来后选中第一项菜单
            function forList (list) {
                if ('route' in list[0] && list[0].route) {
                    return list[0].route
                } else if ('children' in list[0] && list[0].children.length > 0) {
                    return forList(list[0].children)
                }
            }
            next(asideList[0].route ? asideList[0].route : forList(asideList[0].children))
        } else {
            next()
        }
    } else {
        let path = to.fullPath
        if (window.sessionStorage.asideList) {
            let asideList = JSON.parse(window.sessionStorage.asideList)
            let haveRoute = false
            if(asideList)
             forList(asideList)

            function forList (list) {
                for (let i = 0; i < list.length; i++) {
                    try{
                    if (list[i].route && list[i].route.split('?')[0] == path.split('?')[0]) {
                        haveRoute = true
                    }
                    if ('children' in list[i]) {
                        forList(list[i].children)
                    }
                    } catch(e){

                    }
                }
            }
            if (haveRoute) {
                next()
            } else {
                next(from.path)
                // return false;
            }
        } else {
            next('/jumpIframeLogin/ganwei-iotcenter-login/Login')
        }
    }
})
router.afterEach((to, from) => { })
export default router
