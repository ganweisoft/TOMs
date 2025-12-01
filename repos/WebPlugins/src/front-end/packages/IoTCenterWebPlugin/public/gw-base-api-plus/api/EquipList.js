/**
 * 设备列表
 */

const EquipList = {

    // 获取中英文版本
    getLanguageLogo (data) {
        return this.get(`/IoT/api/v3/Auth/GetLanguageLogo`, data);
    },

    // 获取所有设备的节点列表---支持分页、搜索
    getEquipListByPage (data) {
        return this.post(`/IoT/api/v3/EquipList/GetEquipListByPage`, data);
    },

    // 获取所有设备状态---支持分页
    // 获取所有设备的节点列表
    // getEquipLists(data) {
    //     return this.post(`/IoT/api/v3/EquipList/GetEquipListByPage`, data);
    // },
    // 获取当前设备遥测、遥信、设置数量
    getYcYxSetNumByEquipNo (data) {
        return this.get(`/IoT/api/v3/EquipList/GetYcYxSetNumByEquipNo?equipNo=` + data);
    },

    // 获取所有设备状态
    getEquipListStateByPage (data) {
        return this.post(`/IoT/api/v3/EquipList/GetEquipListStateByPage`, data);
    },

    // 下拉列表
    // 触发设备项
    togEqDrop (data) {
        return this.post('/IoT/api/v3/EquipList/GetEquipNoAndName', data);
    },

    // 联动设备项
    linkEqDrop (data) {
        return this.post('/IoT/api/v3/EquipList/GetSetEquip', data);
    },

    // 设备控制项
    evtEqControl (data) {
        return this.post('/IoT/api/v3/EquipList/GetEquipSetParmList', data);
    },

    // 设备控制项--树状结构
    GetEquipSetParmTreeList (data) {
        return this.post('/IoT/api/v3/EquipList/GetEquipSetParmTreeList', data);
    },

    // 获取可控设备列表
    getRealEquipSetParmList (data) {
        return this.post('/IoT/api/v3/EquipList/GetRealEquipSetParmList', data);
    },

    // 获取可看设备列表
    getRealEquipListByPage (data) {
        return this.post('/IoT/api/v3/EquipList/GetRealEquipListByPage', data);
    },

    // 设备列表模块
    // 指定设备和视频Id获取视频信息
    getEquipNumCatVideoId (data) {
        return this.get('/IoT/api/v3/EquipList/GetVideoInfo?equipNumCatVideoId=' + data);
    },

    // 获取设备的遥信状态-支持分页搜索
    getYxpItemStateByPage (data) {
        return this.post('/IoT/api/v3/EquipList/GetYxpItemStateByPage', data);
    },

    // 获取当前设备模拟量集合---支持分页
    getYcpByEquipNo (data) {
        return this.post('/IoT/api/v3/EquipList/GetYcpByEquipNo', data);
    },

    // 获取当前设备状态量集合——支持分页、搜索
    getYxpByEquipNo (data) {
        return this.post('/IoT/api/v3/EquipList/GetYxpByEquipNo', data);
    },

    // 获取一个设备对应的控制选项
    getSetParm (data) {
        return this.post('/IoT/api/v3/EquipList/GetSetParmByEquipNo', data);
    },

    // 获取当前设备单个遥测点的实时状态
    getEquipYcpState (data) {
        return this.post('/IoT/api/v3/EquipList/GetEquipYcpState', data);
    },

    // 获取历史曲线
    getYcpHistroyChartByTime (data) {
        return this.post('/IoT/api/v3/EquipList/GetYcpHistroyChartByTime', data);
    },

    // 设备列表设置指令下发
    getSetCommandBySetNo (data) {
        return this.post('/IoT/api/v3/EquipList/SetCommandBySetNo', data);
    },

    // 曲线数据
    getEquipGetCurData (data) {
        return this.post('/IoT/api/v3/EquipList/GetYcpHistroyByTime', data);

        // return this.post('/IoT/api/v3/BA/EquipCurData', data);
    },

    exportAbnormalRecord (data) {
        return this.getExcelFile('/IoT/api/v3/EquipList/ExportAbnormalRecord?deviceStatus=' + data.deviceStatus);
    },

    // 导出历史曲线
    exportCurve (data) {
        return this.post('/IoT/api/v3/EquipList/ExportEquipHistroyCurves', data)
    },

    // 模板下载
    DownloadTemplateFile () {
        return this.getExcelFile('/IoT/api/v3/ExcelTemplate/Download?templateName=模板管理');
    }

}

export default EquipList;