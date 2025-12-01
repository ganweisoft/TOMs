import { computed, reactive, ref } from "vue";
import { useI18n } from "vue-i18n";

import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios";

import { LinkSettingService } from "@/request/api";

export type FilterValue = {
    triggerEquip: number[],
    triggerType: string[],
    linkEquip: number[],
    linkEquipCommand: Array<{ name: string, value: string[] }>,
    triggerTimeMin: number,
    triggerTimeMax: number
}

/**
 * 过滤联动表格弹窗
 *
 * @export
 * @returns
 */
export default function () {
    const filterVisible = ref(false)
    const filterValue = reactive<FilterValue>({
        triggerEquip: [],
        triggerType: [],
        linkEquip: [],
        linkEquipCommand: [],
        triggerTimeMin: 0,
        triggerTimeMax: 10000
    })
    const { equipList, getEquipList } = GetEquipList()
    const triggerTypes = GetTriggerTypes()
    const { getLinkEquipCommands, linkCommands } = GetLinkEquipCommands()

    function openFilterDialog() {
        getEquipList()
        filterVisible.value = true
    }

    function changeFilterLinkEquip(val: number[]) {
        getLinkEquipCommands({ equipList: val })
    }

    function resetFilter() {
        filterValue.triggerEquip = []
        filterValue.triggerType = []
        filterValue.linkEquip = []
        filterValue.linkEquipCommand = []
        filterValue.triggerTimeMin = 0
        filterValue.triggerTimeMax = 10000
        filterVisible.value = false
    }

    return {
        filterValue, filterVisible, resetFilter, equipList, openFilterDialog, triggerTypes, changeFilterLinkEquip, linkCommands
    }
}

function GetEquipList() {
    const { data: equipList, execute: getEquipList } = useAxios(LinkSettingService.linkGetChoice, {
        initialData: {},
        afterEach(res) {
            if (res.data.data) {
                const iList = res.data.data.iList || []
                const oList = res.data.data.oList || []
                return {
                    triggerEquips: distinctObjectArray(iList, 'equipNo'),
                    linkEquips: distinctObjectArray(oList, 'equipNo')
                }
            }
            return {
                triggerEquips: [],
                linkEquips: []
            }
        }
    })

    return {
        equipList,
        getEquipList
    }
}

export function GetTriggerTypes() {
    const $t = useI18n().t
    return computed(() => {
        return [
            {
                label: $t('linkSetting.input.equipFault'),
                value: 'E'
            },
            {
                label: $t('linkSetting.input.equipRecovery'),
                value: 'e'
            },
            {
                label: $t('linkSetting.input.equipTypeFault'),
                value: 'S'
            },
            {
                label: $t('linkSetting.input.equipTypeRecovery'),
                value: 's'
            },
            {
                label: $t('linkSetting.input.ycLimit'),
                value: 'C'
            },
            {
                label: $t('linkSetting.input.ycRecovery'),
                value: 'c'
            },
            {
                label: $t('linkSetting.input.yxAlarm'),
                value: 'X'
            },
            {
                label: $t('linkSetting.input.yxRecovery'),
                value: 'x'
            },
            {
                label: $t('linkSetting.input.incident'),
                value: 'evt'
            },
        ]
    })
}

function GetLinkEquipCommands() {

    const { execute: getLinkEquipCommands, data: linkCommands } = useAxios(LinkSettingService.GetSetParmByEquipCommand, {
        afterEach(res) {
            if (res.data.data) {
                const map: Record<string, string[]> = {}
                res.data.data.forEach(item => {
                    item.setParmList.forEach(set => {
                        if (!map[set.setNm]) {
                            map[set.setNm] = []
                        }
                        map[set.setNm].push(set.equipNo + '-' + set.setNo)
                    })
                })
                return Object.keys(map).map(name => ({
                    name: name,
                    value: map[name]
                }))
            }
            return []
        },
    })

    return {
        getLinkEquipCommands, linkCommands
    }
}

function distinctObjectArray<T extends object>(arr: Array<T>, key: keyof T) {
    const map: Record<string | number, T> = {}
    for (const item of arr) {
        const no = item[key] as string | number
        map[no] = item
    }
    return Array.from(Object.values(map))
}
