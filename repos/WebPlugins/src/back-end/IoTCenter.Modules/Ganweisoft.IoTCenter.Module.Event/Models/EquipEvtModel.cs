// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;

namespace Ganweisoft.IoTCenter.Module.Event;

public class EquipEvtModel : QueryRequest
{

    public string BeginTime { get; set; }

    public string EndTime { get; set; }

    public string EquipNos { get; set; }

    public string EventType { get; set; }

    public string Sort { get; set; }

    public string EventName { get; set; }
}
