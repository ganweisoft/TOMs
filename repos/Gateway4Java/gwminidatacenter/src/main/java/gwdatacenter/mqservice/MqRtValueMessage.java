package gwdatacenter.mqservice;

import gwdatacenter.*;
import java.util.*;

public class MqRtValueMessage
{
	private int DataType;
	public final int getDataType()
	{
		return DataType;
	}
	public final void setDataType(int value)
	{
		DataType = value;
	}
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
	private ArrayList<DataItem> DataItems;
	public final ArrayList<DataItem> getDataItems()
	{
		return DataItems;
	}
	public final void setDataItems(ArrayList<DataItem> value)
	{
		DataItems = value;
	}
}