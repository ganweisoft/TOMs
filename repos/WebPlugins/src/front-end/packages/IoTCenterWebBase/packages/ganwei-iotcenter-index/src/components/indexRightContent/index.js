import { computed, defineAsyncComponent, getCurrentInstance, inject, nextTick } from 'vue'
const topNav = defineAsyncComponent(() => { return import('@/components/asideMenu/topNav.vue') })
const headerRight = defineAsyncComponent(() => { return import('./headerRight/index.vue') })
const normalPage = defineAsyncComponent(() => { return import('./normalPage/index.vue') })
const labelPage = defineAsyncComponent(() => { return import('./labelPage/index.vue') })
export default {
    components: {
        topNav,
        headerRight,
        normalPage,
        labelPage
    },
    model: {
        prop: "modelValue",
        event: 'update'
    },
    emits: ['update:modelValue'],
    props: {
        modelValue: { //获取父组件的数据value
            type: [Number, String],
            default: -1
        },
        allMenus: {
            type: Object,
            default: () => []
        },
        loading: {
            type: Boolean,
            default: false
        }
    },
    setup (props, context) {
        const config = inject('config')
        const navTopOpt = computed(() => {
            return props.allMenus.map((group, index) => {
                if (config.value.showTopNav) {
                    const { name, resourceId: id, route, icon, backgroundColor } = group
                    return { name, id, index, route, icon, backgroundColor }
                }
            })
        })
        const navTopActive = computed({
            get: () => props.modelValue,
            set: (val) => {
                context.emit('update:modelValue', val)
            }
        })
        const { appContext: { config: { globalProperties } } } = getCurrentInstance()

        const topNavSelect = () => {
            nextTick(() => {
                globalProperties.$bus.emit('navTopSelectChange')
            })
        }

        return {
            navTopOpt,
            navTopActive,
            topNavSelect,
            config
        }
    }
}
