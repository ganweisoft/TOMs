<template>
    <div class="jumpIframeContainers">
        <iframe :src="url" frameborder="0" class="jumpIframes" ref="jumpIframe"></iframe>
    </div>
</template>
<script>
export default {
    data() {
        return {
            url: ''
        }
    },
    created() {
        // sessionStorage.languageSet = 'zh-CN'
        // this.$i18n.locale = 'zh-CN'
        // 中英文暂时接口未迁移，后期接口迁移再恢复
        // this.getLanguageLogos();
    },
    mounted() {
        let packageSrc = process.env.NODE_ENV == 'development' ? process.env.PAGE_ENV + '/' : '/'
        let queryVal = this.$route.query.systemName ? '?systemName=' + this.$route.query.systemName : ''
        let fullPath = this.$route.fullPath
        this.url = this.$router.currentRoute.path.replace('/jumpIframeLogin/', '')
        this.url.indexOf('.html') != -1 ? (this.url = packageSrc + this.url + queryVal) : (this.url = packageSrc + this.url.replace('/', '/#/') + queryVal)
        if (fullPath.split('?').length > 1) {
            this.url = this.url + '?' + fullPath.split('?')[1]
        }
        window.sessionStorage.removeItem('menuActiveName')
        window.sessionStorage.removeItem('menuActive')
        let iframe = document.getElementsByClassName('jumpIframes')[0]
        iframe.onload = () => {
            // this.myUtils.setStyle(iframe.contentWindow.document.getElementsByTagName("head")[0])
        }
    },

    methods: {
        // 读取中英文状态
        getLanguageLogos() {
            this.$api
                .getLanguageLogo()
                .then(rt => {
                    if (rt.data.code === 200) {
                        this.$i18n.locale = rt.data.data.language
                        sessionStorage.languageSet = rt.data.data.language
                    } else {
                        this.$message.error(rt.data.message)
                    }
                })
                .catch(err => {
                    console.log(err)
                })
        }
    }
}
</script>
<style lang="scss" scoped>
.jumpIframeContainers {
    width: 100%;
    height: 100%;
    position: relative;
}

.jumpIframes {
    width: 100%;
    height: 100%;
    overflow: hidden;
    border: none;
    padding: 0 !important;
}
</style>
