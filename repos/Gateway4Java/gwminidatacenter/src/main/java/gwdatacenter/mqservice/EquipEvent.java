package gwdatacenter.mqservice;

import gwdatacenter.*;
import java.util.*;
import java.time.*;

public class EquipEvent
{
	private int DeviceId;
	public final int getDeviceId()
	{
		return DeviceId;
	}
	public final void setDeviceId(int value)
	{
		DeviceId = value;
	}
	private ArrayList<EquipEventItem> EquipEvents = new ArrayList<>();
	public final ArrayList<EquipEventItem> getEquipEvents()
	{
		return EquipEvents;
	}
	public final void setEquipEvents(ArrayList<EquipEventItem> value)
	{
		EquipEvents = value;
	}
}