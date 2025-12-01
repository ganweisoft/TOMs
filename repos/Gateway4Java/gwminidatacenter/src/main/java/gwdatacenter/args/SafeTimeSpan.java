package gwdatacenter.args;

import java.time.OffsetTime;

public class SafeTimeSpan
{
	public OffsetTime tStart;
	public OffsetTime tEnd;
	public SafeTimeSpan(OffsetTime t, OffsetTime t1)
	{
		tStart = t;
		tEnd = t1;
	}
}