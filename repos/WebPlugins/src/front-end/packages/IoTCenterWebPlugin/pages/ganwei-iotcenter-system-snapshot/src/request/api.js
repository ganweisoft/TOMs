/**
 * api模块接口列表
 */
import apiFunction from 'gw-base-api-plus/apiFunction';
import RealTime from './api/RealTime';
import Video from './api/Video';

const api = Object.assign(
    {},
    apiFunction,
    RealTime,
    Video
);

export default api;
