package gwdatacenter.args;

import java.util.*;

public enum SafetyLevel
{
	Unsafety(1),
	Safety(2);

	public static final int SIZE = Integer.SIZE;

	private int intValue;
	private static HashMap<Integer, SafetyLevel> mappings;
	private static HashMap<Integer, SafetyLevel> getMappings()
	{
		if (mappings == null)
		{
			synchronized (SafetyLevel.class)
			{
				if (mappings == null)
				{
					mappings = new HashMap<Integer, SafetyLevel>();
				}
			}
		}
		return mappings;
	}

	private SafetyLevel(int value)
	{
		intValue = value;
		getMappings().put(value, this);
	}

	public int getValue()
	{
		return intValue;
	}

	public static SafetyLevel forValue(int value)
	{
		return getMappings().get(value);
	}
}