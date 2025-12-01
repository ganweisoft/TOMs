import { ElMessageBox } from 'element-plus'
import i18n from '@components/@ganwei-pc/gw-base-utils-plus/i18n'
import notification from '@components/@ganwei-pc/gw-base-utils-plus/notification.js'

let axios;
if (window.AxiosBuilder) {
    axios = new window.AxiosBuilder()
        .withDeafultConfig()
        .withDeafultRequestInterceptor()
        .withDeafultResponseInterceptor()
        .withTipConfig(i18n, notification, ElMessageBox)
        .build()
        .getInstance();
}

const reqUrl = '';
axios.defaults.withCredentials = true; // 让ajax携带cookie

const api = {
    getjsontranslationfile (data) {
        return axios({
            method: 'get',
            url: reqUrl + '/api/localization/getjsontranslationfile',
            params: data,
            data: data,
            headers: {
                'Content-Type': 'application/json;charset=UTF-8',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },

    // 数据推送IP返回
    getSignalrHttp () {
        return reqUrl;
    },

    get: function (url, data) {
        return axios({
            method: 'get',
            url: reqUrl + url,
            params: data,
            data: data,
            headers: {
                'Content-Type': 'application/json;charset=UTF-8',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },

    post: function (url, data = {}) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },

    // 文件数据流导出
    postBlob: function (url, data) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            },

            // 文件下载返回数据流
            responseType: 'blob'
        });
    },

    getExcelFile: function (url, data) {
        return axios({
            method: 'get',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            },
            'responseType': 'blob'
        });
    },

    getFile: function (url, data) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            },
            'responseType': 'blob'
        });
    },

    postFile: function (url, data) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            data: data,
            cache: false,
            processData: false,
            contentType: false,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },

    postUrl: function (url, set) {
        return axios({
            method: 'post',
            url: reqUrl + url + '?' + set.type + '=' + set.data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },
    postParam: function (url, data) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            params: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },
    delete: function (url, data) {
        return axios({
            method: 'delete',
            url: reqUrl + url,
            params: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },
    deleteByData: function (url, data) {
        return axios({
            method: 'delete',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },
    put: (url, data) => {
        return axios({
            method: 'put',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },
    getWord: function (url, data) {
        return axios({
            method: 'get',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },
    getForm: function (url, data) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            params: data,
            data: data,
            headers: {
                'Content-Type': 'multipart/form-data',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    },

    download (url, data) {
        return axios({
            method: 'get',
            url: reqUrl + url,
            params: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            },
            'responseType': 'blob'
        });
    }
};
export default api;
