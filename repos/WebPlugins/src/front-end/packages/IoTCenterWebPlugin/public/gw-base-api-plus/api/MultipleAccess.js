/**
 * @file 多方式接入：插件管理
 */
const MultipleAccess = {

    // 获取插件
    getGWAssemblyByPage(data) {
        return this.post('/IoT/api/v3/GWAssembly/GetAllGWAssembly', data);
    },

    // 删除插件
    delGWAssembly(data) {

        return this.postUrl('/IoT/api/v3/GWAssembly/DelGWAssembly', data);
    },

    // 编辑插件
    editGWAssembly(data) {
        return this.post('/IoT/api/v3/GWAssembly/EditGWAssembly', data);
    },

    // 上传插件
    addGWAssembly(data) {
        return this.postFile('/IoT/api/v3/GWAssembly/AddGWAssembly', data);
    },

    // 改变启用状态
    enableGWAssembly(data) {
        return this.post('/IoT/api/v3/GWAssembly/EnableGWAssembly', data);
    },

    // 获取安全等级
    getSafeLevel() {
        return this.get('/IoT/api/v3/GWAssembly/GetSafeLevel');
    },

    // 设置安全等级
    setSafeLevel(level) {
        return this.post('/IoT/api/v3/GWAssembly/SetSafeLevel?level=' + level);
    }
};
export default MultipleAccess;
