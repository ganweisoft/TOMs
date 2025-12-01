import Vue from 'vue'
import Router from 'vue-router'
import eqEvent from './src/eqEvent'
import sysEvent from './src/sysEvent'
import queryData from './src/queryData'

Vue.use(Router)

export default new Router({
    routes: [{
        path: '',
        redirect: '/eqEvent'
    },
    {
        path: '/eqEvent',
        name: 'eqEvent',
        component: eqEvent
    },
    {
        path: '/sysEvent',
        name: 'sysEvent',
        component: sysEvent
    },
    {
        path: '/queryData',
        name: 'queryData',
        component: queryData
    }
    ]
})