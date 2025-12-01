package gwdatacenter.mqservice;

import gwdatacenter.*;

public class MqCmdMessage
{
	public String RequestId;
	public final String getRequestId()
	{
		return RequestId;
	}
	public final void setRequestId(String value)
	{
		RequestId = value;
	}
	public String LoginName;
	public final String getLoginName()
	{
		return LoginName;
	}
	public final void setLoginName(String value)
	{
		LoginName = value;
	}
	public String Flow;
	public final String getFlow()
	{
		return Flow;
	}
	public final void setFlow(String value)
	{
		Flow = value;
	}
	public String MainInstruct;
	public final String getMainInstruct()
	{
		return MainInstruct;
	}
	public final void setMainInstruct(String value)
	{
		MainInstruct = value;
	}
	public String MinorInstruct;
	public final String getMinorInstruct()
	{
		return MinorInstruct;
	}
	public final void setMinorInstruct(String value)
	{
		MinorInstruct = value;
	}
	public String Value;
	public final String getValue()
	{
		return Value;
	}
	public final void setValue(String value)
	{
		Value = value;
	}
	public int GatewayId;
	public final int getGatewayId()
	{
		return GatewayId;
	}
	public final void setGatewayId(int value)
	{
		GatewayId = value;
	}
	/** 
	 下级平台设备编号
	*/
	public int EquipNo;
	public final int getEquipNo()
	{
		return EquipNo;
	}
	public final void setEquipNo(int value)
	{
		EquipNo = value;
	}
	public int SetNo;
	public final int getSetNo()
	{
		return SetNo;
	}
	public final void setNo(int value)
	{
		SetNo = value;
	}
	/** 
	 下级平台的终端唯一标识
	*/
	public int TerminalIdentityId;
	public final int getTerminalIdentityId()
	{
		return TerminalIdentityId;
	}
	public final void setTerminalIdentityId(int value)
	{
		TerminalIdentityId = value;
	}
}