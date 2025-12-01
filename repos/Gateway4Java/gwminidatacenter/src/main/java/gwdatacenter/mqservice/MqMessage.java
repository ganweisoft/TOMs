package gwdatacenter.mqservice;

import gwdatacenter.database.*;
import gwdatacenter.*;
import java.util.*;
import java.time.*;

public class MqMessage
{
	public String Flow;
	public final String getFlow()
	{
		return Flow;
	}
	public final void setFlow(String value)
	{
		Flow = value;
	}
	public int FlowType;
	public final int getFlowType()
	{
		return FlowType;
	}
	public final void setFlowType(int value)
	{
		FlowType = value;
	}
	public ArrayList<Equip> Equips;
	public final ArrayList<Equip> getEquips()
	{
		return Equips;
	}
	public final void setEquips(ArrayList<Equip> value)
	{
		Equips = value;
	}
}