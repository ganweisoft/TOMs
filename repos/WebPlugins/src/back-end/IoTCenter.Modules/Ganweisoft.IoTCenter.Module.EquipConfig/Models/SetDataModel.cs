// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public partial class SetDataModel
{
    public int StaNo { get; set; } = 1;
    [Required]
    public int EquipNo { get; set; }
    [Required]
    public int SetNo { get; set; }
    public string SetNm { get; set; }
    [Required(AllowEmptyStrings =false,ErrorMessage ="设置类型不能为空")]
    public string SetType { get; set; }
    public string MainInstruction { get; set; }
    public string MinorInstruction { get; set; }
    public bool Record { get; set; }
    [Required(AllowEmptyStrings =false)]
    public string Action { get; set; }
    public string Value { get; set; }
    public bool Canexecution { get; set; }
    public string VoiceKeys { get; set; }
    public bool EnableVoice { get; set; }
    public int QrEquipNo { get; set; }
    public string SetCode { get; set; }
}
