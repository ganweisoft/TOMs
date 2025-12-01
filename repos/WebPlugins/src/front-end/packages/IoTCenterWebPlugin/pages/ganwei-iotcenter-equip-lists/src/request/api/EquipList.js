const equipList = {
    // 获取当前设备遥测、遥信、设置数量
    getYcYxSetNumByEquipNo (data) {
        return this.get(`/IoT/api/v3/EquipList/GetYcYxSetNumByEquipNo?equipNo=` + data);
    },

    // 获取当前设备模拟量集合---支持分页
    getYcpByEquipNo (data) {
        return this.post('/IoT/api/v3/EquipList/GetYcpByEquipNo', data);
    },

    // 获取当前设备状态量集合——支持分页、搜索
    getYxpByEquipNo (data) {
        return this.post('/IoT/api/v3/EquipList/GetYxpByEquipNo', data);
    },

    // 设备列表设置指令下发
    getSetCommandBySetNo (data) {
        return this.post('/IoT/api/v3/EquipList/SetCommandBySetNo', data);
    },

    // 遥测历史曲线数据
    getEquipGetCurData (data) {
        return this.post('/IoT/api/v3/EquipList/GetYcpHistroyByTime', data);
    },

    // 遥信历史曲线数据
    getEquipGetYxpCurData (data) {
        return this.post('/IoT/api/v3/EquipList/GetYxpHistroyByTime', data);
    },

    exportAbnormalRecord (data) {
        return this.getExcelFile('/IoT/api/v3/EquipList/ExportAbnormalRecord?deviceStatus=' + data.deviceStatus);
    },

    // 导出历史曲线
    exportCurve (data) {
        return this.post('/IoT/api/v3/EquipList/ExportEquipHistroyCurves', data)
    },

    // 获取多设备下的测点
    getYcpsYxps (data) {
        return this.post('/IoT/api/v3/EquipList/GetExportEquipYcYxps', data)
    },

    // 获取一个设备的控制表
    getEquipControlTable (data) {
        return this.post('/IoT/api/v3/EquipList/GetSetParmByEquipNo', data);
    },

    // 导出设备
    exportEquip (data) {
        return this.getFile('/IoT/api/v3/SystemConfig/BatchExportEquip', data);
    },

    //获取有记录历史曲线的设备
    equipExistsCurveRcord () {
        return this.get(`/IoT/api/v3/EquipList/EquipExistsCurveRcord`)
    },

    //获取列列表
    getPageColumnListByPage (data) {
        return this.get(`/IoT/api/v3/PageColumnManage/GetPageColumnListByPage`,data)
    },
    // 调整列信息
    addPageColumn (data) {
        return this.post(`/IoT/api/v3/PageColumnManage/AddPageColumn`,data)
    },


}

export default equipList;
