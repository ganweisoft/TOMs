import Vue from 'vue'
import Router from 'vue-router'
import taskRepository from './src/taskRepository'
import weekTaskNew from './src/weekTaskNew'
import specialTask from './src/specialTask'

Vue.use(Router)

export default new Router({
    routes: [{
            path: '',
            redirect: '/taskRepository'
        },
        {
            path: '/taskRepository',
            name: 'taskRepository',
            component: taskRepository
        },
        {
            path: '/weekTaskNew',
            name: 'weekTaskNew',
            component: weekTaskNew
        },
        {
            path: '/specialTask',
            name: 'specialTask',
            component: specialTask
        }
    ]
})