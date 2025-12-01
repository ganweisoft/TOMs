/**
 * @file 数据备份
 */

const DataBackup = {

    // 获取数据备份列表
    getBackupList(data) {
        return this.post('/IoT/api/v3/DataBackup/GetBackupList', data);
    },

    // 备份
    backup() {
        return this.post('/IoT/api/v3/DataBackup/Backup');
    }
};

export default DataBackup;