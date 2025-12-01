// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class GetDataFromCurveRequest
{
    public List<string> DTList { get; set; }
    public int StaNo { get; set; }
    public int EqpNo { get; set; }
    public int YcYxNo { get; set; }
    public string type { get; set; }
}