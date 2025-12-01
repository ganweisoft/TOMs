package gwdatacenter.mqservice;

import gwdatacenter.*;

public class MqttTopic
{
	/** 
	 不同运行时环境设备总集消息
	*/
	public static String getTopicIotsysEquipDown()
	{
		return "$sys/iotcenter/+/java/down";
	}

	/** 
	 设置命令下发主题
	*/
	public static String TopicIotsysCommandDown = "$sys/iotcenter/+/command/down";

	/** 
	 Yc数据上报
	*/
	public static final String TOPIC_IOTSYS_MINIDCYC_DEVICE_DATA_REPORT = "$sys/iotcenter/%1$s/minidc/yc/up";

	/** 
	 Yx数据上报
	*/
	public static final String TOPIC_IOTSYS_MINIDCYX_DEVICE_DATA_REPORT = "$sys/iotcenter/%1$s/minidc/yx/up";

	/** 
	 Yx数据上报
	*/
	public static final String TOPIC_IOTSYS_MINIDCYX_DEVICE_EVT_REPORT = "$sys/iotcenter/%1$s/minidc/evt/up";

	/** 
	 状态上报
	*/
	public static final String TOPIC_IOTSYS_DEVICE_STATE_REPORT = "$sys/iotcenter/%1$s/statereport/up";
}