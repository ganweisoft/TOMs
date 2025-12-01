import { ref } from "vue"

export type EquipSelectChangeHook<T = any> = (val: Array<{ equipNo: number, equipName: string }>) => T

/**
 * 设备选中复用
 *
 * @export
 * @param {EquipSelectChangeHook} onChange
 * @returns
 */
export default function useEquipSelect(onChange: EquipSelectChangeHook) {

    const equipSelectVisible = ref(false)
    const selectedEquip = ref<number>(-1)
    const ifSelectTriggerEquip = ref(null)

    const selectedChange: EquipSelectChangeHook = (val) => {
        if (val.length > 0) {
            val[0].index = ifSelectTriggerEquip.value
            onChange(val)
            selectedEquip.value = val[0].equipNo
        }
        equipSelectVisible.value = false
    }

    function showEquipSelect(val?: number) {
        ifSelectTriggerEquip.value = typeof val === 'number' ? val : null
        equipSelectVisible.value = true;
    }
    return {
        equipSelectVisible, selectedChange, showEquipSelect, selectedEquip
    }
}
