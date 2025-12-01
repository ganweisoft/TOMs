package gwdatacenter.args;

import java.time.*;

public class EquipEvent
{
	public String msg; //显示到实时快照的内容
	public String msg4Linkage; //联动传入的内容，如果为空就传入msg
	public MessageLevel level = MessageLevel.values()[0];
	public LocalDateTime dt = LocalDateTime.MIN;
	public int iEquipNo;

	public EquipEvent(String Msg, MessageLevel Level, LocalDateTime Dt)
	{
		msg = Msg;
		level = Level;
		dt = Dt;
	}
	public EquipEvent(String Msg, String Msg4Linkage, MessageLevel Level, LocalDateTime Dt)
	{
		msg = Msg;
		msg4Linkage = Msg4Linkage;
		level = Level;
		dt = Dt;
	}
}