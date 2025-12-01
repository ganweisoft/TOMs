// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class ExportHistoryCuresModelInternal
{
    public DateTime BeginTime { get; set; }
    public DateTime EndTime { get; set; }

    public string EquipNos { get; set; }

    public int StaNo { get; set; } = 1;

    public List<YcHistoryCureEquipModel> YcHistory { get; set; }
    public List<YxHistoryCureEquipModel> YxHistory { get; set; }
}

public class ExportEquip
{
    public int StaNo { get; set; }

    public int EquipNo { get; set; }

    public int[] Ycps { get; set; } = Array.Empty<int>();

    public int[] Yxps { get; set; } = Array.Empty<int>();
}

public class ExportHistoryCuresModel
{
    [Required]
    [DataType(DataType.DateTime, ErrorMessage = "开始日期格式错误")]
    public DateTime BeginTime { get; set; }
    [Required]
    [DataType(DataType.DateTime, ErrorMessage = "结束日期格式错误")]
    public DateTime EndTime { get; set; }

    public ExportEquip[] ExportEquips { get; set; }

    public bool IsMerge { get; set; }
}

public class YcHistoryCureEquipModel
{
    public int EquipNo { get; set; }
    public string EquipName { get; set; }
    public int YcpNo { get; set; }
    public string YcpName { get; set; }
}

public class YxHistoryCureEquipModel
{
    public int EquipNo { get; set; }
    public string EquipName { get; set; }
    public int YxpNo { get; set; }
    public string YxpName { get; set; }
}

public class YcYxHistoryCure
{
    public string DateTime { get; set; }
    public double Value { get; set; }
}
