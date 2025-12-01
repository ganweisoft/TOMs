import {
    createRouter,
    createWebHashHistory
} from 'vue-router'

const LOGIN = () => import('./views/login.vue');

const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        {
            path: '/',
            redirect: '/Login'
        },
        {
            path: '/Login',
            name: 'Login',
            component: LOGIN
        }
    ]
})
router.beforeEach((to, from, next) => {
    next()
})
router.afterEach((to, from, next) => { })
export default router
