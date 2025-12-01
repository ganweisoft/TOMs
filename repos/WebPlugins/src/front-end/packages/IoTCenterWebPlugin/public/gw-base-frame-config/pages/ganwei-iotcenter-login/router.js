import Vue from 'vue'
import Router from 'vue-router'

const LOGIN = () => import('./src/Login.vue');
const SSOLOGIN = () => import('./src/ssoLogin.vue');
const SSOLOGOUT = () => import('./src/ssoLogout.vue');

// 许可维护
const MAINTENANCEINFO = () =>
    import('./src/maintenanceInfo.vue');
const MAININFO = () => import('./src/mainInfo.vue');
Vue.use(Router)

export default new Router({
    routes: [
        {
            path: '/',
            name: 'Login',
            component: LOGIN
        },
        {
            path: '/Login',
            name: 'Login',
            component: LOGIN
        },
        {
            path: '/ssoLogin/:appid',
            name: 'ssoLogin',
            component: SSOLOGIN
        },
        {
            path: '/ssoLogin',
            name: 'ssoLogin',
            component: SSOLOGIN
        },
        {
            path: '/ssoLogout',
            name: 'ssoLogout',
            component: SSOLOGOUT
        },
        {
            path: '/ssoLogout/:appid',
            name: 'ssoLogout',
            component: SSOLOGOUT
        },
        {
            path: '/Maintain',
            name: 'Maintain',
            component: MAINTENANCEINFO
        },
        {
            path: '/mainInfo',
            name: 'mainInfo',
            component: MAININFO
        }
    ]
})