const templateManage = {
    // 模板下载
    DownloadTemplateFile () {
        return this.getExcelFile('/IoT/api/v3/ExcelTemplate/Download?templateName=模板管理');
    },

    uploadTemplate (data) {
        return this.postFile('/IoT/api/v3/SystemConfig/BatchImportTemplate', data)
    },

    downloadProductList (data) {
        return this.postBlob('/IoT/api/v3/SystemConfig/BatchExportEquipTemplate', data)
    },

    // 获取模板设备遥测配置信息
    getModelEquipYCPConf (params) {
        return this.post(`/IoT/api/v3/ModelConfig/GetYcpDataList`, params)
    },

    // 获取模板设备遥信(YX)配置信息
    getModelEquipYXPConf (params) {
        return this.post(`/IoT/api/v3/ModelConfig/GetYxpDataList`, params)
    },

    // 获取模板设备设置配置信息
    getModelEquipSetParmConf (params) {
        return this.post(`/IoT/api/v3/ModelConfig/GetSetParmDataList`, params)
    },

    // 修改模板设备配置信息
    postAddModelEquip (data) {
        return this.post(`/IoT/api/v3/ModelConfig/AddEquipData`, data)
    },

    // 修改模板遥测配置信息
    postAddModelYc (data) {
        return this.post(`/IoT/api/v3/ModelConfig/AddYcpData`, data)
    },

    // 修改模板遥信配置信息
    postAddModelYx (data) {
        return this.post(`/IoT/api/v3/ModelConfig/AddYxpData`, data)
    },

    // 修改模板设置配置信息
    postAddModelSet (data) {
        return this.post(`/IoT/api/v3/ModelConfig/AddSetData`, data)
    },

    // 修改模板设备配置信息
    postUpdateModelEquip (data) {
        return this.post(`/IoT/api/v3/ModelConfig/EditEquipData`, data)
    },

    // 修改模板设备遥测信息
    postUpdateModelYCPEquip (data) {
        return this.post(`/IoT/api/v3/ModelConfig/EditYcpData`, data)
    },

    // 修改模板设备遥信信息
    postUpdateModelYXPEquip (data) {
        return this.post(`/IoT/api/v3/ModelConfig/EditYxpData`, data)
    },

    // 修改模板设备设置信息
    postUpdateModelSetParmEquip (data) {
        return this.post(`/IoT/api/v3/ModelConfig/EditSetData`, data)
    },

    // 删除模板设备
    deleteModelEquip (params) {
        return this.post(`/IoT/api/v3/ModelConfig/DelEquipData`, params)
    },

    // 删除模板yc
    deleteModelYc (params) {
        return this.post(`/IoT/api/v3/ModelConfig/DelYcpData`, params)
    },

    // 删除模板yx
    deleteModelYx (params) {
        return this.post(`/IoT/api/v3/ModelConfig/DelYxpData`, params)
    },

    // 删除模板设置
    deleteModelSet (params) {
        return this.post(`/IoT/api/v3/ModelConfig/DelSetData`, params)
    },
    // 获取产品属性
    getProductProperty (params) {
        return this.get(`/IoT/api/v3/ProductProperty/GetProductPropertyFromProductId`, params)
    },


    // 设置产品属性
    setProductProperty (params) {
        return this.post(`/IoT/api/v3/ProductProperty/AddProductProperty`, params)
    },

    // 修改产品属性
    updatedProductProperty (params) {
        return this.post(`/IoT/api/v3/ProductProperty/UpdateProductProperty`, params)
    },

    getModelYcYxSetNum (data) {
        return this.get('/IoT/api/v3/ModelConfig/ycyxset-num', data)
    },

    copyEquip (data) {
        return this.post(`/IoT/api/v3/ModelConfig/copy?equipNo=${data.equipNo}`)
    },
    copyYcp (data) {
        return this.post(`/IoT/api/v3/ModelConfig/ycp-copy?no=${data.no}&equipNo=${data.equipNo}`)
    },
    copyYxp (data) {
        return this.post(`/IoT/api/v3/ModelConfig/yxp-copy?no=${data.no}&equipNo=${data.equipNo}`)
    },
    copySetparm (data) {
        return this.post(`/IoT/api/v3/ModelConfig/setparm-copy?no=${data.no}&equipNo=${data.equipNo}`)
    }
}

export default templateManage
