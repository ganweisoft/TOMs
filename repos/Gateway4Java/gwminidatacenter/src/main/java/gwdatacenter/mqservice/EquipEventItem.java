package gwdatacenter.mqservice;

import gwdatacenter.args.MessageLevel;

import java.time.*;

public class EquipEventItem
{
	/** 
	 显示到实时快照的内容
	*/
	private String Msg;
	public final String getMsg()
	{
		return Msg;
	}
	public final void setMsg(String value)
	{
		Msg = value;
	}

	/** 
	 联动传入的内容，如果为空就传入msg
	*/
	private String Msg4Linkage;
	public final String getMsg4Linkage()
	{
		return Msg4Linkage;
	}
	public final void setMsg4Linkage(String value)
	{
		Msg4Linkage = value;
	}
	private MessageLevel Level = MessageLevel.values()[0];
	public final MessageLevel getLevel()
	{
		return Level;
	}
	public final void setLevel(MessageLevel value)
	{
		Level = value;
	}
	private LocalDateTime OccurDateTime = LocalDateTime.MIN;
	public final LocalDateTime getOccurDateTime()
	{
		return OccurDateTime;
	}
	public final void setOccurDateTime(LocalDateTime value)
	{
		OccurDateTime = value;
	}
	private int EquipNo;
	public final int getEquipNo()
	{
		return EquipNo;
	}
	public final void setEquipNo(int value)
	{
		EquipNo = value;
	}
}