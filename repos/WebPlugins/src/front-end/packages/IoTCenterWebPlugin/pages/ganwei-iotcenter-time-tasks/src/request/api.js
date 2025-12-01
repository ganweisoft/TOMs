/**
 * api模块接口列表
 */
import apiFunction from "gw-base-api-plus/apiFunction";
import TimeTask from "./api/TimeTask";

const api = Object.assign({}, apiFunction, TimeTask);

export default api;
