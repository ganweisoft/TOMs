const SecuritySetting = {

    // 获取安全设置
    GetAccountPasswordRule() {
        return this.get('/IoT/api/v3/UserManage/GetAccountPasswordRule');
    },

    // 修改安全设置
    CreateAccountPasswordRule(data) {
        return this.post('/IoT/api/v3/UserManage/CreateAccountPasswordRule', data);
    }
}
export default SecuritySetting;
