// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class GetEquipDataListModel : QueryRequest
{
    public string EquipName { get; set; }

    public List<int> EquipNos { get; set; } = new List<int>();
}
