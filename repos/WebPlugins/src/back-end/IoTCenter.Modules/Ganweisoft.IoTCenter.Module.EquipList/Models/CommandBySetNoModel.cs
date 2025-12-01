// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class CommandBySetNoModel
{
    public int EquipNo { get; set; }

    public int SetNo { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "设置类型不能为空")]
    public string SetType { get; set; }

    public string Value { get; set; }
}
