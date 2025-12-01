// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class SetParmByEquipNosResponse
{
    public int equipNo { get; set; }
    public IEnumerable<SetParmModel> SetParmList { get; set; }
}

public class SetParmModel
{
    [Required]
    public int StaN { get; set; }

    [Required]
    public int EquipNo { get; set; }

    [Required]
    public int SetNo { get; set; }
    public string SetNm { get; set; }

    [Required]
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
    public string Set_Code { get; set; }
}