/**
 * api模块接口列表
 */
import apiFunction from 'gw-base-api-plus/apiFunction';
import commonApi from './api/commonApi';
import equipInfo from './api/equipInfo';
import templateManage from './api/templateManage';

const api = Object.assign(
    {},
    apiFunction,
    commonApi,
    equipInfo,
    templateManage

);

export default api;
