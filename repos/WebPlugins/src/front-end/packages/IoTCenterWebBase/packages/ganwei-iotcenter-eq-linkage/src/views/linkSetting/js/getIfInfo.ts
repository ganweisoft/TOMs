
import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios"

import { LinkSettingService } from "@/request/api"
import { ISGetConditionLinkByAutoProcIdResponse } from "@/request/models"

import { EquipSelectChangeHook } from "./useEquipSelect"

export function IfGetConditionLinkByAutoProcId() {
    const { execute } = useAxios(LinkSettingService.IfGetConditionLinkByAutoProcId, {
        afterEach(res) {
            if (res.data.data) {
                return res.data.data || []
            }
            return []
        },
    })
    const ifGetConditionLinkByAutoProcId: EquipSelectChangeHook<Promise<ISGetConditionLinkByAutoProcIdResponse[]>> = (val) => {
        return execute({ AutoProcId: val })
    }
    return {
        ifGetConditionLinkByAutoProcId
    }

}
