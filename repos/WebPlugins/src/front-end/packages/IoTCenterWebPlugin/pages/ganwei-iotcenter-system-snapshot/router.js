import Vue from 'vue'
import Router from 'vue-router'
import systemSnapshot from './src/systemSnapshot'

Vue.use(Router)

export default new Router({
    routes: [{
            path: '',
            redirect: '/systemSnapshot'
        },
        {
            path: '/systemSnapshot',
            name: 'systemSnapshot',
            component: systemSnapshot
        }
    ]
})