// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace GWModbusStandard.STD;

/// <summary>
/// 定义驱动的事件模型
/// </summary>
public class EquipEventModel
{
    /// <summary>
    /// 事件Id
    /// </summary>
    public string EventId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 事件名称：人员通行
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// 事件类型：PersonnelAccess
    /// </summary>
    public string EventCode { get; set; }

    /// <summary>
    /// 事件内容
    /// </summary>
    public string EventMsg { get; set; }

    /// <summary>
    /// 本条事件所记录的时间
    /// </summary>
    public DateTime EventTime { get; set; }
}
