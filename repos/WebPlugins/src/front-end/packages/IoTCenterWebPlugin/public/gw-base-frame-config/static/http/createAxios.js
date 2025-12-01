/**
 * axios封装
 * 请求拦截、响应拦截、错误统一处理
 */

; (function (_this) {
    let currentAxiosBuilder = null
    let alreadyGetLanguage = false

    const tip = async (msg, zhMsg, duration) => {
        // RefactorUI版本阻断
        if (window.sessionStorage.versionFlag) {
            currentAxiosBuilder.notification({
                title: currentAxiosBuilder.i18n.tc(zhMsg),
                duration: duration,
                type: 'error'
            })
            return
        }

        if (!window.top.i18n && !alreadyGetLanguage) {
            alreadyGetLanguage = true
            if (!sessionStorage.LanguageRequestReCord) {
                sessionStorage.LanguageRequestReCord = 1
                await window.top.getLanguage('ganwei-iotcenter-login', 'login', 'Ganweisoft.IoTCenter.Module.Login', null, currentAxiosBuilder.i18n, '/api/localization/getjsontranslationfile')
            }
        }

        if (window.top.i18n) {
            alreadyGetLanguage = true
        }

        if (alreadyGetLanguage) {

            currentAxiosBuilder.notification.error(currentAxiosBuilder.i18n.tc(msg))
        }
    }

    /**
     * 跳转登录页
     * 携带当前页面路由，以期在登录页面完成登录后返回当前页面
     */
    const toLogin = n => {
        if (top.location.href.indexOf('Login') == -1) {
            (top.location.href = '/')
        }
    }

    const toNoAccess = n => {
        if (!window.top.location.href.includes('-login')) {
            top.location.href = '/#/Index/noAccess?name=' + n
        }
    }

    /**
     * 请求失败后的错误统一处理
     * @param {Number} status 请求失败的状态码
     */
    const errorHandle = function (status, other) {
        // 状态码判断
        switch (status) {
            case 401: // 403 token过期 清除token并跳转登录页
                if (window.sessionStorage.getItem('isSsoLogin')) {
                    window.sessionStorage.clear()
                    top.location.href = '/#/jumpIframeLogin/ganwei-iotcenter-login/ssoLogout'
                } else {
                    toLogin()
                }
                break
            case 403:
                tip('login.framePro.tips.noJurisdiction', '当前接口无权限', 1500)
                break
            case 404:
                break
            case 500: // ghost出现问题
                let url = other.config.url
                if (url.indexOf('/IoT/api/v3/Auth/GetMenus') != -1 || url.indexOf('IoT/api/v3/RealTime/GetRealTimeData') != -1) {
                    toNoAccess(2)
                } else {
                    tip('login.framePro.tips.internalException', '服务内部异常', 3000);
                }
                break
            case 501:
                break
            case 504:
                tip('login.framePro.tips.internalException', '网关超时', 3000);
                // toNoAccess(2)
                break
            default:
                break
        }
    }

    let defaultResponseInterceptorConfig = {
        fullfilled: res => {
            if (res.status === 200 || res.status === 201 || res.status === 204) {
                let requestData = res.request
                if (requestData.responseURL.indexOf('?ReturnUrl=') != -1) {
                    toNoAccess(1)
                    return
                }
            }
            if (!res || !res.data) {
                res.data = {
                    code: 400,
                    message: currentAxiosBuilder.i18n.tc('login.framePro.tips.internalException')
                }
                return Promise.resolve(res)
            }
            return res.status === 200 || res.status === 201 ? Promise.resolve(res) : Promise.reject(res)
        },
        reject: function axiosRetryInterceptor (err) {
            const instance = currentAxiosBuilder.axios

            const { response } = err
            let retryWhiteList = err.config.retryWhiteList
            let url = err.config.url
            let location = url.split('?')[0]

            // 判断是否是需要重复请求的api
            let ifRetryApi = retryWhiteList.findIndex(reg => reg.test(location)) > -1

            // 请求响应报错，状态码不为200时
            if (response && !ifRetryApi) {
                errorHandle(response.status, response)
                return Promise.reject(response)

                // 重连接口操作，如：请求超时、请求失败需要重复请求的接口
            } else if (response == undefined || ifRetryApi) {
                let config = err.config
                if (ifRetryApi) {
                    // 初始化当前重连次数
                    config.__retryCount = config.__retryCount || 0

                    // 超过重连次数时
                    if (config.__retryCount > config.retry) {
                        // 超过重连次数且有响应内容时，进行相应的错误处理
                        if (response) {
                            errorHandle(response.status, response)
                            return Promise.reject(response)
                        } else {
                            tip('login.framePro.tips.overtime', '网络请求超时,请继续等待或者重新登录', 3000)
                            return Promise.reject({})
                        }
                    }

                    // 重连次数加一
                    config.__retryCount += 1

                    // 创建一个新的promise来处理请求
                    let backoff = new Promise(function (resolve) {
                        setTimeout(function () {
                            resolve()
                        }, config.retryDelay || 1000)
                    })

                    // 返回一个axios重试请求的promise
                    return backoff.then(function () {
                        return instance(config)
                    })
                }

                // 接口请求超时的处理
                if (response == undefined) {
                    tip('login.framePro.tips.overtime', '网络请求超时,请继续等待或者重新登录', 3000)
                    return Promise.reject({})
                }
            }
        }
    }

    _this['AxiosBuilder'] = function AxiosBuilder () {
        this.axios = window.axios
        this.config = {}
        this.requestInterceptor = []
        this.responseInterceptor = []
        this.i18n = null
        this.notification = null
    }

    AxiosBuilder.prototype.withTipConfig = function withTipConfig (i18n, notification) {
        this.i18n = i18n
        this.notification = notification
        return this
    }

    AxiosBuilder.prototype.withDeafultConfig = function withDeafultConfig (config) {
        this.config = config || httpConfig
        return this
    }

    AxiosBuilder.prototype.withRequestInterceptor = function withRequestInterceptor (requestInterceptor) {
        if (Array.isArray(requestInterceptor) && requestInterceptor[0] instanceof Function && requestInterceptor[1] instanceof Function) {
            this.requestInterceptor.push(requestInterceptor)
        } else {
            console.error('requestInterceptor should be [(config) => {}, (error) => {}]')
        }
        return this
    }

    AxiosBuilder.prototype.withResponseInterceptor = function withResponseInterceptor (responseInterceptor) {
        if (Array.isArray(responseInterceptor) && responseInterceptor[0] instanceof Function && responseInterceptor[1] instanceof Function) {
            this.responseInterceptor.push(responseInterceptor)
        } else {
            console.error('responseInterceptor should be [(response) => {}, (error) => {}]')
        }
        return this
    }
    AxiosBuilder.prototype.withDeafultRequestInterceptor = function withDeafultRequestInterceptor () {
        this.requestInterceptor.push([
            config => {
                config.headers['Accept-Language'] = window.sessionStorage.languageType || 'zh-CN'
                return config
            },
            error => Promise.error(error)
        ])
        return this
    }
    AxiosBuilder.prototype.withDeafultResponseInterceptor = function withDeafultResponseInterceptor () {
        this.responseInterceptor.push([defaultResponseInterceptorConfig.fullfilled, defaultResponseInterceptorConfig.reject])
        return this
    }
    let httpConfig = {
        countRetry: 0,
        timeout: 30000,
        retryDelay: 3000
    }

    AxiosBuilder.prototype.build = function build () {
        this.axios = axios.create(this.config)
        this.axios.defaults.headers.post['Content-Type'] = 'application/json'
        this.axios.defaults.headers['X-Requested-With'] = 'IoT-XMLHttpRequest'
        this.axios.defaults.withCredentials = true
        this.requestInterceptor.forEach(r => {
            this.axios.interceptors.request.use(...r)
        })
        this.responseInterceptor.forEach(r => {
            this.axios.interceptors.response.use(...r)
        })

        this.axios.defaults['timeout'] = httpConfig.timeout
        this.axios.defaults['retry'] = httpConfig.countRetry
        this.axios.defaults['retryDelay'] = httpConfig.retryDelay
        this.axios.defaults['retryWhiteList'] = []
        if (_this.top.getConfigInfoData) {
            _this.top.getConfigInfoData().then(res => {
                if (res.httpConfig) {
                    this.axios.defaults['timeout'] = res.httpConfig.timeout
                    this.axios.defaults['retry'] = res.httpConfig.countRetry
                    this.axios.defaults['retryDelay'] = res.httpConfig.retryDelay
                    this.axios.defaults['retryWhiteList'] = Array.from(new Set(res.httpConfig.retryWhiteList.replaceAll('\n', '').split(','))).map(str => new RegExp(str))
                }
            }).catch(err => {
                console.log(err)
            })
        }
        return this
    }
    AxiosBuilder.prototype.getInstance = function getInstance () {
        currentAxiosBuilder = this
        return this.axios
    }
})(window)
