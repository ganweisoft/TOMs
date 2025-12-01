import Vue from 'vue'
import Router from 'vue-router'
import equipListsIot from './src/equipList'

Vue.use(Router)

export default new Router({
    routes: [{
        path: '/',
        redirect: '/equipListsIot'
    },
    {
        path: '/equipListsIot',
        name: 'equipListsIot',
        component: equipListsIot
    }
    ]
})