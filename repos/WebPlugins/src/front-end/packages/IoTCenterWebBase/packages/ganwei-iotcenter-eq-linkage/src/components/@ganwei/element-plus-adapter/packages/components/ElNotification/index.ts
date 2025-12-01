import { ElNotification as _ElNotification } from 'element-plus'
import { isVNode } from 'vue'

import './index.scss'

const install = _ElNotification.install

const defaultOption = {
    duration: 2000
}

const ElNotification = _ElNotification

const notificationTypes = [
    'success',
    'info',
    'warning',
    'error',
] as const

notificationTypes.forEach(type => {
    ElNotification[type] = (options) => {
        if (typeof options === 'string') {
            return _ElNotification[type](options)
        }
        if (isVNode(options)) {
            return _ElNotification[type](options)
        }
        let data = {
            ...defaultOption,
            ...options,
            customClass: type,
            type: type
        }
        return ElNotification(data)
    }
})
ElNotification.install = (app) => {
    install?.(app)
    app.config.globalProperties.$notify = ElNotification
}
export default ElNotification