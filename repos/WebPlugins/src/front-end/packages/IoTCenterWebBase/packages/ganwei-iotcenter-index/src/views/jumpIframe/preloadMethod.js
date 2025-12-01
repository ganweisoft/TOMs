export default {
    created () {
        window.preloadApi = this.preloadApi.bind(this)
    },
    methods: {

        /**
         * 预加载接口--第三方应用配置后URL携带返回参数
         * @param {String} configName packageId
         * @param {String} customParameterName 自定义参数名称
         * @param {String} path 请求路径
         * @param {String} methods 请求方法
         * @param {Object} data 请求参数
         */
        preloadApi () {
            let routerPath = window.decodeURIComponent(this.$route.path)
            let routerFullPath = window.decodeURIComponent(this.$route.fullPath)
            let originName = this.routesToPackageId[routerFullPath] || this.routesToPackageId[routerPath], preloadConfig
            try {
                preloadConfig = JSON.parse(sessionStorage.configInfoData)?.preloadConfig
            } catch (error) {
                preloadConfig = []
            }
            if (Array.isArray(preloadConfig)) {
                preloadConfig = preloadConfig?.filter(item => {
                    return item?.configName?.toString()?.trim() == originName?.toString()?.trim()
                })
            } else {
                preloadConfig = []
            }
            let configName = preloadConfig[0]?.configName || "敢为云"
            let path = preloadConfig[0]?.path || '/api/idsvr/connect/connect'
            let methods = preloadConfig[0]?.methods || 'get'
            let data = preloadConfig[0]?.data || {}
            this.customName = preloadConfig[0]?.customParameterName || 'token'
            let xhr = this.$api
            return new Promise((resolve, reject) => {
                if (originName && configName && originName?.toString()?.trim() == configName?.toString()?.trim()) {
                    xhr[methods](path, data).then(res => {
                        if (res?.data?.code == 200) {
                            resolve(res)
                        } else {
                            reject({})
                        }
                    }).catch(e => {
                        reject({})
                    })
                } else {
                    resolve({})
                }
            })
        }
    }
}
