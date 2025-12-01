/**
 * 通用配置管理
 */
const url = '';
const ConfigManage = {

    // 获取通用配置
    getIotConfig (data) {
        return this.post('/api/IotConfig/GetIotConfig', data);
    },

    // 更新通用配置
    updateIotConfig (data) {
        return this.post('/api/IotConfig/UpdateIotConfig', data);
    },

    // 删除通用配置
    deleteIotConfig (data) {
        return this.post('/api/IotConfig/DeleteIotConfig', data);
    },

    // 添加通用配置
    createIotConfig (data) {
        return this.post('/api/IotConfig/CreateIotConfig', data);
    },

    // 获取web配置
    getWebConfig () {
        return this.post('/api/IotConfig/GetWebConfig');
    },

    // 设置web配置
    setWebConfig (data) {
        return this.post('/api/IotConfig/SetWebConfig', data);
    },

    // 重置web配置
    resetWebConfig () {
        return this.post('/api/IotConfig/ResetWebConfig');
    },

    // 获取host配置
    getHostConfig () {
        return this.post('/api/HostConfig/GetHostConfig');
    },

    // 更新host配置
    setHostConfig (data) {
        return this.post('/api/HostConfig/SetHostConfig', data);
    },

    // 获取许可信息
    getLicense () {
        return this.post('/api/HostConfig/GetLicense');
    },

    // 测试连接
    VerifyConn(data) {
        return this.post('/api/HostConfig/VerifyConn', data);
    },

    HostVerifyLogin(data) {
        return this.post('/api/HostConfig/VerifyLogin', data);
    },

    // 首次登录
    IsInitMaintainPwd() {
        return this.get('/api/ServiceManage/IsInitMaintainPwd');
    },

    // 初始化管理员密码
    InitAdminPwd(initData) {
        return this.post('/api/Maintain/InitAdminPwd', initData);
    },

    // 验证管理员密码
    VerifyLogin(data) {
        return this.post('/api/Maintain/VerifyLogin', data);
    },

    // 获取服务状态
    GetServiceStatus() {
        return this.get('/api/ServiceManage/GetServiceStatus');
    },

    // 提交文件
    UploadLicense(data) {
        return this.post('/api/ServiceManage/UploadLicense', data);
    },

    // 获取注册码与许可状态
    GetLicenseInfo() {
        return this.get('/api/ServiceManage/GetLicenseInfo');
    },

    // 启动服务、重启网关、更新许可
    Reboot() {
        return this.post('/api/ServiceManage/Reboot');
    },

    // 初始化用户数据
    InitService() {
        return this.post('/api/ServiceManage/InitService');
    },

    // 重启网站
    RebootWeb() {
        return this.post('/api/ServiceManage/RebootWeb');
    },

    // 初始化数据库
    InitDb(data) {
        return this.post('/api/HostConfig/InitDb', data);
    },

    // 验证配置内容
    VerifyContent(data) {
        return this.post('/api/IotConfig/VerifyContent', data);
    },

    // 制作密文
    MakeCiphertext(data) {
        return this.post('/api/IotConfig/MakeCiphertext', data);
    },

    // 获取初始化状态
    GetInitSate() {
        return this.get('/api/ServiceManage/GetInitSate');
    },

    // 更新许可
    UpdateLicense(data) {
        return this.post('/api/ServiceManage/UpdateLicense', data);
    },

    // 下载日志
    DownLoadXlog() {
        return url + '/api/Maintain/DownLoadXlog';
    },

    // 上传Https证书
    UploadCert(data) {
        return this.post('/api/HostConfig/UploadCert', data);
    }
}

export default ConfigManage;
