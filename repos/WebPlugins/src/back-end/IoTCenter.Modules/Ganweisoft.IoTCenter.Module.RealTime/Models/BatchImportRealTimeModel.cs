// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.ExcelHelper;
using System;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public class BatchImportRealTimeModel
{
    [Field(Name = "事件详情")]
    public string EventMsg { get; set; }
    [Field(Name = "事件时间")]
    public DateTime Time { get; set; }
    [Field(Name = "遥测遥信号")]
    public string Ycyxno { get; set; }
    [Field(Name = "遥测遥信类型")]
    public string Type { get; set; }
    [Field(Name = "设备号")]
    public string Equipno { get; set; }
    [Field(Name = "设备名称")]
    public string EquipName { get; set; }
    [Field(Name = "是否已确认")]
    public string bConfirmed { get; set; }
    [Field(Name = "确认人")]
    public string UserConfirm { get; set; }
    public int Level { get; set; }
    [Field(Name = "确认时间")]
    public string DTConfirm { get; set; }
    [Field(Name = "处理意见")]
    public string ProcAdviceMsg { get; set; }
    [Field(Name = "资产编号")]
    public string ZiChanID { get; set; }
    [Field(Name = "资产名称")]
    public string ZiChanName { get; set; }
    [Field(Name = "关联视频")]
    public string RelatedVideo { get; set; }
}