* IoTCenterHost.Proxy *
|-- Connect : 用于建立与服务端连接的类  
|   |--ConnectServiceImpl.cs : 连接服务   
|-- Proxies : 代理层（适配wcf服务中的所有的接口）
|   |-- AlarmCenterCallbackClientImpl.cs 适配wcf的通用回调类
|   |-- AlarmCenterDatabaseClientImpl.cs : 适配wcf的数据库操作类
|   |-- AlarmCenterExClientImpl.cs : 适配wcf的对应操作类 
|   |-- AlarmCenterServiceClientImpl.cs :   适配wcf的对应操作类 
|   |-- CallbackContractServiceClientImpl.cs : 适配wcf的对应操作类  
|   |-- MultiStationClientImpl.cs : 适配wcf的对应操作类 
|   |-- SingleStationCallbackClientImpl.cs : 适配wcf的对应操作类 
|-- ServiceImpl : 客户端服务实现     
|   |-- AlarmCenterServiceImpl : 通用操作类
|   |-- AlarmEventAppServiceImpl : 事件操作类  
|   |-- CapitalAppServiceImpl : 资产操作类  
|   |-- CommandAppServiceImpl.cs : 命令操作类  
|   |-- CurveAppServiceImpl.cs : 实时曲线  
|   |-- DatabaseProviderAppServiceImpl.cs : 数据库上下文  
|   |-- DataCenterAppServiceImpl.cs : 数据其他操作类
|   |-- EquipAlarmAppServiceImpl.cs : 实时告警  
|   |-- EquipBaseAppServiceImpl.cs : 设备基本信息  
|   |-- GWExProcAppServiceImpl.cs : 扩展操作   
|   |-- HotStandbyAppServiceImpl.cs : 双机热备   
|   |-- MessageAppServiceImpl.cs : 短信 
|   |-- NotificationAppServiceImpl.cs : 系统提醒   
|   |-- PlanAppServiceImpl.cs : 预案   
|   |-- QrCodeAppServiceImpl.cs : 二维码   
|   |-- RealDataAppServiceImpl.cs : 实时数据   
|   |-- SystemSupportAppServiceImpl.cs : 系统支持   
|   |-- VoiceAppServiceImpl.cs : 语音控制   
|   |-- YCAppServiceImpl.cs : 遥测    
|   |-- YXAppServiceImpl.cs : 遥信   
|-- Startup : 二维码相关  
|   |-- GrpcClientConfiguration.cs : 初始化配置grpc客户端   
|   |-- GrpcConvertExtension.cs : gRPC模型转换   
|   |-- MappingProfile.cs : AutoMap对象匹配       
|-- IotCenterExtension.cs : IOC对象注册

