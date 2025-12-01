/**
 * api模块接口列表
 */
import apiFunction from 'gw-base-api-plus/apiFunction';
import equipList from './api/equipList.js';

const api = Object.assign(
    {},
    apiFunction,
    equipList
);

export default api;
