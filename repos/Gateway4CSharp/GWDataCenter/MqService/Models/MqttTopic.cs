namespace GWDataCenter.MqService;

public class MqttTopic
{
    /// <summary>
    /// 不同运行时环境设备总集消息
    /// </summary>
    public static string TopicIotsysEquipDown => "$sys/iotcenter/+/csharp/down";
    
    /// <summary>
    /// 设置命令下发主题
    /// </summary>
    public static string TopicIotsysCommandDown = "$sys/iotcenter/+/command/down";
    
    /// <summary>
    /// 设备新增命令下发
    /// </summary>
    public static string TopicIotsysEquipAddDown = "$sys/iotcenter/+/eqpadd/down";
    
    /// <summary>
    /// 设备删除命令下发
    /// </summary>
    public static string TopicIotsysEquipDeleteDown = "$sys/iotcenter/+/eqpdel/down";

    /// <summary>
    /// Yc数据上报
    /// </summary>
    public const string TOPIC_IOTSYS_MINIDCYC_DEVICE_DATA_REPORT = "$sys/iotcenter/{0}/minidc/yc/up";

    /// <summary>
    /// Yx数据上报
    /// </summary>
    public const string TOPIC_IOTSYS_MINIDCYX_DEVICE_DATA_REPORT = "$sys/iotcenter/{0}/minidc/yx/up";

    /// <summary>
    /// Yx数据上报
    /// </summary>
    public const string TOPIC_IOTSYS_MINIDCYX_DEVICE_EVT_REPORT = "$sys/iotcenter/{0}/minidc/evt/up";
    
    /// <summary>
    /// 状态上报
    /// </summary>
    public const string TOPIC_IOTSYS_DEVICE_STATE_REPORT = "$sys/iotcenter/{0}/statereport/up";
}