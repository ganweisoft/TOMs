
/**
 * api模块接口列表
 */
import apiFunction from '@components/@ganwei-pc/gw-base-api-plus/apiFunction';
import login from './api/login';
const api = Object.assign(
    {},
    apiFunction,
    login
);

export default api;
