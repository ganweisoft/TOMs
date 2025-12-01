using System;
using System.Collections.Generic;

namespace GWDataCenter.MqService;

public class MqRtValueMessage
{
    public int DataType { get; set; }
    public string Time { get; set; }
    public string Flow { get; set; }
    public List<DataItem> DataItems { get; set; }
}

public class DataItem
{
    public int DeviceId { get; set; }
    public Dictionary<int, object> Attribute { get; set; } = new();
}
