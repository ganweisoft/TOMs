using System.Collections.Generic;

namespace GWDataCenter.MqService;

public class MqRtStateMessage
{
    public string Time { get; set; }
    public string Flow { get; set; }
    public List<StateItem> StateItems { get; set; } = [];
}
public class StateItem
{
    public int DeviceId { get; set; }
    public string State { get; set; }
}