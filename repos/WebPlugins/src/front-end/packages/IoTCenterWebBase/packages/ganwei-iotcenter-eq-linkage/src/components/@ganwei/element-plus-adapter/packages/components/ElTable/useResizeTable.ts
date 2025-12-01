import { getCurrentInstance, onBeforeUnmount, onMounted, ref } from "vue";

export default function useResizeTable() {
    const instance = getCurrentInstance()
    // 表格高度
    const height = ref<number>()
    let el: HTMLElement | null = null
    onMounted(() => {
        resizeTable()
        window.addEventListener('resize', resizeTable)
    })

    onBeforeUnmount(() => {
        window.removeEventListener('resize', resizeTable)
    })

    function getPxNumber(value: string) {
        if (value.endsWith('px')) {
            return Number(value.slice(0, -2))
        }
        return 0
    }

    function resizeTable() {
        el = instance?.proxy?.$el?.parentElement;
        if (el) {
            const stylesheet = getComputedStyle(el)
            height.value = el.clientHeight - getPxNumber(stylesheet.paddingTop) - getPxNumber(stylesheet.paddingBottom)
        }
    }
    return height
}