// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public partial class YxpDataModel
{
    public int StaNo { get; set; } = 1;
    [Required]
    public int EquipNo { get; set; }
    [Required]
    public int YxNo { get; set; }
    public string YxNm { get; set; }
    public string ProcAdviceR { get; set; }
    public string ProcAdviceD { get; set; }
    [Required]
    public int LevelR { get; set; }
    [Required]
    public int LevelD { get; set; }
    public string Evt01 { get; set; }
    public string Evt10 { get; set; }
    public string MainInstruction { get; set; }
    public string MinorInstruction { get; set; }
    public string SafeBgn { get; set; }
    public string SafeEnd { get; set; }
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
    public int AlarmRiseCycle { get; set; }
    public string RelatedVideo { get; set; }
    public string ZiChanId { get; set; }
    public string PlanNo { get; set; }
    public string SafeTime { get; set; }
    public bool CurveRcd { get; set; }

    public bool IsGenerateWO { get; set; }

    public string YxCode { get; set; }

    public string DataType { get; set; }
}
