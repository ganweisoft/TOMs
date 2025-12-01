/**
 * 登录鉴权
 */

import qs from 'qs'; // 根据需求是否导入qs模块
const Auth = {

    // 确认用户条款
    addUserServer() {
        return this.post('/IoT/api/v3/Auth/AddUserServer')
    },

    // 获取验证码
    getVerificationCode() {
        return this.get('/IoT/api/v3/Auth/GetVerificationCode?codeType=0');
    },

    // 用户登录,获取token
    login(params) {

        return this.post('/IoT/api/v3/Auth/Login', params);
    },

    // 判断是否登录
    isLoggedIn(params) {
        return this.post('/IoT/api/v3/AutoLogin/IsLoggedIn', params);
    },

    // 用户自动登录
    autoLogin(params) {
        return this.post('/IoT/api/v3/AutoLogin/AutoLogin', params);
    },

    // 退出登录
    loginOut() {
        return this.get('/IoT/api/v3/Auth/LoginOut');
    },

    // 用户登录APP,获取token
    loginApp(params) {
        return this.post('/api/v3/Auth/KeyAPP', qs.stringify(params));
    },

    // 修改密码
    getUpdUserInfoData(data) {
        return this.post('/IoT/api/v3/Auth/UpdUserInfoData', data);
    },

    // 获取软件授权名称
    getAuthName() {
        return this.get('/api/v3/Auth/GetAuthName');
    },

    // 获取系统运行信息
    getSystemInfo() {
        return this.get('/IoT/api/v3/Auth/GetSystemRuntimeInfo');
    },

    // 获取AlarmCenterAPI版本号
    getApiVersion(params) {
        return this.get('/api/v3/Auth/AlarmApiVersion');
    },

    // 获取用户权限模块
    getUserAuth(params) {
        return this.post('/IoT/api/v3/Auth/GetUserAuth', params);
    },

    // 获取授权用户名
    getUserName2SF() {
        return this.get('/IoT/api/v3/Auth/GetName2SF');
    },
    GetMenus(params) {
        return this.get('/IoT/api/v3/Auth/GetMenus', params)
    },
    getcipher() {
        return this.get('/IoT/api/v3/auth/getcipher')
    },

    // 2023-02-23 添加安全模式
    getSafeLevelByGateway() {
        return this.get('/IoT/api/v3/GWAssembly/GetSafeLevelByGateway');
    },

    setSafeLevelByGateway(data) {
        return this.post('/IoT/api/v3/GWAssembly/SetSafeLevelByGateway?level=' + data);
    },

    IsIgnoreFalidateCode() {
        return this.get('/IoT/api/v3/auth/IsIgnoreValidateCode');
    }
    
};

export default Auth;