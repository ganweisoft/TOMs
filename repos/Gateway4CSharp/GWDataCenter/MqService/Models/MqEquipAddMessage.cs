using System.Collections.Generic;

namespace GWDataCenter.MqService;

public class MqEquipAddMessage
{
    public string Flow { get; set; }
    public int FlowType { get; set; }
    public List<Equip> Equips { get; set; }
}