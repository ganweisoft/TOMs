/**
 * @file 数据收集
 */

const DataCollection = {

    // 获取数据采集成功率
    getAcquisitionSuccessRate() {
        return this.get(
            '/IoT/api/v3/DataAcquisition/GetAcquisitionSuccessRate'
        );
    },

    // 获取实时采集列表
    getRealTimeAcquData() {
        return this.get('/IoT/api/v3/DataAcquisition/GetRealTimeAcquData');
    },

    // 设置数据采集周期
    setAcquisitionCycle(data) {
        return this.post(
            '/IoT/api/v3/DataAcquisition/SetAcquisitionCycle?CycleData=' + data
        );
    }
};
export default DataCollection;
