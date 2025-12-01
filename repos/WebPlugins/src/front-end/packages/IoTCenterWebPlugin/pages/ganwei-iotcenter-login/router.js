import Vue from 'vue'
import Router from 'vue-router'

const LOGIN = () => import('./src/Login.vue');
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
        }
    ]
})