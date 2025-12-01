/**
 * 实时快照
 */

const RealTime = {

    // 全局消息提示
    getRealTimeData (data) {
        return this.get('/IoT/api/v3/RealTime/GetRealTimeData', data);
    },

    // 获取事件的报警配置
    getRealTimeEventTypeConfig () {
        return this.get('/IoT/api/v3/RealTime/GetRealTimeEventTypeConfig');
    },

    // 获取实时快照事件总数
    getRealTimeEventCount (data) {
        return this.get('/IoT/api/v3/RealTime/GetRealTimeEventCount?types=' + data + '&times=' + new Date());
    },

    // 获取当前系统报警实时事件
    getRealTimeEvent (data) {
        return this.post('/IoT/api/v3/RealTime/GetRealTimeEvent', data);
    },

    // 确认实时事件状态
    getConfirmedRealTimeEvent (data) {
        return this.post('/IoT/api/v3/RealTime/ConfirmedRealTimeEvent', data);
    },

    // 测试
    getAAA (data) {
        return this.post('/IoT/api/v3/RealTime/GetConfirmedRealTimeEvent', data);
    },

    // 导出实时快照
    exportRealTimeRecord (data) {
        return this.postBlob('/IoT/api/v3/RealTime/BatchExportRealTime?types=[' + data + ']');
    }
}

export default RealTime;