import { Ref } from "vue"

import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios"

import { LinkSettingService } from "@/request/api"
import { ISetParamResponse } from "@/request/models"

import { EquipSelectChangeHook } from "./useEquipSelect"

/**
 * 获取设备遥测 遥信 设置命令
 *
 * @export
 * @returns
 */
export function GetYcYxSetNumByEquipNo() {
    const { data: ycYxSetNum, execute } = useAxios(LinkSettingService.getYcYxSetNumByEquipNo, {
        afterEach(res, BeforeEachContext) {
            if (res.data.data) {
                return res.data.data
            }
            return {
                ycNum: 0,
                yxNum: 0,
                setNum: 0
            }
        },
    })
    const getYcYxSetNumByEquipNo: EquipSelectChangeHook = (val) => {
        return execute({ equipNo: val[0].equipNo })
    }
    return {
        ycYxSetNum, getYcYxSetNumByEquipNo
    }
}

export function GetYcpByEquipNo(equipNo: Ref<number>) {
    const { data: ycp, execute: getYcpByEquipNo } = useAxios(LinkSettingService.getYcpByEquipNo, {
        beforeEach() {
            return {
                equipNo: equipNo.value,
                pageNo: 1,
                pageSize: 1000
            }
        },
        afterEach(res, BeforeEachContext) {
            if (res.data.data?.rows) {
                return res.data.data.rows
            }
            return []
        },
    })
    return {
        ycp, getYcpByEquipNo
    }
}

export function GetYxpByEquipNo(equipNo: Ref<number>) {
    const { data: yxp, execute: getYxpByEquipNo } = useAxios(LinkSettingService.getYxpByEquipNo, {
        beforeEach() {
            return {
                equipNo: equipNo.value,
                pageNo: 1,
                pageSize: 1000
            }
        },
        afterEach(res, BeforeEachContext) {
            if (res.data.data?.rows) {
                return res.data.data.rows
            }
            return []
        },
    })
    return {
        yxp, getYxpByEquipNo
    }
}

export function GetSetParmByEquipNo() {
    const { data: setParams, execute } = useAxios(LinkSettingService.getSetParm, {
        afterEach(res) {
            if (res.data.data?.rows) {
                return res.data.data.rows || []
            }
            return []
        },
    })
    const getSetParmByEquipNo: EquipSelectChangeHook<Promise<ISetParamResponse[]>> = (val) => {
        return execute({ equipNo: val[0].equipNo })
    }
    return {
        setParams, getSetParmByEquipNo
    }
}

export function IfGetEquipYcYxps() {
    const { execute } = useAxios(LinkSettingService.IfGetEquipYcYxps, {
        afterEach(res) {
            if (res.data.data?.items) {
                return res.data.data.items || []
            }
            return []
        },
    })
    const ifGetEquipYcYxps: EquipSelectChangeHook<Promise<ISetParamResponse[]>> = (val) => {
        return execute({ equipNos: [val] })
    }
    return {
        ifGetEquipYcYxps
    }
}
