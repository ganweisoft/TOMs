import publics from './publics.js';
import taskRepository from './taskRepository.js'
import weekTaskNew from './weekTaskNew.js'
import specialTask from './specialTask.js'
import enLocale from 'element-ui/lib/locale/lang/en'
export default {
    publics,
    taskRepository,
    weekTaskNew,
    specialTask,
    ...enLocale
}
