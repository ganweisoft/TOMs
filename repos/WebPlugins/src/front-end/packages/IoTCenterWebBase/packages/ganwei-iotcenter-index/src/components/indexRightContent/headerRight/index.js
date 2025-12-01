
import contentFullScreen from './contentFullScreen/index.vue'
import fullScreen from './fullScreen/index.vue'
import switchTheme from './switchTheme/index.vue'
import unReadMessages from './unReadMessages/index.vue'
import userInfo from './userInfo/index.vue'

export default {
    components: {
        fullScreen,
        contentFullScreen,
        unReadMessages,
        switchTheme,
        userInfo
    },
    inject: ['config', 'theme'],
}
