import { computed, reactive, ref } from "vue"
import { useI18n } from 'vue-i18n';
import { ElNotification } from 'element-plus';

import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios";

import { LinkSettingService } from "@/request/api";
import { IEditSceneLinkData } from "@/request/models";

import EditSceneSetting from './EditSceneSetting';
import { SceneRepositoryData } from "./GetSceneSettings";
import { useEquipControlForm, XhForm } from './useSettingPanel';
export default function (refresh: () => void) {
    const { nameFormRef, xhEqpFormRef, xhTimeFormRef, cycleTaskContentRef, xhForm, xhRule, deleteXhFormCycleTaskContent, reset } = useEquipControlForm()
    const addVisible = ref(false)
    const isEdit = ref(false)
    const loading = ref(false)
    const $t = useI18n().t;
    const validaters = () => {
        if (nameFormRef.value?.validate && cycleTaskContentRef.value?.validate()) {
            return Promise.all([nameFormRef.value.validate(), cycleTaskContentRef.value.validate()]).then(() => {
                return true
            }).catch((e) => {
                console.log(e);
                return false
            })
        }
        return Promise.resolve(false)
    }

    const { setEditForm, editScene } = EditSceneSetting(xhForm, validaters)

    const { addScene } = AddScene(xhForm, validaters)

    const showAdd = () => {
        addVisible.value = true
        isEdit.value = false
    }
    const showEdit = (row: SceneRepositoryData) => {
        addVisible.value = true
        isEdit.value = true
        setEditForm(row)
    }

    const closeAdd = () => {
        addVisible.value = false
        reset()
    }

    const confirmAddorEdit = () => {
        let promise: Promise<any>;
        if (isEdit.value) {
            promise = editScene()
        } else {
            promise = addScene()
        }
        return promise.then(() => {
            ElNotification({
                title: $t('sceneSetting.tips.delTitle1'),
                message: $t('sceneSetting.tips.operationSuccess'),
                type: 'success',
              })
            loading.value = false
            closeAdd()
            refresh()
        })
    }

    return {
        nameFormRef, xhEqpFormRef, xhTimeFormRef, cycleTaskContentRef, xhForm, xhRule, deleteXhFormCycleTaskContent,
        addVisible, showAdd, showEdit, closeAdd, confirmAddorEdit, loading, isEdit
    }
}

function AddScene(xhForm: XhForm, validaters: () => Promise<boolean>) {
    const { execute: addScene } = useAxios(LinkSettingService.addScene, {
        validate() {
            return validaters()
        },
        beforeEach() {
            return {
                setNm: xhForm.name,
                list: xhForm.cycleTaskContent.map<IEditSceneLinkData>(item => {
                    if (item.type === 'E') {
                        return {
                            id: item.id,
                            sceneType: 'E',
                            equipNm: item.equipName,
                            equipNo: item.equipNo,
                            setType: item.setType,
                            setNo: item.setNo,
                            setNm: item.setNm,
                            value: item.value,
                        }
                    }
                    return {
                        id: item.id,
                        sceneType: 'T',
                        timeValue: item.conTime
                    }
                })
            }
        }
    })
    return {
        addScene
    }
}
