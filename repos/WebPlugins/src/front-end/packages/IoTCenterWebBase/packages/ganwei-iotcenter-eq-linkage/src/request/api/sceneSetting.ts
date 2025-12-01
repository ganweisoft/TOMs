/**
 * 设备管理
 */

import BaseService, { PageResponse } from "@components/@ganwei-pc/gw-base-api-plus/apiFunction";

class SceneSettingService extends BaseService {
    static linkEqDrop(data) {
        return BaseService.post('/IoT/api/v3/EquipList/GetSetEquip', data)
    }

    // 获取当前设备模拟量集合---支持分页
    static getYcpByEquipNo(data) {
        return BaseService.post('/IoT/api/v3/EquipList/GetYcpByEquipNo', data)
    }

    // 获取当前设备状态量集合——支持分页、搜索
    static getYxpByEquipNo(data) {
        return BaseService.post('/IoT/api/v3/EquipList/GetYxpByEquipNo', data)
    }

    // 获取一个设备对应的控制选项
    static getSetParm(data) {
        return BaseService.post('/IoT/api/v3/EquipList/GetFullSetParmByEquipNo?equipNo=' + data?.equipNo, data)
    }
}
export default SceneSettingService;
