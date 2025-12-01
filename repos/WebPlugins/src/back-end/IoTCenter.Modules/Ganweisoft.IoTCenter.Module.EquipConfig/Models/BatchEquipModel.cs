// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.ExcelHelper;
using Newtonsoft.Json;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class BatchEquipModel
{
    [JsonProperty("equip_no")]
    public int EquipNo { get; set; }

    [JsonProperty("equip_nm")]
    [Field(Name = "设备名称")]
    public string EquipName { get; set; }

    [JsonProperty("equip_detail")]
    [Field(Name = "设备属性")]
    public string EquipDetail { get; set; }

    [JsonProperty("equip_addr")]
    [Field(Name = "设备地址")]
    public string EquipAddr { get; set; }

    [JsonProperty("deviceType")]
    [Field(Name = "设备类型")]
    public string DeviceType { get; set; }

    [JsonProperty("out_of_contact")]
    [Field(Name = "故障信息")]
    public string OutOfContact { get; set; }

    [JsonProperty("communication_drv")]
    [Field(Name = "驱动文件")]
    public string CommunicationDrv { get; set; }

    [JsonProperty("acc_cyc")]
    [Field(Name = "通讯刷新周期")]
    public int? AccCyc { get; set; }

    [JsonProperty("communication_time_param")]
    [Field(Name = "通讯时间参数")]
    public string CommunicationTimeParam { get; set; }

    [JsonProperty("AlarmRiseCycle")]
    [Field(Name = "报警升级周期（分钟）")]
    public int AlarmRiseCycle { get; set; }

    [JsonProperty("attrib")]
    public int Attrib { get; set; }

    #region 附加信息

    [JsonProperty("related_pic")]
    [Field(Name = "关联页面")]
    public string RelatedPic { get; set; }

    [JsonProperty("related_video")]
    [Field(Name = "关联视频")]
    public string RelatedVideo { get; set; }

    [Field(Name = "显示报警")]
    public int ShowAlarm { get; set; }

    [Field(Name = "记录报警")]
    public int RecordAlarm { get; set; }

    [Field(Name = "短信报警")]
    public int Smslarm { get; set; }

    [Field(Name = "Email报警")]
    public int EmailAlarm { get; set; }

    [Field(Name = "资产编号")]
    public string ZiChanID { get; set; }

    [Field(Name = "预案号")]
    public string PlanNo { get; set; }

    [JsonProperty("alarm_scheme")]
    public int AlarmScheme { get; set; }

    [JsonProperty("backup")]
    [Field(Name = "双机热备")]
    public string Backup { get; set; }



    [JsonProperty("proc_advice")]
    [Field(Name = "通讯故障处理意见")]
    public string ProcAdvice { get; set; }

    [Field(Name = "故障恢复提示")]
    public string Contacted { get; set; }

    [JsonProperty("event_wav")]
    [Field(Name = "报警声音文件")]
    public string EventWav { get; set; }

    [JsonProperty("local_addr")]
    [Field(Name = "通讯端口")]
    public string LocalAddr { get; set; }

    [JsonProperty("communication_param")]
    [Field(Name = "通讯参数")]
    public string CommunicationParam { get; set; }

    [Field(Name = "安全时段")]
    public string SafeTime { get; set; }

    [JsonProperty("raw_equip_no")]
    [Field(Name = "模板设备号")]
    public int RawEquipNo { get; set; }

    [JsonProperty("tabname")]
    [Field(Name = "附表名称")]
    public string TabName { get; set; }

    [JsonProperty("sta_IP")]
    [Field(Name = "站点IP")]
    public string StaIp { get; set; }

    [Field(Name = "预留字段1")]
    public string Reserve1 { get; set; }

    [Field(Name = "预留字段2")]
    public string Reserve2 { get; set; }

    [Field(Name = "预留字段3")]
    public string Reserve3 { get; set; }

    [JsonProperty("sta_n")]
    public int StaN { get; set; } = 1;
    #endregion
}

public class BatchYcpModel
{
    [JsonProperty("sta_n")]
    public int StaNo { get; set; } = 1;

    [JsonProperty("equip_no")]
    public int EquipNo { get; set; }

    [JsonProperty("yc_no")]
    [Field(Name = "模拟量号")]
    public int YcNo { get; set; }

    [JsonProperty("yc_nm")]
    [Field(Name = "模拟量名称")]
    public string YcNm { get; set; }

    [JsonProperty("val_min")]
    [Field(Name = "下限值")]
    public double ValMin { get; set; }

    [JsonProperty("restore_min")]
    [Field(Name = "回复下限值")]
    public double RestoreMin { get; set; }

    [JsonProperty("restore_max")]
    [Field(Name = "回复上限值")]
    public double RestoreMax { get; set; }

    [JsonProperty("val_max")]
    [Field(Name = "上限值")]
    public double ValMax { get; set; }

    [JsonProperty("unit")]
    [Field(Name = "单位")]
    public string Unit { get; set; }

    [JsonProperty("related_pic")]
    [Field(Name = "关联页面")]
    public string RelatedPic { get; set; }

    [JsonProperty("related_video")]
    [Field(Name = "关联视频")]
    public string RelatedVideo { get; set; }

    [JsonProperty("ZiChanID")]
    [Field(Name = "资产编号")]
    public string ZiChanId { get; set; }

    [JsonProperty("PlanNo")]
    [Field(Name = "预案号")]
    public string PlanNo { get; set; }

    [JsonProperty("curve_rcd")]
    [Field(Name = "曲线记录")]
    public int CurveRcd { get; set; }

    [JsonProperty("curve_limit")]
    [Field(Name = "曲线记录阈值")]
    public double CurveLimit { get; set; }

    [JsonProperty("outmax_evt")]
    [Field(Name = "越上限事件")]
    public string OutmaxEvt { get; set; }

    [JsonProperty("outmin_evt")]
    [Field(Name = "越下限事件")]
    public string OutminEvt { get; set; }

    [Field(Name = "显示报警")]
    public int ShowAlarm { get; set; }

    [Field(Name = "记录报警")]
    public int RecordAlarm { get; set; }

    [Field(Name = "短信报警")]
    public int Smslarm { get; set; }

    [Field(Name = "Email报警")]
    public int EmailAlarm { get; set; }

    [Field(Name = "触发工单")]
    public int TriggerWorkOrder { get; set; }

    [JsonProperty("lvl_level")]
    [Field(Name = "报警级别")]
    public int LvlLevel { get; set; }

    [JsonProperty("val_trait")]
    [Field(Name = "属性值")]
    public int ValTrait { get; set; }

    [JsonProperty("mapping")]
    [Field(Name = "比例变换")]
    public int Mapping { get; set; }

    [JsonProperty("physic_min")]
    [Field(Name = "实测最小值")]
    public double PhysicMin { get; set; }

    [JsonProperty("physic_max")]
    [Field(Name = "实测最大值")]
    public double PhysicMax { get; set; }

    [JsonProperty("yc_min")]
    [Field(Name = "最小值")]
    public double YcMin { get; set; }

    [JsonProperty("yc_max")]
    [Field(Name = "最大值")]
    public double YcMax { get; set; }

    [JsonProperty("main_instruction")]
    [Field(Name = "操作命令")]
    public string MainInstruction { get; set; }

    [JsonProperty("minor_instruction")]
    [Field(Name = "操作参数")]
    public string MinorInstruction { get; set; }

    [JsonProperty("alarm_acceptable_time")]
    [Field(Name = "越线滞纳时间（秒）")]
    public int AlarmAcceptableTime { get; set; }

    [JsonProperty("restore_acceptable_time")]
    [Field(Name = "恢复滞纳时间（秒）")]
    public int RestoreAcceptableTime { get; set; }

    [JsonProperty("alarm_repeat_time")]
    [Field(Name = "重复报警时间（分钟）")]
    public int AlarmRepeatTime { get; set; }

    [JsonProperty("AlarmRiseCycle")]
    [Field(Name = "报警升级周期")]
    public int AlarmRiseCycle { get; set; }

    [JsonProperty("wave_file")]
    [Field(Name = "声音文件")]
    public string WaveFile { get; set; }

    [JsonProperty("alarm_shield")]
    [Field(Name = "报警屏蔽")]
    public string AlarmShield { get; set; }

    [Field(Name = "安全时段")]
    public string SafeTime { get; set; }

    [Field(Name = "预留字段1")]
    public string Reserve1 { get; set; }

    [Field(Name = "预留字段2")]
    public string Reserve2 { get; set; }

    [Field(Name = "预留字段3")]
    public string Reserve3 { get; set; }

    public int AlarmScheme { get; set; }
}


public class BatchYxpModel
{
    public int StaNo { get; set; } = 1;

    [JsonProperty("equip_no")]
    public int EquipNo { get; set; }

    [Field(Name = "状态量号")]
    public int YxNo { get; set; }

    [Field(Name = "状态量名称")]
    public string YxNm { get; set; }

    [Field(Name = "事件（0-1）")]
    public string Evt01 { get; set; }

    [Field(Name = "事件（1-0）")]
    public string Evt10 { get; set; }

    [Field(Name = "关联页面")]
    public string RelatedPic { get; set; }

    [Field(Name = "关联视频")]
    public string RelatedVideo { get; set; }

    [Field(Name = "资产编号")]
    public string ZiChanId { get; set; }

    [Field(Name = "预案号")]
    public string PlanNo { get; set; }

    [Field(Name = "曲线记录")]
    public int CurveRcd { get; set; }

    [Field(Name = "显示报警")]
    public int ShowAlarm { get; set; }

    [Field(Name = "记录报警")]
    public int RecordAlarm { get; set; }

    [Field(Name = "短信报警")]
    public int Smslarm { get; set; }

    [Field(Name = "Email报警")]
    public int EmailAlarm { get; set; }

    [Field(Name = "触发工单")]
    public int TriggerWorkOrder { get; set; }

    [Field(Name = "属性值")]
    public int ValTrait { get; set; }

    [Field(Name = "是否取反")]
    public int Inversion { get; set; }

    [Field(Name = "处理意见（0-1）")]
    public string ProcAdviceR { get; set; }

    [Field(Name = "处理意见（1-0）")]
    public string ProcAdviceD { get; set; }

    [Field(Name = "级别（0-1）")]
    public int LevelR { get; set; }

    [Field(Name = "级别（1-0）")]
    public int LevelD { get; set; }

    [Field(Name = "初始状态")]
    public int Initval { get; set; }

    [Field(Name = "操作命令")]
    public string MainInstruction { get; set; }

    [Field(Name = "操作参数")]
    public string MinorInstruction { get; set; }

    [Field(Name = "越线滞纳时间（秒）")]
    public int AlarmAcceptableTime { get; set; }

    [Field(Name = "恢复滞纳时间（秒）")]
    public int RestoreAcceptableTime { get; set; }

    [Field(Name = "重复报警时间（分钟）")]
    public int AlarmRepeatTime { get; set; }

    [Field(Name = "声音文件")]
    public string WaveFile { get; set; }

    [Field(Name = "报警屏蔽")]
    public string AlarmShield { get; set; }

    [Field(Name = "报警升级周期")]
    public int AlarmRiseCycle { get; set; }

    [Field(Name = "安全时段")]
    public string SafeTime { get; set; }

    [Field(Name = "预留字段1")]
    public string Reserve1 { get; set; }

    [Field(Name = "预留字段2")]
    public string Reserve2 { get; set; }

    [Field(Name = "预留字段3")]
    public string Reserve3 { get; set; }

    [JsonProperty("alarm_scheme")]
    public int AlarmScheme { get; set; }
}


public class BatchSetModel
{
    [JsonProperty("sta_n")]
    public int StaNo { get; set; } = 1;

    [JsonProperty("equip_no")]
    public int EquipNo { get; set; }

    [JsonProperty("set_no")]
    [Field(Name = "设置号")]
    public int SetNo { get; set; }

    [JsonProperty("set_nm")]
    [Field(Name = "设置名称")]
    public string SetNm { get; set; }

    [JsonProperty("value")]
    [Field(Name = "值")]
    public string Value { get; set; }

    [JsonProperty("set_type")]
    [Field(Name = "设置类型")]
    public string SetType { get; set; }

    [JsonProperty("action")]
    [Field(Name = "动作")]
    public string Action { get; set; }

    [JsonProperty("main_instruction")]
    [Field(Name = "操作命令")]
    public string MainInstruction { get; set; }

    [JsonProperty("minor_instruction")]
    [Field(Name = "操作参数")]
    public string MinorInstruction { get; set; }

    [JsonProperty("record")]
    [Field(Name = "记录")]
    public int Record { get; set; }

    [JsonProperty("VoiceKeys")]
    [Field(Name = "语音控制字符")]
    public string VoiceKeys { get; set; }

    [JsonProperty("EnableVoice")]
    [Field(Name = "是否语音控制")]
    public int EnableVoice { get; set; }

    [Field(Name = "预留字段1")]
    public string Reserve1 { get; set; }

    [Field(Name = "预留字段2")]
    public string Reserve2 { get; set; }

    [Field(Name = "预留字段3")]
    public string Reserve3 { get; set; }

    [Field(Name = "二维码")]
    [JsonProperty("qr_equip_no ")]
    public int? QrEquipNo { get; set; }
}
