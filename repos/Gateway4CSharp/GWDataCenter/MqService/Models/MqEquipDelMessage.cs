using System.Collections.Generic;

namespace GWDataCenter.MqService;

public class MqEquipDelMessage
{
    public string Flow { get; set; }
    public int FlowType { get; set; }
    public List<int> EquipNos { get; set; }
}