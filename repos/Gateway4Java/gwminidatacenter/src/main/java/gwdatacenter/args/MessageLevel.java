package gwdatacenter.args;

public enum MessageLevel
{
	Wubao(-1),
	Info(0),
	SpecalInfo(9999),
	Debug(10000),
	SetParm(10001),
	ZiChan(10002),
	Warn(10003),
	Error(10004),
	Fatal(10005);

	public static final int SIZE = Integer.SIZE;

	private int intValue;
	private static java.util.HashMap<Integer, MessageLevel> mappings;
	private static java.util.HashMap<Integer, MessageLevel> getMappings()
	{
		if (mappings == null)
		{
			synchronized (MessageLevel.class)
			{
				if (mappings == null)
				{
					mappings = new java.util.HashMap<Integer, MessageLevel>();
				}
			}
		}
		return mappings;
	}

	private MessageLevel(int value)
	{
		intValue = value;
		getMappings().put(value, this);
	}

	public int getValue()
	{
		return intValue;
	}

	public static MessageLevel forValue(int value)
	{
		return getMappings().get(value);
	}
}