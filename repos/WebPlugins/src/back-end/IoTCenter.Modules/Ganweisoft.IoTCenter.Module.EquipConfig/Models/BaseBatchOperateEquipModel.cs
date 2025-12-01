// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;


public class BaseBatchOperateEquipModel
{
    public int StaN { get; set; } = 1;
    public List<int> Ids { get; set; } = new List<int>();
}
public class BatchOperateEquipModel : BaseBatchOperateEquipModel
{
    public Dictionary<string, object> Dicts { get; set; } = new Dictionary<string, object>();
}
