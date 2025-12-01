import {
    createRouter,
    createWebHashHistory
} from 'vue-router'
import linkSetting from './views/linkSetting/linkSetting.vue'
import sceneSetting from './views/sceneSetting/sceneSetting.vue'

const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        {
            path: '',
            redirect: '/linkSetting'
        },
        {
            path: '/linkSetting',
            name: 'linkSetting',
            component: linkSetting
        },
        {
            path: '/sceneSetting',
            name: 'sceneSetting',
            component: sceneSetting
        }
    ]
})
router.beforeEach((to, from, next) => {
    next()
})
router.afterEach((to, from, next) => { })
export default router
