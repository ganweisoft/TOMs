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

    function getElementHeight(el) {
        const stylesheet = getComputedStyle(el)
        return el.clientHeight || getPxNumber(stylesheet.height)
    }

    function resizeTable() {
        el = instance?.proxy?.$el?.parentElement;
        if (el) {
            const stylesheet = getComputedStyle(el)
            height.value = getElementHeight(el) - getPxNumber(stylesheet.paddingTop) - getPxNumber(stylesheet.paddingBottom)
        }
    }
    return height
}