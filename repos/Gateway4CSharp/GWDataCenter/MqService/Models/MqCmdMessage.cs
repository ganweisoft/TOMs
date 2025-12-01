namespace GWDataCenter.MqService;

public class MqCmdMessage
{
    public string RequestId { get; set; }
    public string LoginName { get; set; }
    public string Flow { get; set; }
    public string MainInstruct { get; set; }
    public string MinorInstruct { get; set; }
    public string Value { get; set; }
    public int GatewayId { get; set; }
    /// <summary>
    /// 下级平台设备编号
    /// </summary>
    public int EquipNo { get; set; }
    public int SetNo { get; set; }
    /// <summary>
    /// 下级平台的终端唯一标识
    /// </summary>
    public int TerminalIdentityId { get; set; }
}