/**
 * api模块接口列表
 */
import apiFunction from './apiFunction';
import Auth from './api/Auth';
import EquipList from './api/EquipList';
import RealTime from './api/RealTime';
import SystemConfig from './api/SystemConfig';
import SecuritySetting from './api/SecuritySetting';
import DataCollection from './api/DataCollection';
import MultipleAccess from './api/MultipleAccess';
import DataBackup from './api/DataBackup';
import GwEcharts from './api/GwEcharts';
import ConfigManage from './api/ConfigManage';
import EquipPlatform from './api/EquipPlatform';
import planManage from './api/PlanManage';
import Iam from './api/Iam'
import language from './api/language'
import webConfig from './api/webConfig';

const api = Object.assign(
    {},
    apiFunction,
    Auth,
    EquipList,
    Event,
    RealTime,
    SystemConfig,
    DataCollection,
    MultipleAccess,
    DataBackup,
    GwEcharts,
    ConfigManage,
    EquipPlatform,
    planManage,
    SecuritySetting,
    ConfigManage,
    Iam,
    language,
    webConfig
);

export default api;
