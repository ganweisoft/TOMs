// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class YcpResponesModel
{
    public int StaN { get; set; }

    public int EquipNo { get; set; }

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
    public string YcCode { get; set; }

    public string DataType { get; set; }

}
