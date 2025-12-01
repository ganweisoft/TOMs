const commonApi = {
    // 获取资产列表
    getZiChan(data) {
        return this.get(`/IoT/api/v3/CommonTable/GetZiChanTableData`, data)
    },
    // 获取预案列表
    getPlanList(data) {
        return this.post('/IoT/api/v3/ReservePlan/GetPlanList', data);
    },

    // 获取声音文件列表
    getwaveFilenames() {
        return this.get('/IoT/api/v3/excelTemplate/getwaveFilenames')
    },

    // 获取模板设备的列表
    getModelEquipTree(params) {
        return this.post(`/IoT/api/v3/ModelConfig/GetAllEquipDataList`, params)
    },

    // 获取所有设备配置信息
    getAllEquipSimple(params) {
        return this.get(`/IoT/api/v3/ModelConfig/all-equip-simple`, params)
    },
    // 查询产品下设备的操作命令及操作命令参数
    getBatchConfigEquip(params) {
        return this.get(`/IoT/api/v3/ModelBatchConfig/QueryBatchConfigEquip`, params)
    },

    // 获取当前设备遥测、遥信、设置数量
    getYcYxSetNumByEquipNo(data) {
        return this.get(`/IoT/api/v3/EquipList/GetYcYxSetNumByEquipNo?equipNo=` + data);
    },

    // 保存关联设备
    saveBatchConfigEquip(params) {
        return this.post(`/IoT/api/v3/ModelBatchConfig/SaveBatchConfigEquip`, params)
    },

    // 导出更新的设备
    exportBatchConfigEquip(params) {
        return this.getFile(`/IoT/api/v3/ModelBatchConfig/ExportBatchConfigEquip`, params)
    },

    // 导入更新的设备
    importBatchConfigEquip(params) {
        return this.postFile(`/IoT/api/v3/ModelBatchConfig/ImportBatchConfigEquip`, params)
    },
    // 获取动态库列表
    getCommunicationDrv() {
        return this.get('/IoT/api/v3/SystemConfig/GetCommunicationDrv');
    },

    // 获取所有视频信息
    getVideoAllInfo(data) {
        return this.get(`/IoT/api/v3/realtime/getvideoInfos?equipNo=${data.equip_no}`);
    },

    //  获取关联设备-下拉类别
    getSubsystemTypeEquips(params) {
        return this.get(`/IoT/api/v3/ModelBatchConfig/GetSubsystemTypeEquips`, params)
    },

    //  关联设备-查询产品列表、产品信息、设备列表
    calSubsystemTypeEquip(params) {
        return this.post(`/IoT/api/v3/ModelBatchConfig/CalSubsystemTypeEquip`, params)
    },

    //  关联设备-保存产品列表、产品信息、设备列表
    saveSubsystemTypeEquip(params) {
        return this.post(`/IoT/api/v3/ModelBatchConfig/SaveSubsystemTypeEquip`, params)
    },


}

export default commonApi;