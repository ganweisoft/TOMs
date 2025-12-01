package gwdatacenter;

import java.util.Timer;
import java.util.TimerTask;

/**
 事件延迟执行类。用于某些频繁事件在指定时间后只需激发一次事件
*/
public class DelayEventFire
{
	private Timer T = new Timer();
	private sharpSystem.EventHandler<sharpSystem.EventArgs> m_EventHandler;
	private int m_Msec;
	private Object m_Sender;
	private sharpSystem.EventArgs m_Args;
	private boolean binit = false;
	private Object olock = false;
	/** 
	 
	 
	 @param Hander 传入的事件
	 @param Msec 延时时间，单位毫秒
	 @param Sender 传入事件的sender参数
	 @param Args 传入事件的EventArgs参数
	*/
	public DelayEventFire(sharpSystem.EventHandler<sharpSystem.EventArgs> Hander, int Msec, Object Sender, sharpSystem.EventArgs Args)
	{
		m_EventHandler = Hander;
		m_Msec = Msec;
		m_Sender = Sender;
		m_Args = Args;
	}

	/** 
	 在限定时间Msec必须触发，防止持续的事件涌入导致传入事件迟迟不触发
	*/
	public final void AddEvent()
	{
		synchronized (olock)
		{
			if (!binit)
			{
				T.schedule(new TimerTask() {
					@Override
					public void run() {
						theout(T);
					}
				}, m_Msec);
				binit = true;
			}
		}
	}

	public final void theout(Object source)
	{
		synchronized (olock)
		{
			if (m_EventHandler != null)
			{
				m_EventHandler.invoke(m_Sender, m_Args);
			}
			((Timer)source).cancel();
		}
	}
}