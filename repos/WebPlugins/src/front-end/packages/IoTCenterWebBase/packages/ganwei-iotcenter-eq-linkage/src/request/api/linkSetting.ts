
/**
 * 定时报表
 */
import BaseService, { PageResponse } from "@components/@ganwei-pc/gw-base-api-plus/apiFunction";

import { IAddEquipLinkRequest, IAddSceneLinkRequest, IDeleteSceneLinkRequest, IEditSceneLinkRequest, IEquipCommandsRequest, IEquipCommandsResponse, IEquipLinkListRequest, IEquipLinkListResponse, IEquipListResponse, IEquipNo, IEquipYcYxSetNumResponse, ISceneLinkRequest, ISceneLinkResponse, ISetParamResponse, IYcpResponse, IYxpResponse } from "../models";

class LinkSettingService extends BaseService {
    static getYcYxSetNumByEquipNo(data: IEquipNo) {
        return BaseService.get<IEquipNo, IEquipYcYxSetNumResponse>(`/IoT/api/v3/EquipList/GetYcYxSetNumByEquipNo`, data)
    }

    static getYcpByEquipNo(data: IEquipNo) {
        return BaseService.post<IEquipNo, PageResponse<IYcpResponse>>('/IoT/api/v3/EquipList/GetYcpByEquipNo', data)
    }

    static getYxpByEquipNo(data: IEquipNo) {
        return BaseService.post<IEquipNo, PageResponse<IYxpResponse>>('/IoT/api/v3/EquipList/GetYxpByEquipNo', data)
    }

    // 获取一个设备对应的控制选项
    static getSetParm(data: IEquipNo) {
        return BaseService.post<any, PageResponse<ISetParamResponse>>('/IoT/api/v3/EquipList/GetFullSetParmByEquipNo?equipNo=' + data?.equipNo, data)
    }

    // 删除一条记录
    static linkDel(data) {
        return BaseService.postUrl('/IoT/api/v3/EquipLink/DelEquipLinkData', data)
    }

    // 删除一条记录
    static IfLinkDel(data) {
        return BaseService.post('/IoT/api/v3/ConLinkage/DelConditionLinkData', data)
    }

    // 获取筛选项
    static linkGetChoice() {
        return BaseService.post<object, IEquipListResponse>('/IoT/api/v3/EquipLink/GetIEquipAndOEquiepList')
    }

    // 获取联动列表/IoT/api/v3/EquipLink/GetEquipLinkListByPage
    static linkGetList(data: IEquipLinkListRequest) {
        return BaseService.get<IEquipLinkListRequest, PageResponse<IEquipLinkListResponse>>('/IoT/api/v3/ConLinkage/GetConditionLinkListByPage', data)
    }

    // 添加联动设备
    static linkAdd(data: IAddEquipLinkRequest) {
        return BaseService.post('/IoT/api/v3/EquipLink/AddEquipLinkData', data)
    }

    // 添加条件联动配置
    static IfLinkAdd(data) {
        return BaseService.post('/IoT/api/v3/ConLinkage/AddConditionLinkData', data)
    }

    // 编辑联动设备
    static linkEdit(data) {
        return BaseService.post(`/IoT/api/v3/EquipLink/EditEquipLinkData`, data)
    }
   
    // 编辑条件联动配置
    static IfLinkEdit(data) {
        return BaseService.post(`/IoT/api/v3/ConLinkage/EditConditionLinkData`, data)
    }

    // 获取要测遥信集合
    static IfGetEquipYcYxps(data) {
        return BaseService.post(`/IoT/api/v3/ConLinkage/GetEquipYcYxps`, data)
    }

    // 获取条件联动信息
    static IfGetConditionLinkByAutoProcId(data) {
        return BaseService.get(`/IoT/api/v3/ConLinkage/GetConditionLinkByAutoProcId`, data)
    }

    // 获取场景列表
    static getSceneList(data: ISceneLinkRequest) {
        return BaseService.post<any, ISceneLinkResponse>('/IoT/api/v3/EquipLink/GetSceneListByPage', data)
    }

    // 新增场景
    static addScene(data: IAddSceneLinkRequest) {
        return BaseService.post('/IoT/api/v3/EquipLink/AddSceneLinkData', data)
    }

    // 修改场景
    static editScene(data: IEditSceneLinkRequest) {
        return BaseService.post('/IoT/api/v3/EquipLink/EditSceneLinkData', data)
    }

    // 删除场景
    static delScene(data: IDeleteSceneLinkRequest) {
        return BaseService.post('/IoT/api/v3/EquipLink/DelSceneLinkData', data)
    }

    // 编辑联动设备命令
    static GetSetParmByEquipCommand(data: IEquipCommandsRequest) {
        return BaseService.get<IEquipCommandsRequest, Array<IEquipCommandsResponse>>('/IoT/api/v3/EquipList/GetSetParmByEquipNos', data)
    }
}

export default LinkSettingService;
