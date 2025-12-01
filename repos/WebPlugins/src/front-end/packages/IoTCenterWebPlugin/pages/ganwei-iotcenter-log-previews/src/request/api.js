/**
 * api模块接口列表
 */
import apiFunction from 'gw-base-api-plus/apiFunction';
import LogPreview from './api/LogPreview';

const api = Object.assign(
    {},
    apiFunction,
    LogPreview
);

export default api;
