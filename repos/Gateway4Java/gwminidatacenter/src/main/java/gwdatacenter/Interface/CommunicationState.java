package gwdatacenter.Interface;

public enum CommunicationState
{
	/** 
	 通讯失败
	*/
	fail,
	/** 
	 通讯成功
	*/
	ok,
	/** 
	 因为有设置而中断数据采集
	*/
	setreturn,
	/** 
	 通讯重试中
	*/
	retry;

	public static final int SIZE = Integer.SIZE;

	public int getValue()
	{
		return this.ordinal();
	}

	public static CommunicationState forValue(int value)
	{
		return values()[value];
	}
}