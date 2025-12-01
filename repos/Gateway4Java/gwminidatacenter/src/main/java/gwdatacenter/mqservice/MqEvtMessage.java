package gwdatacenter.mqservice;

import gwdatacenter.*;
import java.util.*;
import java.time.*;

public class MqEvtMessage
{
	private String Time;
	public final String getTime()
	{
		return Time;
	}
	public final void setTime(String value)
	{
		Time = value;
	}
	private String Flow;
	public final String getFlow()
	{
		return Flow;
	}
	public final void setFlow(String value)
	{
		Flow = value;
	}
	private ArrayList<EquipEvent> EventItems = new ArrayList<>();
	public final ArrayList<EquipEvent> getEventItems()
	{
		return EventItems;
	}
	public final void setEventItems(ArrayList<EquipEvent> value)
	{
		EventItems = value;
	}
}