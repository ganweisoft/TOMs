// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public class RealTimePageModel : QueryRequest
{
    public string EventType { get; set; }

    public string EventName { get; set; }

    public bool? Confirmed { get; set; }

    public string Guid { get; set; }
    public string MaxRecordId { get; set; }

    public string LastRecordId { get; set; }
}

public class RealTimeFilterPageModel : RealTimePageModel
{
    public int[] Equips { get; set; }
    public int[] Level { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
