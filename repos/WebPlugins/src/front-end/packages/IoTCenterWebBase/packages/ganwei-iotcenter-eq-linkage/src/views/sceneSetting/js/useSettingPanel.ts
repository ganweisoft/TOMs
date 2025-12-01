import { computed, ComputedRef, reactive, Ref, ref } from "vue";
import { useI18n } from "vue-i18n";
import { ElForm } from "element-plus";

export function useTabs(names: string[]) {
    const activeTab = ref(names[0])
    const tabChange = (val: string) => {
        activeTab.value = val;
    }
    const isActive = (val: string) => activeTab.value === val;

    return {
        activeTab, tabChange, isActive
    }
}

export type XhForm = {
    name: string;
    equip: {
        value: string;
        equipSetValue: [string, string];
        defaultValue: string;
        setType: string;
        equipName: string;
        setNm: string;
    };
    conTime: string;
    cycleTaskContent: CycleTaskContent[];
}

export type CycleTaskContent = {
    type: 'E';
    id: string;
    equipNo: string;
    setNo: string;
    value: string;
    equipName: string;
    setNm: string;
    setType: string;
    attribute: string;
    icon: string;
    color: string;
} | {
    type: 'T';
    id: string;
    icon: string;
    color: string;
    conTime: string;
}
type ElFormInstance = InstanceType<typeof ElForm>

export function useEquipControlForm() {
    const $t = useI18n().t
    const nameFormRef = ref<ElFormInstance>()
    const xhEqpFormRef = ref<ElFormInstance>()
    const xhTimeFormRef = ref<ElFormInstance>()
    const cycleTaskContentRef = ref<ElFormInstance>()
    const xhForm = reactive<XhForm>({
        name: '',
        equip: {
            value: '',
            equipSetValue: ['', ''],
            defaultValue: '',
            setType: '',
            equipName: '',
            setNm: ''
        },
        conTime: '',
        cycleTaskContent: []
    })
    const xhRule = computed(() => {
        return {
            name: [{ required: true, message: $t('sceneSetting.tips.scenenName'), trigger: 'blur' }],
            conTime: [
                {
                    trigger: 'blur',
                    required: true,
                    message: $t('sceneSetting.tips.scenenTimeNull')
                }
            ],
            cycleTaskContent: [
                {
                    validator: (rule, value, callback) => {
                        if (value.length === 0) {
                            return callback(new Error($t('sceneSetting.tips.scenenControl')))
                        }
                        let eqSum = 0,
                            timeSum = 0
                        value.forEach(item => {
                            if (item.type === 'T') return timeSum++
                            if (item.type === 'E') return eqSum++
                        })
                        if (eqSum == 0) {
                            return callback(new Error($t('sceneSetting.tips.scenenControl')))
                        }
                        if (timeSum == 0) {
                            return callback(new Error($t('sceneSetting.tips.scenenTime')))
                        }
                        callback()
                    },
                    trigger: 'blur'
                }
            ],
            equip: [
                {
                    validator(rule, value, callback) {
                        if (value.setNm && value.equipName) {
                            return callback()
                        }
                        return callback(new Error($t('sceneSetting.dialog.selectEqpItem')))
                    },
                    trigger: 'blur'
                }

            ]
        }
    })

    const deleteXhFormCycleTaskContent = (ids: string[]) => {
        xhForm.cycleTaskContent = xhForm.cycleTaskContent.filter(item => !ids.includes(item.id))
    }

    const reset = () => {
        xhForm.name = ''
        xhForm.equip = {
            value: '',
            equipSetValue: [-1, -1],
            defaultValue: '',
            setType: '',
            equipName: '',
            setNm: ''
        }
        xhForm.conTime = ''
        xhForm.cycleTaskContent = []
    }

    return {
        nameFormRef, xhEqpFormRef, xhTimeFormRef, cycleTaskContentRef, xhForm, xhRule, deleteXhFormCycleTaskContent, reset
    }
}

export function useCheckBox<R extends Record<string, string> | string>(values: () => R[]): useCheckBoxResult<R>
export function useCheckBox<T extends Record<string, string>, R = string>(values: () => T[], key: keyof T): useCheckBoxResult<R>
export function useCheckBox<T extends Record<string, string> | string, R = T>(values: () => T[], key?: keyof T): useCheckBoxResult<R> {
    const options = computed(() => {
        if (key) return values().map(item => item[key]) as R[]
        return values() as unknown as R[]
    })
    const indeterminate = ref(false)
    const checkedModel = ref<R[]>([]) as Ref<R[]>
    const checkAll = ref(false)

    function checkAllChange(val: boolean) {
        if (val) {
            checkedModel.value = [...options.value]
        } else {
            checkedModel.value = []
        }
    }

    function checkChange(value: R[]) {
        const checkedCount = value.length;
        checkAll.value = checkedCount === options.value.length;
        indeterminate.value = checkedCount > 0 && checkedCount < options.value.length;
    }

    function reset() {
        indeterminate.value = false
        checkedModel.value = []
        checkAll.value = false
    }

    return {
        options,
        indeterminate,
        checkedModel,
        checkAll,
        checkAllChange,
        checkChange,
        reset
    }
}

type useCheckBoxResult<R> = {
    options: ComputedRef<R[]>,
    indeterminate: Ref<boolean>,
    checkedModel: Ref<R[]>,
    checkAll: Ref<boolean>,
    checkAllChange: (val: boolean) => void,
    checkChange: (value: R[]) => void,
    reset: () => void
}
