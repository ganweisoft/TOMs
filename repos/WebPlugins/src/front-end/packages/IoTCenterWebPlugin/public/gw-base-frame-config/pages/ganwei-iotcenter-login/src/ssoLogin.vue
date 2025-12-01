<template>
    <div id="ssoLogin" v-loading='loading' loading-type="2">
        <div class="error-box" v-show="isError">
            <img src="./images/login-sso-error.png">
            <p class="error-txt">{{ errorTxt }}</p>
            <el-button type="primary" @click="toBack">确定</el-button>
        </div>
    </div>
</template>
<script>
export default {
    data () {
        return {
            query: null,
            urlConfig: null,
            loginConfig: null,
            loading: true,
            isError: false,
            errorTxt: ''
        }
    },
    mounted () {

        this.getSsoConfig();
    },
    methods: {

        async getSsoConfig () {
            await this.myUtils.configInfoData(this).then(webConfig => {
                if (!this.$route.params.appid) {
                    this.urlConfig = webConfig.ssoConfig.default.ssoUrl
                    this.loginConfig = webConfig.ssoConfig.default.ssoLogin;
                } else {
                    this.urlConfig = webConfig.ssoConfig[this.$route.params.appid].ssoUrl
                    this.loginConfig = webConfig.ssoConfig[this.$route.params.appid].ssoLogin;
                }

            })

            await this.ssoLogin()
        },

        async ssoLogin () {
            if (this.$route.query) {
                this.query = this.$route.query;
            }

            if (JSON.stringify(this.query) == '{}') {

                // 调用获取重定向地址接口
                this.ssoUrlApi(this.urlConfig.api, this.urlConfig.method).then(res => {
                    if (res.data.code == 200) {
                        let data = res.data.data;
                        let { requestUrl, ...queryData } = data;
                        let url = `${data.requestUrl}?`
                        Object.keys(queryData).forEach((item, index) => {
                            if (index > 0) {
                                url += '&'
                            }
                            url += `${this.camel2UnderLine(item)}=${queryData[item]}`
                        })
                        url = url.replace(/\#/g, '%23');

                        window.top.location.href = url;
                    } else {
                        this.isError = true;
                        this.errorTxt = res.data.message;
                    }
                    this.loading = false;
                }).catch(er => {
                    this.isError = true;
                    this.loading = false;
                })
            } else {

                // 调用单点登录接口
                this.ssoApi(this.loginConfig.api, this.loginConfig.method, this.query).then(res => {
                    if (res.data.code == 200) {
                        this.$message.success('登录成功！');
                        if (res.data.data) {
                            sessionStorage.userName = res.data.data.userName
                            sessionStorage.roleName = res.data.data.roleName
                        }
                        sessionStorage.isSsoLogin = '1';
                        sessionStorage.ssoLoginAppId = this.$route.params.appid;
                        if (this.query.url) {
                            window.top.location.href = window.top.location.origin + decodeURI(this.query.url);
                        } else {
                            window.top.location.href = '/#/Index'
                        }

                        // this.$router.push('/Index');
                    } else {
                        this.isError = true;
                        this.errorTxt = res.data.message;
                    }
                    this.loading = false;
                }).catch(er => {
                    this.isError = true;
                    this.loading = false;
                    this.errorTxt = '出现异常，请联系平台运维人员处理~';
                })
            }

        },

        // 可配置接口
        ssoUrlApi (api, method) {
            return this.Axios({
                method: method,
                url: api
            });
        },

        // 可配置接口
        ssoApi (api, method, data) {
            return this.Axios({
                method: method,
                url: api,
                data: method.toLowerCase() == 'get' ? null : data,
                params: method.toLowerCase() == 'post' ? '' : data
            });
        },

        toBack () {
            window.history.back(-1);
        },

        camel2UnderLine (name) {
            let isCamel = name.match(/^([a-z]+)([A-Z][a-z]*)$/);
            if (isCamel) {
                name = isCamel[1] + '_' + isCamel[2].toLowerCase();
            }
            return name;
        }
    }
}
</script>
<style lang="scss" scoped>
#ssoLogin {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;

    .error-box {
        width: 80%;
        max-width: 570px;
        text-align: center;
        transform: translateY(-10px);

        img {
            width: 100%;
        }

        .error-txt {
            font-size: 24px;
            color: #90959b;
            margin: 26px 0 28px;
        }

        .el-button {
            width: 160px;
            font-size: 22px;
            padding: 8px 20px;

            &:not(:hover) {
                background-color: #669eff;
                border-color: #669eff;
            }
        }
    }
}
</style>