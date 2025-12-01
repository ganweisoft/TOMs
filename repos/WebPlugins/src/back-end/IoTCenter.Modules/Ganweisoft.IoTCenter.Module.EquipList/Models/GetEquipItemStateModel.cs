// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System.Text.Json.Serialization;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class GetEquipItemStateModel : QueryRequest
{
    public int EquipNo { get; set; }
    public string SearchName { get; set; }

    [JsonIgnore]
    public bool IsGetAll { get; set; }
}
