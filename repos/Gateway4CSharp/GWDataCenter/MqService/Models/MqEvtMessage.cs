using System;
using System.Collections.Generic;

namespace GWDataCenter.MqService;

public class MqEvtMessage
{
    public string Time { get; set; }
    public string Flow { get; set; }
    public List<EquipEvent> EventItems { get; set; } = [];
}

public class EquipEvent
{
    public int DeviceId { get; set; }
    public List<EquipEventItem> EquipEvents { get; set; } = [];
}

public class EquipEventItem
{
    /// <summary>
    /// 显示到实时快照的内容
    /// </summary>
    public string Msg { get; set; }
    
    /// <summary>
    /// 联动传入的内容，如果为空就传入msg
    /// </summary>
    public string Msg4Linkage { get; set; }
    public MessageLevel Level { get; set; }
    public DateTime OccurDateTime { get; set; }
    public int EquipNo { get; set; }
}