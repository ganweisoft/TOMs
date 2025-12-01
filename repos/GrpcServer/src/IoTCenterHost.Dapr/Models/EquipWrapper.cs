//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Dapr.Models;

public class RuntimeEquipSyncEvent
{
    public string Flow { get; set; }
    public int FlowType { get; set; }
    public string AppInstanceId { get; set; }
    public List<EquipWrapper> Equips { get; set; }
}
public class EquipWrapper
{
    public int EquipNo { get; set; }
    public int StaN { get; set; }
    public string EquipNm { get; set; }
    public string EquipDetail { get; set; }
    public int AccCyc { get; set; }
    public string RelatedPic { get; set; }
    public string ProcAdvice { get; set; }
    public string OutOfContact { get; set; }
    public string Contacted { get; set; }
    public string EventWav { get; set; }
    public string CommunicationDrv { get; set; }
    public string LocalAddr { get; set; }
    public string EquipAddr { get; set; }
    public string CommunicationParam { get; set; }
    public string CommunicationTimeParam { get; set; }
    public int RawEquipNo { get; set; }
    public string Tabname { get; set; }
    public int AlarmScheme { get; set; }
    public int Attrib { get; set; }
    public string StaIp { get; set; }
    public int? AlarmRiseCycle { get; set; }
    public string Reserve1 { get; set; }
    public string Reserve2 { get; set; }
    public string Reserve3 { get; set; }
    public string RelatedVideo { get; set; }
    public string ZiChanId { get; set; }
    public string PlanNo { get; set; }
    public string SafeTime { get; set; }
    public string Backup { get; set; }
    public List<YcpWrapper> Ycps { get; set; } = new List<YcpWrapper>();
    public List<YxpWrapper> Yxps { get; set; } = new List<YxpWrapper>();
    public List<SetWrapper> SetParms { get; set; } = new List<SetWrapper>();
}

public class YcpWrapper
{
    public int YcNo { get; set; }
    public int StaN { get; set; }
    public int EquipNo { get; set; }
    public string YcNm { get; set; }
    public bool? Mapping { get; set; }
    public double YcMin { get; set; }
    public double YcMax { get; set; }
    public double PhysicMin { get; set; }
    public double PhysicMax { get; set; }
    public double ValMin { get; set; }
    public double RestoreMin { get; set; }
    public double RestoreMax { get; set; }
    public double ValMax { get; set; }
    public int ValTrait { get; set; }
    public string MainInstruction { get; set; }
    public string MinorInstruction { get; set; }
    public int AlarmAcceptableTime { get; set; }
    public int RestoreAcceptableTime { get; set; }
    public int AlarmRepeatTime { get; set; }
    public string ProcAdvice { get; set; }
    public int LvlLevel { get; set; }
    public string OutminEvt { get; set; }
    public string OutmaxEvt { get; set; }
    public string WaveFile { get; set; }
    public string RelatedPic { get; set; }
    public int AlarmScheme { get; set; }
    public bool CurveRcd { get; set; }
    public double? CurveLimit { get; set; }
    public string AlarmShield { get; set; }
    public string Unit { get; set; }
    public int? AlarmRiseCycle { get; set; }
    public string Reserve1 { get; set; }
    public string Reserve2 { get; set; }
    public string Reserve3 { get; set; }
    public string RelatedVideo { get; set; }
    public string ZiChanId { get; set; }
    public string PlanNo { get; set; }
    public string SafeTime { get; set; }
    public DateTime? SafeBgn { get; set; }
    public DateTime? SafeEnd { get; set; }
    public string GWValue { get; set; }
    public DateTime? GWTime { get; set; }
    public string DataType { get; set; }
    public string YcCode { get; set; }
}

public class YxpWrapper
{
    public int YxNo { get; set; }
    public int StaN { get; set; }
    public int EquipNo { get; set; }
    public string YxNm { get; set; }
    public string ProcAdviceR { get; set; }
    public string ProcAdviceD { get; set; }
    public int LevelR { get; set; }
    public int LevelD { get; set; }
    public string Evt01 { get; set; }
    public string Evt10 { get; set; }
    public string MainInstruction { get; set; }
    public string MinorInstruction { get; set; }
    public int AlarmAcceptableTime { get; set; }
    public int RestoreAcceptableTime { get; set; }
    public int AlarmRepeatTime { get; set; }
    public string WaveFile { get; set; }
    public string RelatedPic { get; set; }
    public int AlarmScheme { get; set; }
    public bool Inversion { get; set; }
    public int Initval { get; set; }
    public int ValTrait { get; set; }
    public string AlarmShield { get; set; }
    public int? AlarmRiseCycle { get; set; }
    public string RelatedVideo { get; set; }
    public string ZiChanId { get; set; }
    public string PlanNo { get; set; }
    public string SafeTime { get; set; }
    public bool CurveRcd { get; set; }
    public string Reserve1 { get; set; }
    public string Reserve2 { get; set; }
    public string Reserve3 { get; set; }
    public DateTime? SafeBgn { get; set; }
    public DateTime? SafeEnd { get; set; }
    public string GWValue { get; set; }
    public DateTime? GWTime { get; set; }
    public string DataType { get; set; }
    public string YxCode { get; set; }
}

public class SetWrapper
{
    public int SetNo { get; set; }
    public int StaN { get; set; }
    public int EquipNo { get; set; }
    public string SetNm { get; set; }
    public string SetType { get; set; }
    public string MainInstruction { get; set; }
    public string MinorInstruction { get; set; }
    public bool Record { get; set; }
    public string Action { get; set; }
    public string Value { get; set; }
    public bool Canexecution { get; set; }
    public string VoiceKeys { get; set; }
    public bool EnableVoice { get; set; }
    public int QrEquipNo { get; set; }
    public string Reserve1 { get; set; }
    public string Reserve2 { get; set; }
    public string Reserve3 { get; set; }
    public string SetCode { get; set; }
}