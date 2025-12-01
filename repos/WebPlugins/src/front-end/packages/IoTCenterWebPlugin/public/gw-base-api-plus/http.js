/**
 * axios封装
 * 请求拦截、响应拦截、错误统一处理
 */
import axios from 'axios';
import notification from 'gw-base-utils-plus/notification'
import i18n from 'gw-base-utils-plus/i18n'
export default () => {
    let alreadyGetLanguage = false

    function configInfoData () {
        return new Promise((resolve, reject) => {
            if (window.top.configInfoData) {
                resolve(window.top.configInfoData)
            } else {
                axios({
                    methed: 'get',
                    url: '/IoT/api/v3/Frontconfiguration/GetFrontconfigurationData'
                }).then(res => {
                    if (res.data.code == 200 && res.data.data) {
                        window.top.configInfoData = JSON.parse(res.data.data)
                        resolve(JSON.parse(res.data.data))
                    } else {
                        reject({})
                    }
                }).catch(err => {
                    console.log(err, '获取平台配置失败')
                    reject({})
                })
            }
        })
    }

    /**
     * 提示函数
     * 不显示关闭按钮、显示一秒后关闭
     */
    const tip = async (msg, zhMsg, duration) => {

        // RefactorUI版本阻断
        if (window.sessionStorage.versionFlag) {
            notification({
                title: i18n.tc(zhMsg),
                duration: duration,
                type: 'error'
            });
            return;
        }

        if (!window.i18n && !alreadyGetLanguage) {
            alreadyGetLanguage = true
            await getLoginLanuage(sessionStorage.languageType, 'ganwei-iotcenter-login', 'login', 'Ganweisoft.IoTCenter.Module.Login', zhMsg, duration)
        }

        if (alreadyGetLanguage) {
            notification({
                title: i18n.tc(msg),
                duration: duration,
                type: 'error'
            });
        }
    }



    function getLoginLanuage (languageType, pluginName, menuName, packageId, msg, duration) {
        return new Promise((resolve, reject) => {
            let data = {
                pluginName: pluginName,
                menuName: menuName,
                packageId: packageId
            }

            instance.defaults.withCredentials = true; // 让ajax携带cookie
            instance({
                method: 'get',
                url: '/api/localization/getjsontranslationfile',
                params: data,
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8'
                }
            }).then(res => {
                if (res.data.code == 200) {
                    i18n._vm.messages[sessionStorage.languageType][menuName] = JSON.parse(res.data.data)
                    window.i18n = i18n
                    resolve(res.data.data)
                } else {
                    console.log('get Language error');
                }
            }, e => {
                notification({
                    title: i18n.tc(msg),
                    duration: duration,
                    type: 'error'
                });
            })
        })
    }

    /**
     * 跳转登录页
     * 携带当前页面路由，以期在登录页面完成登录后返回当前页面
     */
    const toLogin = (n) => {
        if (top.location.href.indexOf('Login') == -1) {
            sessionStorage.roleName ? top.location.href = '/#/Index/noAccess?name=' + n : top.location.href = '/';
        }
    }

    /**
     * 请求失败后的错误统一处理
     * @param {Number} status 请求失败的状态码
     */
    const errorHandle = (status, other) => {

        // 状态码判断
        switch (status) {
            case 401: // 403 token过期 清除token并跳转登录页
                if (window.sessionStorage.getItem('isSsoLogin')) {
                    window.sessionStorage.clear();
                    top.location.href = '/#/jumpIframeLogin/ganwei-iotcenter-login/ssoLogout';
                } else {
                    toLogin(1);
                }
                break;
            case 403: // 403 拒绝访问接口 清除token并跳转无权限页面
                // store.commit('loginMsg', null);
                tip('login.framePro.tips.noJurisdiction', '当前接口无权限', 1500)
                break;
            case 404:

                // if (top.location.href.indexOf('Login') == -1 && (other.config.url.indexOf('IoT/api/v3/Auth/GetVerificationCode') == -1 || other.config.url.indexOf('api/localization/getsupportedcultures') == -1)) {
                tip('login.framePro.tips.noInterface', '请联系管理员更新服务接口', 3000);

                // }
                break;
            case 500: // ghost出现问题
                let url = other.config.url;
                if (url.indexOf('/IoT/api/v3/Auth/GetMenus') != -1 || url.indexOf('IoT/api/v3/RealTime/GetRealTimeData') != -1) {
                    toLogin(2);
                } else {
                    tip('login.framePro.tips.internalException', '服务内部异常', 3000);
                }
                break;
            case 501:
                break;
            case 504:
                tip('login.framePro.tips.internalException', '网关超时', 3000);
                toLogin(2);
                break;
            default:

                break;
        }
    }
    let httpConfig = {
        'countRetry': 0,
        'timeout': 30000,
        'retryDelay': 3000
    }
    configInfoData().then(webConfig => {
        if (webConfig) {
            httpConfig = webConfig.httpConfig
        }
    })


    // 创建axios实例
    // let httpConfig = window.localStorage.getItem(webConfig).httpConfig ? JSON.parse(window.sessionStorage.httpConfig) : ;
    let instance = axios.create({
        timeout: httpConfig.timeout,
        retry: httpConfig.countRetry,
        retryDelay: httpConfig.retryDelay
    });

    // 设置post请求头
    instance.defaults.headers.post['Content-Type'] = 'application/json';
    instance.defaults.headers['X-Requested-With'] = 'IoT-XMLHttpRequest';

    // instance.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=UTF-8';

    /**
     * 请求拦截器
     * 每次请求前，如果存在token则在请求头中携带token
     */
    instance.interceptors.request.use(
        config => {

            // 登录流程控制中，根据本地是否存在token判断用户的登录情况
            // 但是即使token存在，也有可能token是过期的，所以在每次的请求头中携带token
            // 后台根据携带的token判断用户的登录情况，并返回给我们对应的状态码
            // 而后我们可以在响应拦截器中，根据状态码进行一些统一的操作。
            // const token = store.state.gwToken;
            // token && (config.headers.Authorization = token);
            config.headers['Accept-Language'] = window.sessionStorage.languageType || 'zh-CN'
            return config;
        },
        error => Promise.error(error))

    instance.interceptors.response.use((res) => {
        if (res.status === 200 || res.status === 201 || res.status === 204) {
            let requestData = res.request;
            if (requestData.responseURL.indexOf('?ReturnUrl=') != -1) {
                toLogin(1);
                return;
            }
        }
        return (res.status === 200 || res.status === 201) ? Promise.resolve(res) : Promise.reject(res);

    }, function axiosRetryInterceptor (err) {

        const { response } = err;
        let url = err.config.url

        // 判断是否是需要重复请求的api
        let ifRetryApi = url.indexOf('IoT/api/v3/Auth/GetVerificationCode') != -1 || url.indexOf('api/localization/getsupportedcultures') != -1 || url.indexOf('api/localization/getjsontranslationfile') != -1

        // 请求响应报错，状态码不为200时
        if (response && !ifRetryApi) {
            errorHandle(response.status, response);
            return Promise.reject(response);

            // 重连接口操作，如：请求超时、请求失败需要重复请求的接口
        } else if ((response == undefined) || ifRetryApi) {
            let config = err.config;

            // 接口请求超时的处理
            if (response == undefined) {
                if (!config || !config.retry || config.url.indexOf('/api/ServiceManage/Reboot') != -1 || config.url.indexOf('/api/v3/RealTime/GetRealTimeData') != -1) {
                    if (config.url.indexOf('/api/v3/RealTime/GetRealTimeData') != -1) { tip('login.framePro.tips.overtime', '网络请求超时,请继续等待或者重新登录', 3000); }
                    return Promise.reject(err);
                }
                tip('login.framePro.tips.overtime', '网络请求超时,请继续等待或者重新登录', 3000);

            }

            // 初始化当前重连次数
            config.__retryCount = config.__retryCount || 1;

            // 超过重连次数时
            if (config.__retryCount > config.retry) {

                // 超过重连次数且有响应内容时，进行相应的错误处理
                if (!response.data) {
                    tip('login.framePro.tips.overtime', '网络请求失败,请检查服务是否正常启动', 3000);
                } else if (response != undefined) {
                    errorHandle(response.status, response);
                }

                // 超过重连次数时，结束重连
                return Promise.reject();
            }

            // 重连次数加一
            config.__retryCount += 1;

            // 创建一个新的promise来处理请求
            let backoff = new Promise(function (resolve) {
                setTimeout(function () {
                    resolve();
                }, config.retryDelay || 1000);
            });

            // 返回一个axios重试请求的promise
            return backoff.then(function () {
                return instance(config);
            });
        }

    });

    return instance;
}

