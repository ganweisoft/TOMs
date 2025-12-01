/**
 * api模块接口列表
 */
import apiFunction from 'gw-base-api-plus/apiFunction';
import Event from './api/Event';

const api = Object.assign(
    {},
    apiFunction,
    Event
);

export default api;
