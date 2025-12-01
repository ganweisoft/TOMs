import { ref } from "vue";

import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios";

import { LinkSettingService } from "@/request/api";
import { ISceneLinkSettingData } from "@/request/models";

export const UUID = () => Math.random().toString(36).substr(3, 10)

export type SceneRepositoryData = {
    sceneName: string,
    sceneNo: number,
    sum: number,
    itemList: Array<ISceneLinkSettingData>,
    equipNo: number,
}

export default function () {
    const keyword = ref('')
    const { loading: listLoad, page, data: sceneRepository, execute: getSceneList } = useAxios(LinkSettingService.getSceneList, {
        initialData: [],
        immediate: true,
        isPage: true,
        pageChangeAutoRequest: true,
        beforeEach(context) {
            return {
                searchName: keyword.value,
                pageNo: context.page.pageNo,
                pageSize: context.page.pageSize
            }
        },
        afterEach(res) {
            page.total = res.data.data?.totalCount || 0
            if (res.data.data?.rows) {
                const data = res.data.data.rows || []
                return data.map<SceneRepositoryData>(item => ({
                    sceneName: item.setNm,
                    sceneNo: item.setNo,
                    sum: item.list.length,
                    itemList: item.list.map(linkItem => {
                        linkItem.id = UUID();
                        return linkItem
                    }),
                    equipNo: item.equipNo
                }))
            }
            return []
        }
    })
    return {
        sceneRepository, getSceneList, keyword, listLoad, page
    }
}
