<template>
    <div class="jumpPage">
        <loading v-show="loading"></loading>
        <iframe v-if="name != 'noPage' && isHasPage" :id="name" :src="path" width="100%" height="100%" allowfullscreen></iframe>
        <noPage v-if="name == 'noPage'"></noPage>
    </div>
</template>
<script>
import noPage from '@/components/noAccess/noPage.vue'

import preloadMethod from './preloadMethod.js'
import QueryParser from './queryParser'
export default {
    name: 'MFEJump',
    mixins: [preloadMethod],
    components: {
        noPage
    },
    inject: ['outerLinkMap', 'routesToName', 'routesToPackageId'],
    data () {
        return {
            loading: true,
            path: '',
            name: 'MFEJump',
            isHasPage: false,
            pageNofound: "Status Code: 404; Not Found",
            customName: '',
            auth: '',
            queryParser: new QueryParser(),
        };
    },
    created () {
        if (this?.$store?.state?.loadingStatus) {
            this?.$store?.commit("setLoadingStatus", false)
            return;
        }

        // 预加载方法
        this.preloadApi().then(res=>{
            if(res?.data?.data) {
                if(typeof res?.data?.data == 'string' || typeof res?.data?.data == 'number') {
                    this.auth = res?.data?.data
                }
            }
        }).catch(e=>{
            console.log(e)
        }).finally(()=>{
            this.queryParser = new QueryParser(this.$route)
            this.loadApplication()
        })
    },
    methods: {
        async loadApplication () {
            // 检查模块名
            let packageName = this.$route.params.packageName, route = this.$route?.params?.route || '';
            if (!packageName) {
                this.$route.query.pageNofound = true;
                this.isNoPage()
                return
            }

            this.setInternalLocation(packageName, route)
            let path = this.queryParser.getFullPath()
            let menuName = '', packageId = ''

            try {
                if (!path.includes('outerLink=true')) {
                    menuName = route.split('/')[0].split('?')[0]
                    packageId = this.getPackageId(this.$route.path, this.$route.fullPath)
                } else {
                    path = this.setOuterLocation(this.outerLinkMap[decodeURIComponent(this.$route.fullPath)])
                }
            } catch (error) {
                console.log(error)
            }

            // 传递应用参数
            this.queryParser.addQuery({
                packageId,
                pluginName: packageName,
                menuName,
                languageType: localStorage.languageType,
                userName: sessionStorage.userName,
                auth: this.auth,
                customName: this.customName,
                // 会自动 encodeURIComponent,所以需要先decodeURIComponent
                CSRF_TOKEN: decodeURIComponent(sessionStorage.getItem('CSRF_TOKEN') || '')
            })
            this.loadActualPath(packageName)

            this.$nextTick(() => {
                this.checkFrameLoad(packageName)
            })

        },

        setInternalLocation(packageName, route) {
            let location = (this.$hostMap(packageName) || '') + (route.indexOf(".html") != -1 ? '/' : '/#/') + route
            this.queryParser.setLocation(location)
            return location
        },

        setOuterLocation(outerLink) {
            const url = new URL(outerLink)
            let location = url.origin + url.pathname
            this.queryParser.setLocation(location)
            return location
        },

        async languageChangeFun (packageName, menuName, packageId) {
            if(packageName && menuName && packageId) {
                return window.getLanguage(packageName, menuName, packageId, this)
            }
        },

        loadActualPath(packageName) {
            let path = this.queryParser.getFullPath()
            if (process.env.NODE_ENV == 'production') {
                this.isHasPage = true;
                this.name = this.path = path;
            }

            if (process.env.NODE_ENV == 'development') {
                if (!this.$hostMap(packageName) && path && !path.includes('http')) {
                    this.name = 'noPage'
                    this.isHasPage = false
                    this.$notify({
                        title: '提示',
                        message: '请在hostMap.js配置该应用!'
                    });
                } else {
                    this.isHasPage = true;
                    this.name = path, this.path = path;
                }
            }
        },

        checkFrameLoad(packageName) {
            let iframe = document.getElementById(this.name)
            if (iframe) {
                iframe.onload = () => {
                    // 首次加载跟随主题框架初始化加载插件主题
                    try {
                        if(packageName != 'custom') {
                            iframe.contentWindow.document.documentElement.setAttribute('data-theme', sessionStorage.theme)
                            iframe.contentWindow.postMessage({ theme: sessionStorage.theme })
                        }
                        
                    } catch (error) {
                        console.log(error)
                    }
                    this.checkPageLoad()
                }
            } else {
                this.isNoPage()
            }
        },

        checkPageLoad () {
            this.loading = false;
            let isError = false;
            let dom = document.getElementById(this.name)
            if (dom) {
                try {
                    isError = dom.contentWindow.document.querySelector('body>pre')?.innerHTML.includes(this.pageNofound)
                } catch (error) {
                    console.log(error)
                }
            }
            if (isError) {
                this.isNoPage()
            }
        },

        async isFileExist (filePath) {
            let exist = false
            await this.$api.get(filePath).then(res => {
                if (res.status == 200) {
                    exist = true
                } else {
                    exist = false
                }
            }).catch(err => {
                console.log(err)
                exist = false
            })
            return exist
        },

        executeIframeFun (callback) {
            let iframe = document.getElementById(this.name)
            let hasQueue = false
            try {
                // 有些第三方链接，考虑到安全性问题，不允许读取contentWindow内部变量，会报错
                let executeQueues = iframe?.contentWindow?.executeQueues || {}
                Object.keys(executeQueues).forEach(item => {
                    hasQueue = true;
                    executeQueues[item]();
                })
            } catch (error) {
                console.log(error)
            }

            if (callback) {
                if (hasQueue) {
                    setTimeout(() => { callback() }, 500)
                } else {
                    callback()
                }
            }
        },

        getPackageId (route, fullPath) {
            return this.routesToPackageId[fullPath] || this.routesToPackageId[route]
        },

        isNoPage () {
            this.name = 'noPage';
            this.isHasPage = false;
            this.loading = false
        },
    },
};
</script>

<style lang="scss" scoped>
.jumpPage {
    width: 100%;
    height: 100%;
    background: var(--frame-main-background);
    iframe {
        color-scheme: light;
    }
}
</style>
