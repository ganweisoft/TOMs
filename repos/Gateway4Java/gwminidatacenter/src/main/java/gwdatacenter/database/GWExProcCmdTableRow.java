package gwdatacenter.database;

import gwdatacenter.*;

public class GWExProcCmdTableRow
{
	private int proc_code;
	public final int getProcCode()
	{
		return proc_code;
	}
	public final void setProcCode(int value)
	{
		proc_code = value;
	}
	private String cmd_nm;
	public final String getCmdNm()
	{
		return cmd_nm;
	}
	public final void setCmdNm(String value)
	{
		cmd_nm = value;
	}
	private String main_instruction;
	public final String getMainInstruction()
	{
		return main_instruction;
	}
	public final void setMainInstruction(String value)
	{
		main_instruction = value;
	}
	private String minor_instruction;
	public final String getMinorInstruction()
	{
		return minor_instruction;
	}
	public final void setMinorInstruction(String value)
	{
		minor_instruction = value;
	}
	private String value;
	public final String getValue()
	{
		return value;
	}
	public final void setValue(String value)
	{
		value = value;
	}
	private boolean record;
	public final boolean getRecord()
	{
		return record;
	}
	public final void setRecord(boolean value)
	{
		record = value;
	}
}