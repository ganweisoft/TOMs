const frame = {
    // 获取菜单
    GetMenus (params) {
        return this.get('/IoT/api/v3/Auth/GetMenus', params)
    },
    // 获取用户信息
    getUserInfo () {
        return this.get('/IoT/api/v3/Auth/userInfo')
    },
    // 获取软件授权名称
    getAuthName () {
        return this.get('/IoT/api/v3/Auth/GetName2SF');
    },
    // 导出历史曲线
    exportCurve (data) {
        return this.post('/IoT/api/v3/EquipList/ExportEquipHistroyCurves', data)
    },
    //获取语言包
    getjsontranslationfile (data) {
        return this.get('/api/localization/getjsontranslationfile', data);
    },
    // 退出登录
    loginOut (type) {
        return type === 'post' ? this.post('/IoT/api/v3/Auth/LoginOut') : this.get('/IoT/api/v3/Auth/LoginOut'); 
    },
    // 修改密码
    getUpdUserInfoData (data) {
        return this.post('/IoT/api/v3/Auth/UpdUserInfoData', data);
    },
    // 获取安全设置
    GetAccountPasswordRule () {
        return this.get('/IoT/api/v3/UserManage/GetAccountPasswordRule');
    },
    // 获取系统运行信息
    getSystemInfo () {
        return this.get('/IoT/api/v3/Auth/GetSystemRuntimeInfo');
    },
    // 获取服务状态
    getServiceStatus () {
        return this.get('/api/ServiceManage/GetServiceStatus')
    },
    // 2023-02-23 添加安全模式
    getSafeLevelByGateway () {
        return this.get('/api/ServiceManage/GetSafeLevelByGateway');
    },
    // 更新应用授权
    updateApplicationAuth () {
        return this.post('/api/HostConfig/RefreshGWStoreInfo')
    },

    // 重启网关
    RebootHost () {
        return this.post('/api/HostConfig/RebootHost');
    },

    // 重启时账号密码校验
    HostVerifyLogin (data) {
        return this.post('/api/HostConfig/VerifyLogin', data);
    },

    // 重启网站
    RebootWeb () {
        return this.post('/api/ServiceManage/RebootWeb');
    },

    // 设置安全等级
    setSafeLevel (level) {
        return this.post('/api/ServiceManage/SetSafeLevelByGateway?level=' + level);
    },

    // 检查APP模块
    checkAppModule (data) {
        return this.get('/api/pageCheck', data);
    },

    // 保存主题
    saveTheme (data) {
        return this.post('/IoT/api/v3/FrontConfiguration/EditPersonnelFrontConfigData', data);
    },

    // 获取升级列表
    getUpgradePluginList (data) {
        return this.post('/IoT/api/v3/GWAssembly/GetPluginList', data);
    },

    // 升级插件
    upgradePlugin (data) {
        return this.post('/IoT/api/v3/GWAssembly/BatchInstallAppStorePlugin', data);
    },

    // 获取权限
    testAuth (data) {
        return this.get('/loT/api/v3/UserManager/getPageControlPermission', data);
    }
}

export default frame
