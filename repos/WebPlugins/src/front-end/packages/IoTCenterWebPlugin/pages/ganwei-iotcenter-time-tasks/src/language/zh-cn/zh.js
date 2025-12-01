import publics from './publics.js';
import taskRepository from './taskRepository.js'
import weekTaskNew from './weekTaskNew.js'
import specialTask from './specialTask.js'
import zhLocale from 'element-ui/lib/locale/lang/zh-CN'
export default {
    publics,
    taskRepository,
    weekTaskNew,
    specialTask,
    ...zhLocale
}
