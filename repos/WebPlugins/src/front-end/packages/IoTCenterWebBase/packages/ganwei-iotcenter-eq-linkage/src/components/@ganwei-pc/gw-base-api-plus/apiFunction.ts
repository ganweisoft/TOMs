import { ElMessageBox } from 'element-plus'
import i18n from '@components/@ganwei-pc/gw-base-utils-plus/i18n'
import notification from '@components/@ganwei-pc/gw-base-utils-plus/notification'
import { AxiosInstance } from 'axios'
import qs from 'qs'

let axios: AxiosInstance;
if (window.AxiosBuilder) {
    axios = new window.AxiosBuilder()
        .withDeafultConfig()
        .withDeafultRequestInterceptor()
        .withDeafultResponseInterceptor()
        .withTipConfig(i18n, notification, ElMessageBox)
        .build()
        .getInstance() as AxiosInstance
}
interface Response<T> {
    code: number;
    data?: T;
    message: string;
}

export interface PageResponse<T> {
    rows: T[],
    total: number;
}

const reqUrl = '';
axios.defaults.withCredentials = true; // 让ajax携带cookie

class BaseService {
    // 数据推送IP返回
    static getSignalrHttp() {
        return reqUrl;
    }
    static getjsontranslationfile(data) {
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
    }
    static get<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'get',
            url: reqUrl + url,
            params: data,
            data: data,
            headers: {
                'Content-Type': 'application/json;charset=UTF-8',
                'Access-Control-Allow-Origin': reqUrl
            },
            paramsSerializer: function (params) {
                return qs.stringify(params, { arrayFormat: 'repeat' })
            }
        });
    }

    static getStatic<T>(url: string) {
        return axios<T>({
            method: 'get',
            url: reqUrl + url,
            headers: {
                'Content-Type': 'application/json;charset=UTF-8',
            }
        });
    }

    static post<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'post',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static postBlob<T extends Record<string, any>>(url: string, data?: T) {
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
    }

    static getExcelFile<T extends Record<string, any>>(url: string, data?: T) {
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
    }
    static getFile<T extends Record<string, any>>(url: string, data?: T) {
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
    }

    static postFile<T extends Record<string, any>>(url: string, data?: T) {
        return axios({
            method: 'post',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static postUrl<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'post',
            url: reqUrl + url + '?' + data.type + '=' + data.data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static postParam<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'post',
            url: reqUrl + url,
            params: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static delete<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'delete',
            url: reqUrl + url,
            params: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }
    static deleteByData<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'delete',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static put<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'put',
            url: reqUrl + url,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static postForm<R>(url: string, data?: FormData) {
        return axios<R>({
            method: 'post',
            url: reqUrl + url,
            params: data,
            data: data,
            headers: {
                'Content-Type': 'multipart/form-data',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static getWord<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'get',
            url: reqUrl + url,
            params: data,
            data: data,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }
    static getForm<T extends Record<string, any>, D = any, R = Response<D>>(url: string, data?: T) {
        return axios<R>({
            method: 'post',
            url: reqUrl + url,
            params: data,
            data: data,
            headers: {
                'Content-Type': 'multipart/form-data',
                'Access-Control-Allow-Origin': reqUrl
            }
        });
    }

    static download<T extends Record<string, any>>(url: string, data?: T) {
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
}
export default BaseService;
