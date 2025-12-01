package gwdatacenter.args;

public class NoSetItemPermissionEventArgs extends sharpSystem.EventArgs
{
	private String strGUID;
	public final String getStrGUID()
	{
		return strGUID;
	}
	public final void setStrGUID(String value)
	{
		strGUID = value;
	}
}