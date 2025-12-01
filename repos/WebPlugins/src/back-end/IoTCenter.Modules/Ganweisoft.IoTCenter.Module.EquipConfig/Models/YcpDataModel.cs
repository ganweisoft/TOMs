// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class YcpDataModel
{
    public int StaNo { get; set; } = 1;
    [Required]
    public int EquipNo { get; set; }
    [Required]
    public int YcNo { get; set; }
    public string YcNm { get; set; }
    public bool Mapping { get; set; }
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
    public string SafeBgn { get; set; }
    public string SafeEnd { get; set; }
    public int AlarmAcceptableTime { get; set; }
    public int RestoreAcceptableTime { get; set; }
    public int AlarmRepeatTime { get; set; }
    public string ProcAdvice { get; set; }
    [Required]
    public int LvlLevel { get; set; }
    public string OutminEvt { get; set; }
    public string OutmaxEvt { get; set; }
    public string WaveFile { get; set; }
    public string RelatedPic { get; set; }
    public int AlarmScheme { get; set; }
    public bool CurveRcd { get; set; }
    public double CurveLimit { get; set; }
    public string AlarmShield { get; set; }
    public string Unit { get; set; }
    public int AlarmRiseCycle { get; set; }
    public string RelatedVideo { get; set; }
    public string ZiChanId { get; set; }
    public string PlanNo { get; set; }
    public string SafeTime { get; set; }

    public bool IsGenerateWO { get; set; }

    public string YcCode { get; set; }

    public string DataType { get; set; }
}
