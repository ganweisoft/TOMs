/**
 * 事件查询
 */

const Event = {

    // 事件查询
    // 获取设备事件列表
    evtList (data) {
        return this.post('/IoT/api/v3/Event/GetEquipEvtByPage', data);
    },

    evtCounts (data) {
        return this.post('/IoT/api/v3/Event/GetEquipEvtCounts', data);
    },

    // 获取系统事件列表
    evtSysList (data) {
        return this.get('/IoT/api/v3/Event/GetSysEvtByPage', data);
    },

    // 系统数据查询
    // 获取数据表
    getTabels () {
        return this.get('/IoT/api/v3/Event/GetHistorySubTables')
    },
    getColumns (data) {
        return this.get('/IoT/api/v3/Event/GetHistorySubColumns', data)
    },
    getSearchResult (data) {
        return this.post('/IoT/api/v3/Event/GetHistorySubs', data)
    },
    exportTableData (data) {
        return this.getFile('/IoT/api/v3/Event/BatchExportHistorySubs', JSON.stringify(data))
    },
    deleteQueryCache (data) {
        return this.post('/IoT/api/v3/Event/DeleteQuery', data)
    }
}

export default Event;