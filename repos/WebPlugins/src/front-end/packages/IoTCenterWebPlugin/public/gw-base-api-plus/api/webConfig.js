// 获取共用config.json配置接口 多模块使用到

export default {
    getWebConfigData (){
        return this.get('/IoT/api/v3/Frontconfiguration/GetFrontconfigurationData');
    },
}