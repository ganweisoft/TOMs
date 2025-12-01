/**
 * 单点登录
 */

const Iam = {

    // IAM 获取AccessToken接口
    IamGetToken (params) {
        return this.post('/IoT/huawei/IAM/V1/oauth/getToken', params);
    }
}

export default Iam;