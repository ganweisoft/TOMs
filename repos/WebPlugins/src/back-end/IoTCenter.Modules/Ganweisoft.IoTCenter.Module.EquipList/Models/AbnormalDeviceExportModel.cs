// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.ExcelHelper;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class AbnormalDeviceExportModel
{
    [Field(Name = "站点号")]
    public int StaN { get; set; }

    [Field(Name = "设备号")]
    public int EquipNo { get; set; }

    [Field(Name = "设备名称")]
    public string EquipNm { get; set; }

    [Field(Name = "产品Id")]
    public int RawEquipNo { get; set; }

    [Field(Name = "设备状态")]
    public string EquipStatus { get; set; } = "-";

    [Field(Name = "设备属性")]
    public string EquipDetail { get; set; }

    [Field(Name = "通讯刷新周期")]
    public int? AccCyc { get; set; }

    [Field(Name = "关联页面")]
    public string RelatedPic { get; set; }

    [Field(Name = "通讯故障处理意见")]
    public string ProcAdvice { get; set; }

    [Field(Name = "故障信息")]
    public string OutOfContact { get; set; }

    [Field(Name = "故障恢复提示")]
    public string Contacted { get; set; }

    [Field(Name = "报警声音文件")]
    public string EventWav { get; set; }

    [Field(Name = "驱动文件")]
    public string CommunicationDrv { get; set; }

    [Field(Name = "通讯端口")]
    public string LocalAddr { get; set; }

    [Field(Name = "设备地址")]
    public string EquipAddr { get; set; }

    [Field(Name = "通讯参数")]
    public string CommunicationParam { get; set; }

    [Field(Name = "通讯时间参数")]
    public string CommunicationTimeParam { get; set; }

    [Field(Name = "报警方式")]
    public int AlarmScheme { get; set; }

    [Field(Name = "报警升级周期（分钟）")]
    public int AlarmRiseCycle { get; set; }

    [Field(Name = "关联视频")]
    public string RelatedVideo { get; set; }

    [Field(Name = "资产编号")]
    public string ZiChanID { get; set; }

    [Field(Name = "预案号")]
    public string PlanNo { get; set; }

    [Field(Name = "安全时段")]
    public string SafeTime { get; set; }

    [Field(Name = "设备分组")]
    public string GroupName { get; set; }
}

public class AbnormalDeviceYcpExportModel
{
    [Field(Name = "站点号")]
    public int StaNo { get; set; }

    [Field(Name = "设备号")]
    public int EquipNo { get; set; }

    [Field(Name = "模拟量号")]
    public int YcNo { get; set; }

    [Field(Name = "模拟量名称")]
    public string YcNm { get; set; }

    [Field(Name = "比例变换")]
    public int Mapping { get; set; }

    [Field(Name = "最小值")]
    public double YcMin { get; set; }

    [Field(Name = "最大值")]
    public double YcMax { get; set; }

    [Field(Name = "下限值")]
    public double ValMin { get; set; }

    [Field(Name = "回复下限值")]
    public double RestoreMin { get; set; }

    [Field(Name = "回复上限值")]
    public double RestoreMax { get; set; }

    [Field(Name = "上限值")]
    public double ValMax { get; set; }

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

    [Field(Name = "报警级别")]
    public int LvlLevel { get; set; }

    [Field(Name = "越上限事件")]
    public string OutmaxEvt { get; set; }

    [Field(Name = "越下限事件")]
    public string OutminEvt { get; set; }

    [Field(Name = "报警方式")]
    public int AlarmScheme { get; set; }

    [Field(Name = "曲线记录")]
    public int CurveRcd { get; set; }

    [Field(Name = "曲线记录阈值")]
    public double CurveLimit { get; set; }

    [Field(Name = "单位")]
    public string Unit { get; set; }

    [Field(Name = "报警升级周期")]
    public int AlarmRiseCycle { get; set; }

    [Field(Name = "资产编号")]
    public string ZiChanId { get; set; }

    [Field(Name = "预案号")]
    public string PlanNo { get; set; }

    [Field(Name = "安全时段")]
    public string SafeTime { get; set; }

    [Field(Name = "遥测编码")]
    public string YC_Code { get; set; }
}

public class AbnormalDeviceYxpExportModel
{
    [Field(Name = "站点号")]
    public int StaNo { get; set; }

    [Field(Name = "设备号")]
    public int EquipNo { get; set; }

    [Field(Name = "状态量号")]
    public int YxNo { get; set; }

    [Field(Name = "状态量名称")]
    public string YxNm { get; set; }

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

    [Field(Name = "报警方式")]
    public int AlarmScheme { get; set; }

    [Field(Name = "报警升级周期")]
    public int AlarmRiseCycle { get; set; }

    [Field(Name = "资产编号")]
    public string ZiChanId { get; set; }

    [Field(Name = "预案号")]
    public string PlanNo { get; set; }

    [Field(Name = "安全时段")]
    public string SafeTime { get; set; }

    [Field(Name = "遥信编码")]
    public string YX_Code { get; set; }
}

public class AbnormalDeviceSetParmExportModel
{
    [Field(Name = "站点号")]
    public int StaNo { get; set; }

    [Field(Name = "设备号")]
    public int EquipNo { get; set; }

    [Field(Name = "设置号")]
    public int SetNo { get; set; }

    [Field(Name = "设置名称")]
    public string SetNm { get; set; }

    [Field(Name = "设置类型")]
    public string SetType { get; set; }

    [Field(Name = "操作命令")]
    public string MainInstruction { get; set; }

    [Field(Name = "操作参数")]
    public string MinorInstruction { get; set; }

    [Field(Name = "设置编码")]
    public string Set_Code { get; set; }


}
