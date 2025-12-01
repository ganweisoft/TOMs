/**
 * 边缘物联接口
 */

 const EquipPlatform = {
    // 设备注册中心
    getEquipList(data) {
        return this.post('/api/south/device/page', data);
    },

    batchRegistry(data) {
        return this.post('/api/south/device/batchregistry', data);
    },

    batchDownLine(data) {
        return this.post('/api/south/device/batchdownline', data);
    },
    getGrouplist() {
        return this.get('/api/south/device/getegrouplist');
    },
    binding(data) {
        return this.post('/api/south/device/binding', data);
    },
    // 设备上报
    getReportList(data) {
        return this.post('/api/south/platform/page', data);
    },
    create(data) {
        return this.post('/api/south/platform/create', data);
    },
    deleteReport(data) {
        return this.post('/api/south/platform/delete?platformId=' + data);
    },
    modify(data) {
        return this.post('/api/south/platform/modify', data);
    },
    getRecordsOfPlatform(data) {
        return this.get('/api/south/platform/records', data);
    },
    getRecordsOfDevice(data) {
        return this.get('/api/south/device/records', data);
    },
    // 下拉数据
    resources(data) {
        return this.get('/api/south/data/resources', data);
    },
    products(data) {
        return this.get('/api/south/data/products', data);
    },
    platforms() {
        return this.get('/api/south/data/platforms');
    },
    reportRules() {
        return this.get('/api/south/data/reportRules');
    },
    reportTypes() {
        return this.get('/api/south/data/reportTypes');
    },
    protocolTypes() {
        return this.get('/api/south/data/protocolTypes');
    },
    getRegisterStatus() {
        return this.get('/api/south/data/registerStatus');
    },
    getDeviceStatus() {
        return this.get('/api/south/data/deviceStatus');
    }
};

export default EquipPlatform;
