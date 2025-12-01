import { vLoading as _vLoading } from 'element-plus'
import { Directive, ObjectDirective, getCurrentInstance } from 'vue'

import './index.scss'

const vLoading: Directive<HTMLElement> = {
    mounted(el, binding, vnode, prevVnode) {
        const vueInstance = vnode.ctx.proxy
        el.setAttribute(`element-loading-text`, vueInstance?.$t('login.tips.loading') + '...')
        el.setAttribute(`element-loading-spinner`, ' ')
        el.setAttribute(`element-loading-background`, 'customLoading');
        (_vLoading as ObjectDirective).mounted?.(el, binding, vnode, prevVnode) // 调用element-plus的vLoading
    },
    updated(el, binding, vnode, prevVnode) {
        (_vLoading as ObjectDirective).updated?.(el, binding, vnode, prevVnode) // 调用element-plus的vLoading
    },
    unmounted(el, binding, vnode, prevVnode) {
        (_vLoading as ObjectDirective).unmounted?.(el, binding, vnode, prevVnode) // 调用element-plus的vLoading
    }
}

export default vLoading