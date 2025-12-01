package gwdatacenter.mqservice;

import gwdatacenter.*;
import java.util.*;

public class MqRtStateMessage
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
	private ArrayList<StateItem> StateItems = new ArrayList<>();
	public final ArrayList<StateItem> getStateItems()
	{
		return StateItems;
	}
	public final void setStateItems(ArrayList<StateItem> value)
	{
		StateItems = value;
	}
}