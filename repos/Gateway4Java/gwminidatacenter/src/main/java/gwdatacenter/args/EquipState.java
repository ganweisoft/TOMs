package gwdatacenter.args;

import java.util.*;

public enum EquipState
{
	NoCommunication(0),
	CommunicationOK(1),
	HaveAlarm(2),
	HaveSetParm(3),
	Initial(4),
	CheFang(5),
	BackUp(6);

	public static final int SIZE = Integer.SIZE;

	private int intValue;
	private static HashMap<Integer, EquipState> mappings;
	private static HashMap<Integer, EquipState> getMappings()
	{
		if (mappings == null)
		{
			synchronized (EquipState.class)
			{
				if (mappings == null)
				{
					mappings = new HashMap<Integer, EquipState>();
				}
			}
		}
		return mappings;
	}

	private EquipState(int value)
	{
		intValue = value;
		getMappings().put(value, this);
	}

	public int getValue()
	{
		return intValue;
	}

	public static EquipState forValue(int value)
	{
		return getMappings().get(value);
	}
}