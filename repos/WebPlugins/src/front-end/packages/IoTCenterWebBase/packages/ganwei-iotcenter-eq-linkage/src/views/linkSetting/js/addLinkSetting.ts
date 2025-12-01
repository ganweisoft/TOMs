import { computed, reactive, Ref, ref, watch } from "vue"
import { useI18n } from "vue-i18n"
import { ElForm } from 'element-plus';

import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios"
import { useMessage } from "@components/@ganwei-pc/gw-base-utils-plus/notification"

import { LinkSettingService } from "@/request/api"
import { IEquipYcYxSetNumResponse, ISetParamResponse } from "@/request/models"

import {IfGetConditionLinkByAutoProcId} from "./getIfInfo";
import { GetSetParmByEquipNo, GetYcpByEquipNo, GetYcYxSetNumByEquipNo, GetYxpByEquipNo, IfGetEquipYcYxps } from "./useEquip";
import useEquipSelect, { EquipSelectChangeHook } from "./useEquipSelect";
import { GetTriggerTypes } from "./useFilterDialog"

type AddForm = {
    id: number | null;
    toggleEq: number;
    toggleType: string;
    toggleDot: string;
    delay: string;
    linkEq: number;
    linkOrder: Pick<ISetParamResponse, 'setType' | 'setNo'>;
    remark: string;
    value: string;
    linkEqName: string;
    toggleEqName: string;
    enable: boolean;
    procName: string;
    ifLinkEquipList: any;
    isConditionLink: boolean;
    conditionRelation: boolean;
}

type ElFormInstance = InstanceType<typeof ElForm>

export default function () {
    const $t = useI18n().t
    const isView = ref(false)
    const isEdit = ref(false)
    const addType = ref(1)
    const dialogTitle = computed(() => !isEdit.value ? (addType.value == 1 ? $t('linkSetting.dialog.newTitleName') : $t('linkSetting.dialog.newIfTitleName')) : (isView.value ? $t('linkSetting.dialog.editTitleName') : $t('linkSetting.配置预览')))
    const addVisible = ref(false)
    const ifOptions = [
        {
          value: 0,
          label: $t('linkSetting.label.equal')
        },
        {
          value: 1,
          label: $t('linkSetting.label.notEqual')
        },
        {
          value: 2,
          label: $t('linkSetting.label.greaterThan')
        },
        {
          value: 4,
          label: $t('linkSetting.label.greaterThanOrEqualTo')
        },
        {
          value: 8,
          label: $t('linkSetting.label.lessThan')
        },
        {
            value: 16,
            label: $t('linkSetting.label.lessThanOrEqualTo')
          }
      ]

      const ifCommandOptions = [
        {
          value: '0',
          label: $t('linkSetting.label.notcommunicating')
        },
        {
          value: '1',
          label: $t('linkSetting.label.communicationisnormal')
        },
        {
          value: '2',
          label: $t('linkSetting.label.thereisanalarm')
        },
        {
          value: '3',
          label: $t('linkSetting.label.settingupinprogress')
        },
        {
          value: '4',
          label: $t('linkSetting.label.initializing')
        },
        {
            value: '5',
            label: $t('linkSetting.label.withdrawagarrison')
          }
      ]

    // 添加联动
    const { addForm, addRules, linkInfoNew, addLinking, addLoading, addIfLinking, addIfLoading, resetAdd, addItem, removeItem, addChildrenItem, removeChildrenItem } = AddLinking(addType.value)

    // 编辑联动
    const { editLoading, editIfLoading, editLinking, ifEditLinking } = EditLinking(addForm, linkInfoNew)

    // 添加联动时 选中触发设备，联动触发类型、触发点
    const { getYcYxSetNumByEquipNo, ycYxSetNum } = GetYcYxSetNumByEquipNo()

    // 获取所有参数
    const {ifGetEquipYcYxps} = IfGetEquipYcYxps()
    const{ifGetConditionLinkByAutoProcId} = IfGetConditionLinkByAutoProcId()
    const traiggerEquipChange: EquipSelectChangeHook = (val) => {
        if(typeof val[0].index === 'number') {
            addForm.ifLinkEquipList[val[0].index].iequipNm = val[0].equipName
            addForm.ifLinkEquipList[val[0].index].iequipNo = val[0].equipNo
            const data = ifGetEquipYcYxps(val[0].equipNo)
            data.then(res => {
                addForm.ifLinkEquipList[val[0].index].controlCommand = res
            })
            return;
        }
        if (addForm.toggleEq !== val[0].equipNo) {
            addForm.toggleEqName = val[0].equipName
            addForm.toggleEq = val[0].equipNo
            addForm.toggleType = ''
            addForm.toggleDot = ''
        }
        getYcYxSetNumByEquipNo(val)
    }
    const { equipSelectVisible, selectedEquip, selectedChange: triggerSelectedChange, showEquipSelect } = useEquipSelect(traiggerEquipChange)
    const { addTriggerTypes, triggerPoints, triggerTypeChange } = GetTriggerPoints(addForm, selectedEquip, ycYxSetNum)

    // 添加联动时 选中联动设备，联动命令列表
    const { getSetParmByEquipNo, setParams } = GetSetParmByEquipNo()

    const linkEquipChange: EquipSelectChangeHook<ReturnType<typeof getSetParmByEquipNo>> = (val) => {
        if (addForm.linkEq !== val[0].equipNo) {
            addForm.linkEqName = val[0].equipName
            addForm.linkEq = val[0].equipNo
            addForm.linkOrder = {}
            addForm.value = ''
        }
        return getSetParmByEquipNo(val)
    }

    const { equipSelectVisible: linkEquipSelectVisible, selectedChange: linkSelectedChange, showEquipSelect: showLinkEquipSelect } = useEquipSelect(linkEquipChange)

    function getOrder(val: ISetParamResponse) {
        if (val.setType != "V") {
            addForm.value = val.value
        }
    }

    function showAdd(val: number) {
        addVisible.value = true
        isEdit.value = false
        isView.value = true
        addType.value = val
    }

    // 编辑时 覆盖属性
    function showEdit(row: AddForm, status: boolean) {
        addType.value = row.isConditionLink ? 2 : 1
        addVisible.value = true
        isEdit.value = true
        isView.value = status
        for (const _key in row) {
            const key = _key as keyof AddForm;
            addForm[key] = row[key]
        }
        triggerSelectedChange([{ equipNo: row.toggleEq, equipName: row.toggleEqName }])
        linkEquipChange([{ equipNo: row.linkEq, equipName: row.linkEqName }]).then(() => {
            const findItem = setParams.value?.find(item => item.setNo === row.linkOrder.setNo)
            if (findItem) {
                addForm.linkOrder = findItem
            }
        })
        triggerTypeChange(row.toggleType, false)
        if(addType.value === 2) { // 条件联动编辑
            const dt = ifGetConditionLinkByAutoProcId(row.id)
            dt.then(res => {
                addForm.ifLinkEquipList = res.iConditionItems
                addForm.procName = res.procName
                addForm.conditionRelation = row.conditionRelation === 0 ? false : true
                addForm.ifLinkEquipList.forEach((item, index) => {
                    const data = ifGetEquipYcYxps(item.iequipNo)
                    Array.isArray(item.iYcYxItems) && item.iYcYxItems.forEach(childrenItem => {
                        childrenItem.iycyxNo = childrenItem.iycyxNo + "-" + childrenItem.iycyxType
                    });
                    data.then(res => {
                        addForm.ifLinkEquipList[index].controlCommand = res
                    })
                })
            })
        }
    }

    function addorEditLinking() {
        if (isEdit.value) {
            return (addType.value == 1 ? editLinking() : ifEditLinking())
        }
        return (addType.value == 1 ? addLinking() : addIfLinking())
    }

    return {
        getOrder, isEdit, isView, dialogTitle, showAdd, showEdit, addorEditLinking,
        addVisible, resetAdd, addForm, addRules, addLinking, linkInfoNew, addLoading,
        equipSelectVisible, triggerSelectedChange, showEquipSelect, addTriggerTypes, triggerPoints, triggerTypeChange,
        linkEquipSelectVisible, linkSelectedChange, showLinkEquipSelect, setParams,
        editLoading, editIfLoading, addIfLoading, ifOptions, ifCommandOptions, addType, addItem, removeItem, addChildrenItem, removeChildrenItem
    }
}

/**
 * 根据选中设备 获取触发类型、触发点的禁用
 *
 * @param {AddForm} form
 * @param {Ref<number>} selectedEquip
 * @param {(Ref<IEquipYcYxSetNumResponse | undefined>)} ycYxSetNum
 * @returns
 */
function GetTriggerPoints(form: AddForm, selectedEquip: Ref<number>, ycYxSetNum: Ref<IEquipYcYxSetNumResponse | undefined>) {
    const $t = useI18n().t
    const $message = useMessage()
    const types = GetTriggerTypes()
    const addTriggerTypes = computed(() => {
        let value = [...types.value]
        if (ycYxSetNum.value && ycYxSetNum.value.ycNum <= 0) {
            value = value.filter(i => !['C', 'c'].includes(i.value))
        }
        if (ycYxSetNum.value && ycYxSetNum.value.yxNum <= 0) {
            value = value.filter(i => !['X', 'x'].includes(i.value))
        }
        return value
    })

    const { ycp, getYcpByEquipNo } = GetYcpByEquipNo(selectedEquip)
    const { yxp, getYxpByEquipNo } = GetYxpByEquipNo(selectedEquip)

    const triggerPoints = computed(() => {
        if (['X', 'x'].includes(form.toggleType)) {
            return yxp.value
        }
        if (['C', 'c'].includes(form.toggleType)) {
            return ycp.value
        }
        return []
    })

    function triggerTypeChange(val: string, reset = true) {
        if (reset) {
            if (val === 'evt') {
                form.toggleDot = '0'
            } else {
                form.toggleDot = ''
            }
        }
        if (selectedEquip.value === -1) {
            $message.warning($t('linkSetting.tips.selTriEquip'))
            return
        }
        if (['C', 'c'].includes(val)) {
            return getYcpByEquipNo()
        }
        if (['X', 'x'].includes(val)) {
            return getYxpByEquipNo()
        }

    }
    return { addTriggerTypes, triggerPoints, triggerTypeChange }
}

/**
 * 添加联动设置
 *
 * @returns
 */
function AddLinking() {
    const $message = useMessage()
    const $t = useI18n().t
    const addForm = reactive<AddForm>({
        id: null,
        toggleEq: -1,
        toggleType: '',
        toggleDot: '',
        delay: '',
        linkEq: -1,
        linkOrder: {
            setType: '',
            setNo: -1
        },
        remark: '',
        value: '',
        linkEqName: '',
        toggleEqName: '',
        enable: true,
        conditionRelation: false,
        procName: '',
        ifLinkEquipList: [{
            iequipNo: 0,
            iequipNm: '',
            controlCommand: [],
            iYcYxItems: [{
                iycyxType: null,
                iycyxNo: null,
                condition: 0,
                iycyxValue: ''
            }],
        }]
    })
    const addRules = computed(() => {
        return {
            procName: [
                {
                    required: true,
                    message: $t('linkSetting.input.newLinkInfoProcName'),
                    trigger: 'change'
                }
            ],
            toggleEqName: [
                {
                    required: true,
                    message: $t('linkSetting.input.triggerDevice'),
                    trigger: 'change'
                }
            ],
            toggleType: [
                {
                    required: true,
                    message: $t('linkSetting.input.triggerType'),
                    trigger: 'change'
                }
            ],
            toggleDot: [
                {
                    required: true,
                    message: $t('linkSetting.input.triggerPoint'),
                    trigger: 'change'
                }
            ],
            delay: [
                {
                    required: true,
                    message: $t('linkSetting.input.delayTime'),
                    trigger: 'change'
                }
            ],
            linkEqName: [
                {
                    required: true,
                    message: $t('linkSetting.input.linkEquip'),
                    trigger: 'change'
                }
            ],
            linkOrder: [
                {
                    required: true,
                    message: $t('linkSetting.input.linkCommand'),
                    trigger: 'change'
                },
                {
                    validator(rule: any, value: any, callback: any) {
                        if (!value.setType || !value.setNo) {
                            callback(new Error($t('linkSetting.input.linkCommand')))
                        }
                        callback()
                    },
                    trigger: 'blur'
                }
            ],
            value: [
                {
                    required: true,
                    message: $t('linkSetting.input.commandInstructions'),
                    trigger: 'change'
                }
            ]
        }
    })
    const linkInfoNew = ref<ElFormInstance>()
    const { execute: addLinking, loading: addLoading } = useAxios(LinkSettingService.linkAdd, {
        async validate() {
            return linkInfoNew.value?.validate().then(() => {
                return true
            }).catch(() => {
                return false
            })
        },
        beforeEach() {
            return {
                iequipNo: Number(addForm.toggleEq),
                iycyxNo: addForm.toggleType === 'evt' ? 0 : Number(addForm.toggleDot),
                iycyxType: addForm.toggleType === 'evt' ? `evt{${addForm.toggleDot}}` : addForm.toggleType,
                delay: Number(addForm.delay),
                oequipNo: Number(addForm.linkEq),
                osetNo: addForm.linkOrder.setNo,
                value: addForm.value,
                procdesc: addForm.remark,
                enable: Boolean(addForm.enable)
            }
        },
        afterEach(res) {
            if (res.data.code === 200) {
                $message.success($t('linkSetting.tips.operationSuccess'))
            }
        }

    })

    const { execute: addIfLinking, loading: addIfLoading } = useAxios(LinkSettingService.IfLinkAdd, {
        async validate() {
            return linkInfoNew.value?.validate().then(() => {
                return true
            }).catch(() => {
                return false
            })
        },
        beforeEach() {
            return {
                delay: Number(addForm.delay),
                oequipNo: Number(addForm.linkEq),
                osetNo: addForm.linkOrder.setNo,
                value: addForm.value,
                procdesc: addForm.remark,
                enable: Boolean(addForm.enable),
                procName: String(addForm.procName),
                iConditionItems: handleData(addForm.ifLinkEquipList),
                conditionRelation: addForm.conditionRelation ? 1 : 0
            }
        },
        afterEach(res) {
            if (res.data.code === 200) {
                $message.success($t('linkSetting.tips.operationSuccess'))
            }
        }
    })

    function resetAdd() {
        for (const key in addForm) {
            addForm[key] = ''
            addForm['conditionRelation'] = false
            addForm['linkOrder'] = {
                setType: '',
                setNo: -1
            }
            addForm['enable'] = true;
            addForm['procName'] = '';
            addForm['ifLinkEquipList'] = [{
                iequipNm: '',
                iequipNo: 0,
                controlCommand: [],
                iYcYxItems: [{
                    iycyxType: null,
                    iycyxNo: null,
                    condition: 0,
                    iycyxValue: ''
                }]
            }];
        }
    }

    function addItem(){
        addForm.ifLinkEquipList.push({
            iequipNm: '',
            iequipNo: 0,
            controlCommand: [],
            iYcYxItems: [{
                iycyxType: null,
                iycyxNo: null,
                condition: 0,
                iycyxValue: ''
            }]
        })
    }
    function removeItem(index: number){
        if(addForm.ifLinkEquipList.length === 1){
            $message.warning($t('linkSetting.tips.minimumConfiguration'))
            return;
        }
        addForm.ifLinkEquipList.splice(index, 1)
    }

    function addChildrenItem(index: number){
        addForm.ifLinkEquipList[index].iYcYxItems.push({
            iycyxType: null,
            iycyxNo: null,
            condition: 0,
            iycyxValue: ''
        })
    }

    function removeChildrenItem(index: number, childrenIndex: number){
        if(addForm.ifLinkEquipList[index].iYcYxItems.length === 1){
            $message.warning($t('linkSetting.tips.minimumConfiguration'))
            return;
        }
        addForm.ifLinkEquipList[index].iYcYxItems.splice(childrenIndex, 1)
    }

    function handleData(arry: Array<any>){
        const obj = JSON.parse(JSON.stringify(arry));
        obj.forEach(item => {
            item?.iYcYxItems.forEach(childrenItem => {
                childrenItem.iycyxType = item?.controlCommand.find(filterItem => ((filterItem.ycyxNo + "-" + filterItem.ycyxType) === childrenItem.iycyxNo))?.ycyxType;
                childrenItem.iycyxNo = childrenItem.iycyxNo.replace(new RegExp('-.*$'), '')
            });
            delete item?.controlCommand;
            // delete item?.equipName;
        });
        return obj;
    }

    return {
        addForm, addRules, linkInfoNew, addLinking, addLoading, addIfLinking, addIfLoading, resetAdd, addItem, removeItem, addChildrenItem, removeChildrenItem
    }
}

/**
 * 编辑联动设置
 *
 * @export
 * @param {AddForm} addForm
 * @param {(Ref<ElFormInstance | undefined>)} linkInfoNew
 * @returns
 */
export function EditLinking(addForm: AddForm, linkInfoNew: Ref<ElFormInstance | undefined>) {
    const $message = useMessage()
    const $t = useI18n().t
    const { execute: editLinking, loading: editLoading } = useAxios(LinkSettingService.linkEdit, {
        async validate() {
            return linkInfoNew.value?.validate() || false
        },
        beforeEach() {
            return {
                id: addForm.id,
                iequipNo: Number(addForm.toggleEq),
                iycyxNo: addForm.toggleType === 'evt' ? 0 : Number(addForm.toggleDot),
                iycyxType: addForm.toggleType === 'evt' ? `evt{${addForm.toggleDot}}` : addForm.toggleType,
                delay: Number(addForm.delay),
                oequipNo: Number(addForm.linkEq),
                osetNo: addForm.linkOrder.setNo,
                value: addForm.value,
                procdesc: addForm.remark,
                enable: Boolean(addForm.enable),
                isConditionLink: Boolean(addForm.isConditionLink)
            }
        },
        afterEach(res) {
            if (res.data.code === 200) {
                $message.success($t('linkSetting.tips.operationSuccess'))
            }
        }
    })
    const { execute: ifEditLinking, loading: editIfLoading } = useAxios(LinkSettingService.IfLinkEdit, {
        async validate() {
            return linkInfoNew.value?.validate() || false
        },
        beforeEach() {
            return {
                id: addForm.id,
                delay: Number(addForm.delay),
                oequipNo: Number(addForm.linkEq),
                osetNo: addForm.linkOrder.setNo,
                value: addForm.value,
                procdesc: addForm.remark,
                enable: Boolean(addForm.enable),
                procName: String(addForm.procName),
                iConditionItems: handleData(addForm.ifLinkEquipList),
                isConditionLink: Boolean(addForm.isConditionLink),
                conditionRelation: addForm.conditionRelation ? 1 : 0
            }
        },
        afterEach(res) {
            if (res.data.code === 200) {
                $message.success($t('linkSetting.tips.operationSuccess'))
            }
        }
    })

    function handleData(arry: Array<any>){
        const obj = JSON.parse(JSON.stringify(arry));
        obj.forEach(item => {
            delete item?.controlCommand;
            delete item?.equipName;
            Array.isArray(item.iYcYxItems) && item.iYcYxItems.forEach(childrenItem => {
                childrenItem.iycyxNo = childrenItem.iycyxNo.replace(new RegExp('-.*$'), '')
            });
        });
        return obj;
    }

    return {
        editLoading, editIfLoading, editLinking, ifEditLinking
    }
}

