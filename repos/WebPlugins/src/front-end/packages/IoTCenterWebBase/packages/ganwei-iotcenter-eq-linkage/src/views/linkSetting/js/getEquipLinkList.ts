import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { usePageAxios } from '@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios';

import { LinkSettingService } from '@/request/api';

import { FilterValue } from './useFilterDialog';

/**
 * 获取联动表格
 *
 * @export
 * @param {FilterValue} filterValue
 * @returns
 */
export default function (filterValue: FilterValue) {
    const $t = useI18n().t
    const equipName = ref('')

    const { loading: linkLoad, page, data: tableData, execute: getList } = usePageAxios(LinkSettingService.linkGetList, {
        initialData: [],
        immediate: true,
        beforeEach() {
            const data = {}, osetNoList = getEquipSetLists(filterValue.linkEquipCommand);
            if(equipName.value) data.equipName = equipName.value;
            if(filterValue.triggerEquip) data.iequipNos = filterValue.triggerEquip;
            if(Array.isArray(filterValue.triggerType) && filterValue.triggerType.length > 0) data.iycyxTypes = decodeURIComponent(filterValue.triggerType.join(","));
            if(Array.isArray(filterValue.linkEquip) && filterValue.linkEquip.length > 0) data.oequipNos = decodeURIComponent(filterValue.linkEquip.join(","));
            if(Array.isArray(osetNoList) && osetNoList.length > 0) data.osetNos = osetNoList;
            return data;
        },
        afterEach(res) {
            const getToggleDotName = (type: string, item: { ycYxName: string }) => {
                // evt{*}
                if (/evt\{(\w+)\}/.test(type)) {
                    const matches = type.match(/evt\{(\w+)\}/)!
                    if (matches.length > 1) {
                        return matches[1]
                    }
                }
                return item.ycYxName || 0
            }

            const getToggleDot = (type: string, item: { iycyxNo: number }) => {
                if (/evt\{(\w+)\}/.test(type)) {
                    const matches = type.match(/evt\{(\w+)\}/)!
                    if (matches.length > 1) {
                        return matches[1]
                    }
                }
                return String(item.iycyxNo)
            }
            const togTypeMatch = (target: string) => {
                target = target.startsWith('evt') ? 'evt' : target
                switch (target) {
                    case 'E':
                        return $t('linkSetting.input.equipFault')
                    case 'e':
                        return $t('linkSetting.input.equipRecovery')
                    case 'S':
                        return $t('linkSetting.input.equipTypeFault')
                    case 's':
                        return $t('linkSetting.input.equipTypeRecovery')
                    case 'C':
                        return $t('linkSetting.input.ycLimit')
                    case 'c':
                        return $t('linkSetting.input.ycRecovery')
                    case 'X':
                        return $t('linkSetting.input.yxAlarm')
                    case 'x':
                        return $t('linkSetting.input.yxRecovery')
                    case 'evt':
                        return $t('linkSetting.input.incident')
                    default:
                        return ''
                }
            }

            if (res.data.data) {
                const data = res.data.data.rows || []
                return data.map(item => ({
                    id: item.id,
                    toggleEqName: item.iequipNm,
                    toggleEq: item.iequipNo,
                    toggleType: item.iycyxType.startsWith('evt') ? 'evt' : item.iycyxType,
                    toggleTypeName: togTypeMatch(item.iycyxType),
                    toggleDot: getToggleDot(item.iycyxType, item),
                    ToggleDotName: getToggleDotName(item.iycyxType, item),
                    delay: item.delay,
                    linkEq: item.oequipNo,
                    linkEqName: item.oequipNm,
                    linkOrder: {
                        setName: item.setNm,
                        setType: item.setType,
                        setNo: item.osetNo
                    },
                    value: item.value,
                    remark: item.procDesc,
                    enable: item.enable,
                    editEnable: item.editEnable,
                    isConditionLink: item.isConditionLink,
                    conditionRelation: item.conditionRelation
                }))
            }
            return []
        }
    })

    function search() {
        page.pageNo = 1
        return getList()
    }

    return {
        equipName, linkLoad, page, tableData, getList, search
    }
}

function getEquipSetLists(linkEquipCommand: FilterValue['linkEquipCommand']) {
    const map: Record<string, { equipNo: number, setNos: number[] }> = {}
    for (const command of linkEquipCommand) {
        command.value.forEach(item => {
            const [equipNo, setNo] = item.split('-')
            if (!map[equipNo]) {
                map[equipNo] = {
                    equipNo: Number(equipNo),
                    setNos: []
                }
            }
            map[equipNo].setNos.push(Number(setNo))
        })
    }
    return Array.from(Object.values(map))
}
