import { ref } from "vue";

import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios";

import { LinkSettingService } from "@/request/api";
import { IEditSceneLinkData } from "@/request/models";

import { SceneRepositoryData } from "./GetSceneSettings";
import { CycleTaskContent, XhForm } from "./useSettingPanel";

export default function (xhForm: XhForm, validaters: () => Promise<boolean>) {
    const currentRow = ref<SceneRepositoryData>()
    const { execute: editScene } = useAxios(LinkSettingService.editScene, {
        validate: async () => {
            return validaters()
        },
        beforeEach() {
            if (!currentRow.value) {
                throw new Error('currentRow is undefined')
            }
            return {
                setNm: xhForm.name,
                setNo: currentRow.value.sceneNo,
                equipNo: currentRow.value.equipNo,
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

    function setEditForm(row: SceneRepositoryData) {
        currentRow.value = row
        xhForm.name = row.sceneName
        xhForm.cycleTaskContent = row.itemList.map<CycleTaskContent>(item => {
            if (item.sceneType === 'E') {
                return {
                    type: 'E',
                    id: item.id,
                    equipNo: item.equipNo,
                    setNo: item.setNo,
                    value: item.value,
                    equipName: item.equipNm,
                    setNm: item.setNm,
                    setType: item.setType,
                    attribute: item.value ? 'yc' : 'yx',
                    icon: 'iconfont icon-gw-icon-yijimenu-xitongyunwei',
                    color: '#3875FF'
                }
            }
            return {
                type: 'T',
                id: item.id,
                icon: 'iconfont icon-gw-icon-menu-dingshibaobiao',
                color: '#2EBFDF',
                conTime: item.timeValue
            }
        }) || []
    }
    return {
        setEditForm, editScene
    }
}
