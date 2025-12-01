// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EquipycyxListRequest : QueryRequest
{
    public int StaN { get; set; }
    public int EquipNo { get; set; }

    public string SearchName { get; set; }
}
