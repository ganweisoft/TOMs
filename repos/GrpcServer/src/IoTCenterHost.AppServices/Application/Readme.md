---
author: 邹锭  
createFor: 应用服务层代码  
createAt: 创建于2019年10月10日  
usage: 应用服务对象主要作为操作的黏合剂，不涉及与数据操作和第三方接口操作的核心逻辑。  
---  
* Application *  
|-- Application : 应用层 （定义对外提供接口的外观，负责组装领域逻辑）  
|   |-- Interfaces : 接口  
|   |-- AccountAppServiceImpl.cs : 账户操作用例  
|   |-- AlarmCenterAppServiceImpl.cs : 综合类型  
|   |-- AlarmEventAppServiceImpl.cs : 事件  
|   |-- BaseAppServiceImpl : 应用层抽象方法  
|   |-- CapitalAppServiceImpl.cs : 资产  
|   |-- CommandAppServiceImpl.cs : 命令  
|   |-- CurveAppServiceImpl.cs : 实时曲线  
|   |-- DatabaseProviderAppServiceImpl.cs : 数据操作方法  
|   |-- DataCenterAppServiceImpl.cs : 其他  
|   |-- EquipAlarmAppServiceImpl.cs : 设备告警提醒  
|   |-- EquipBaseAppServiceImpl.cs : 设备基础方法  
|   |-- GWExProcAppServiceImpl.cs : 扩展方法  
|   |-- HotStandbyAppServiceImpl.cs : 双机热备  
|   |-- MessageAppServiceImpl.cs : 消息  
|   |-- NotificationAppServiceImpl.cs : 提醒  
|   |-- PlanAppServiceImpl.cs : 预案  
|   |-- QrCodeAppServiceImpl.cs : 二维码  
|   |-- RealDataAppServiceImpl.cs : 实时数据  
|   |-- SystemSupportAppServiceImpl.cs : 系统接口  
|   |-- VoiceAppServiceImpl.cs : 语音  
|   |-- YCAppServiceImpl.cs : 遥测点  
|   |-- YXAppServiceImpl.cs : 遥信点  