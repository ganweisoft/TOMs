// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class GroupQueryRequest : QueryRequest
{
    public string SearchName { get; set; }
    
    public int GroupId { get; set; }
}