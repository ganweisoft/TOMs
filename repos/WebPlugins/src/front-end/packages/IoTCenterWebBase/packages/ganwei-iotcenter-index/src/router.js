
/* eslint-disable */
import {
    createRouter,
    createWebHashHistory
} from 'vue-router'

const INDEX = () => import('./views/Index.vue')
const jumpIframe = () => import('@/views/jumpIframe/index.vue')
const router = createRouter({
    history: createWebHashHistory(),
    routes: [{
        path: '/',
        redirect: '/Index'
    },
    {
        path: '/Index',
        name: 'INDEX',
        component: INDEX,
        children: [
            {
                path: 'jumpIframe/:packageName([^/]*)/:route(.*)',
                name: 'jumpIframe3',
                component: jumpIframe
            }
        ]
    },
    ]
})
router.beforeEach(async (to, from, next) => {
    next()
})
router.afterEach((to, from, next) => { })
export default router
