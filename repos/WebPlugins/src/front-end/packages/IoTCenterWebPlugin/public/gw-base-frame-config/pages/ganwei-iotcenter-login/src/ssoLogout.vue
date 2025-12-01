<template>
    <div>
        <a></a>
    </div>
</template>
<script>
export default {
    data () {
        return {
            url: ''
        }
    },
    mounted () {
        this.getSsoConfig();
    },
    methods: {

        async getSsoConfig () {
            await this.myUtils.configInfoData(this).then(webConfig => {
                if (!this.$route.params.appid) {
                    this.config = webConfig.ssoConfig.default.ssoLogOut
                } else {
                    this.config = webConfig.ssoConfig[this.$route.params.appid].ssoLogOut
                }
            })
            await this.ssoLogout()
        },

        async ssoLogout () {
            let ssoLogOut = await this.ssoUrlApi(this.config.api, this.config.method);
            if (ssoLogOut.status == 200 && ssoLogOut.data && ssoLogOut.data.data) {
                this.ssoLogOutConfig = ssoLogOut.data.data;
                let { serviceurl, ...queryData } = this.ssoLogOutConfig;

                let url = `${serviceurl}?`
                Object.keys(queryData).forEach((item, index) => {
                    if (index > 0) {
                        url += '&'
                    }
                    url += `${item}=${queryData[item]}`
                })
                url = url.replace(/\#/g, '%23');
                window.top.location.href = serviceurl ? url : '/';
            } else if (ssoLogOut.status == 200) {
                window.top.location.href = '/';
            }
        },
        ssoUrlApi (api, method) {
            return this.Axios({
                method: method,
                url: api
            });
        }
    }
}
</script>
<style lang="scss" scoped></style>