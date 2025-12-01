const equipInfo = {
    // 新增分组
    addGroupEquipList (data) {
        return this.post('/IoT/api/v3/BA/AddGroup', data);
    },

    // 删除一个分组
    deleteGroup (data) {
        return this.post('/IoT/api/v3/BA/DeleteGroup', data);
    },

    // 重命名分组
    getEquipReName (data) {
        return this.get('/IoT/api/v3/BA/ReNameGroup', data);
    },

    // 批量移动设备
    moveToNewGroup (data) {
        return this.post('/IoT/api/v3/BA/BatchMoveEquip', data)
    },

    // 获取当前设备遥测、遥信、设置数量
    getYcYxSetNumByEquipNo (data) {
        return this.get(`/IoT/api/v3/EquipList/GetYcYxSetNumByEquipNo?equipNo=` + data);
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

    // 应用设备信息到其他设备
    BatchModifyFromEquip (data) {
        return this.post('/IoT/api/v3/SystemConfig/BatchModifyFromEquip', data);
    },
    getTemplateList (data) {
        return this.get('/IoT/api/v3/ModelConfig/all-equip-simple', data)
    },

    // 批量编辑是，获取自定义参数
    getCustomProp (data) {
        return this.post('/IoT/api/v3/SystemConfig/GetCustomProp', data);
    }
}

export default equipInfo
