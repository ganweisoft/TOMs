// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public class RealTimeEventList
{
#pragma warning disable CA1720 // 标识符包含类型名称
    public string GUID { get; set; }
#pragma warning restore CA1720 // 标识符包含类型名称
    public string RelatedVideo { get; set; }
    public string ZiChanID { get; set; }
public string ZiChanName{ get; set; }
    public string PlanNo { get; set; }
    public bool bConfirmed { get; set; }
    public DateTime Time { get; set; }
    public int Ycyxno { get; set; }
    public string Type { get; set; }
    public int Equipno { get; set; }
    public string EquipName { get; set; }
    public string ProcAdviceMsg { get; set; }
    public string RelatedPic { get; set; }
    public string UserConfirm { get; set; }
    public string EventMsg { get; set; }
    public int Level { get; set; }
    public string Wavefile { get; set; }
    public DateTime DTConfirm { get; set; }
    public long TimeID { get; set; }
}
