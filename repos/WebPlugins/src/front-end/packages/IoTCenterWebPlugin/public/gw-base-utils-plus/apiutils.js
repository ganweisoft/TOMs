import api from 'gw-base-api-plus/api.js';

/**
  * @param {Number} H5流媒体服务登录
  * @returns {Number}
  */
export function streamLogin () {

    // 登录H5Stream平台
    if (window.localStorage.ac_session.length > 15) {
        return;
    }
    api.loginVideo({
        hostUrl: '/api/v1/Login',
        user: '',
        password: ''
    })
        .then(res => {
            let result = res.data;
            if (result.code == 200 && result.data.length > 0) {
                let resultData = JSON.parse(result.data);
                window.localStorage.ac_session = resultData.strSession;
            } else {
                this.$message.error(res.data.message, res);
            }
        })
        .catch(err => {
            this.$message.error(err.data, err);
            console.log(this.$t('myUtils.msg.streamLogin[1]'), err);
        });

}