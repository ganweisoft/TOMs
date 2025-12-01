import Vue from 'vue'
import Router from 'vue-router'
import equipInfo from './src/equipInfo'
import templateManage from './src/templateManage'
import manageFram from './src/manageFram'

Vue.use(Router)

export default new Router({
    routes: [
        {
            path: '',
            redirect: '/equipInfo'
        },
        {
            path: '/equipInfo',
            name: 'equipInfo',
            component: equipInfo
        },
        {
            path: '/templateManage',
            name: 'templateManage',
            component: templateManage
        },
        {
            path: '/manageFram',
            name: 'manageFram',
            component: manageFram
        }
    ]
})
