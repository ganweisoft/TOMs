import Vue from 'vue'
import Router from 'vue-router'
import logPreview from './src/logPreview'

Vue.use(Router)

export default new Router({
    routes: [{
            path: '',
            redirect: '/logPreview'
        },
        {
            path: '/logPreview',
            name: 'logPreview',
            component: logPreview
        }
    ]
})