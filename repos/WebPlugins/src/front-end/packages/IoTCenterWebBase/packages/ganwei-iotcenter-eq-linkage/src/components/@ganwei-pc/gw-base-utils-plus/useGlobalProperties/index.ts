import { ComponentCustomProperties, getCurrentInstance } from "vue";

export default function (): ComponentCustomProperties {
    const instance = getCurrentInstance()
    if (!instance?.appContext.config.globalProperties) {
        throw new Error('Global properties not found')
    }
    return instance?.appContext.config.globalProperties || {} as ComponentCustomProperties
}
