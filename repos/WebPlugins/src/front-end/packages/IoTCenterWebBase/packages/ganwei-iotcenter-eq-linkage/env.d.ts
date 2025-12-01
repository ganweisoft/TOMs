import dayjs from "dayjs"
import { IPartialNotify } from '@ganwei-pc/gw-base-utils-plus/notification'

declare module '*.vue' {
    import type { DefineComponent } from 'vue'
    // eslint-disable-next-line @typescript-eslint/no-explicit-any, @typescript-eslint/ban-types
    const component: DefineComponent<{}, {}, any>
    export default component
}

declare module 'vue' {
    interface ComponentCustomProperties {
        $moment: typeof dayjs
        $message: IPartialNotify
    }
}

declare global {
    interface Window {
        isProduction: boolean
        executeQueues?: {
            deleteQueryCache?: () => void
        }
    }
}

export { }
