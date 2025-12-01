/**
 * 设备管理
 */

const SystemConfig = {

    // 获取设备分类列表
    getEquipClassifications (data) {
        return this.get(`/IoT/api/v3/SystemConfig/GetEquipClassifications?name=` + data);
    },

    // 添加或编辑设备分类
    getAddOrUpdateEquipClassification (data) {
        return this.post(`/IoT/api/v3/SystemConfig/AddOrUpdateEquipClassification`, data);
    },

    // 删除设备分类
    getDeleteEquipClassification (data) {
        return this.post(`/IoT/api/v3/SystemConfig/DeleteEquipClassification?id=` + data);
    },

    // 获取指定设备类型下设备列表
    getClassificationEquips (data) {

        // return this.get(`/IoT/api/v3/SystemConfig/GetClassificationEquips?id=${data.id}&name=${data.name}&pageNo=${data.pageNo}&pageSize=${data.pageSize}`);
        return this.get(`/IoT/api/v3/SystemConfig/GetClassificationEquips`, data);
    },

    getYcpData (data) {
        return this.post(`/IoT/api/v3/SystemConfig/GetYcpDataList`, data);
    },

    // 获取当前设备状态量集合——支持分页、搜索
    getYxpData (data) {
        return this.post(`/IoT/api/v3/SystemConfig/GetYxpDataList`, data);
    },

    // 获取设备的列表
    getEquipTree (params) {
        return this.post(`/IoT/api/v3/SystemConfig/GetEquipDataList`, params)
    },

    // 获取设备遥测配置信息
    getEquipYCPConf (params) {
        return this.post(`/IoT/api/v3/SystemConfig/GetYcpDataList`, params)
    },

    // 获取设备遥信(YX)配置信息
    getEquipYXPConf (params) {
        return this.post(`/IoT/api/v3/SystemConfig/GetYxpDataList`, params)
    },

    // 获取设置配置信息设备号
    getEquipSetParmConf (params) {
        return this.post(`/IoT/api/v3/SystemConfig/GetSetParmDataList`, params)
    },

    // 新增设备配置信息
    postAddEquip (data, groupId) {
        return this.post(`/IoT/api/v3/SystemConfig/AddEquipData?groupId=` + groupId, data)
    },

    // 新增设备遥测信息
    postAddYc (data) {
        return this.post(`/IoT/api/v3/SystemConfig/AddYcpData`, data)
    },

    // 新增设备遥信信息
    postAddYx (data) {
        return this.post(`/IoT/api/v3/SystemConfig/AddYxpData`, data)
    },

    // 新增设备设置信息
    postAddSet (data) {
        return this.post(`/IoT/api/v3/SystemConfig/AddSetData`, data)
    },

    // 修改设备配置信息
    postUpdateEquip (data) {
        return this.post(`/IoT/api/v3/SystemConfig/EditEquipData`, data)
    },

    // 修改设备遥测信息
    postUpdateYCPEquip (data) {
        return this.post(`/IoT/api/v3/SystemConfig/EditYcpData`, data)
    },

    // 修改设备遥信信息
    postUpdateYXPEquip (data) {
        return this.post(`/IoT/api/v3/SystemConfig/EditYxpData`, data)
    },

    // 修改设备设置信息
    postUpdateSetParmEquip (data) {
        return this.post(`/IoT/api/v3/SystemConfig/EditSetData`, data)
    },

    // 删除设备
    deleteEquip (params) {
        return this.post(`/IoT/api/v3/SystemConfig/DelEquipData`, params)
    },

    // 删除yc
    deleteYc (params) {
        return this.post(`/IoT/api/v3/SystemConfig/DelYcpData`, params)
    },

    // 删除yx
    deleteYx (params) {
        return this.post(`/IoT/api/v3/SystemConfig/DelYxpData`, params)
    },

    // 删除设置
    deleteSet (params) {
        return this.post(`/IoT/api/v3/SystemConfig/DelSetData`, params)
    },

    // 通过模板新增
    addEquipFromModel (data) {
        return this.post(`/IoT/api/v3/SystemConfig/AddEquipFromModel`, data)
    },

    // 设为模板
    setEquipToModel (data) {
        return this.postParam('/IoT/api/v3/SystemConfig/SetEquipToModel', data);
    },

    // 批量删除设备
    batchDelEquip (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchDelEquip', data);
    },

    // 批量修改设备
    batchEditEquip (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchModifyEquipParam', data);
    },

    // 批量修改遥测
    batchEditYc (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchModifyYcp', data);
    },

    // 批量修改遥信
    batchEditYx (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchModifyYxp', data);
    },

    // 批量修改设置
    batchEditSet (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchModifyEquipSetting', data);
    },

    // 导出设备
    exportEquip (data) {
        return this.getFile('/IoT/api/v3/SystemConfig/BatchExportEquip', data);
    },

    // 导入设备
    importEquip (data) {
        return this.getFile('/IoT/api/v3/SystemConfig/BatchImportTemplate', data);
    },

    // 获取动态库列表
    getCommunicationDrv () {
        return this.get('/IoT/api/v3/SystemConfig/GetCommunicationDrv');
    },

    // 应用设备信息到其他设备
    BatchModifyFromEquip (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchModifyFromEquip', data);
    },
    GroupListNoState(data) {
        return this.post('/IoT/api/v3/BA/GroupListNoState', data)
    }
}

export default SystemConfig;
