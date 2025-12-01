package gwdatacenter.communication;

import gwdatacenter.EquipItem;
import gwdatacenter.communication.netPort.GWNetPort;
import gwdatacenter.Interface.ICommunication;
import gwdatacenter.StationItem;
import gwdatacenter.communication.serialPort.SZ_SerialPort;

import java.util.*;

/** 
 监控系统数据库包装类
*/
public class SerialPort implements ICommunication
{
	public ICommunication Instance = null;
	private String szLocal_addr;
	public SerialPort()
	{
	}


	//通讯失败重复次数
	public final int getCommFaultReTryTime()
	{
		if (Instance != null)
		{
			return Instance.getCommFaultReTryTime();
		}
		return 3;
	}
	public final void setCommFaultReTryTime(int value)
	{
		if (Instance != null)
		{
			Instance.setCommFaultReTryTime(value);
		}
	}
	//两次通讯的等待间隔(毫秒)
	public final int getCommWaitTime()
	{
		if (Instance != null)
		{
			return Instance.getCommWaitTime();
		}
		return 500;
	}
	public final void setCommWaitTime(int value)
	{
		if (Instance != null)
		{
			Instance.setCommFaultReTryTime(value);
		}
	}

	public final boolean Initialize(EquipItem item)
	{
		String strFlag = item.getLocalAddr();
		szLocal_addr = item.getLocalAddr();
		String strFlag1 = item.communication_param;
		strFlag = strFlag.toUpperCase().strip();
		strFlag1 = strFlag1.toUpperCase().strip();
		boolean flag = false;
		if (Instance == null)
		{
			Instance = new SZ_SerialPort();

			if (strFlag.substring(0, 2).equals("TS") || strFlag.substring(0, 2).equals("TC"))
			{
				Instance = new GWNetPort();
			}
		}

		if (Instance == null)
		{
			return false;
		}
		try
		{
			flag = Instance.Initialize(item);
			if (item.communication_drv.toUpperCase().contains("DATASIMU.NET.DLL")) //如果是模拟动态库，就初始化成功，便于显示模拟数据
			{
				flag = true;
			}
		}
		catch (RuntimeException e)
		{
		}
		return flag;
	}


//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public int Read(byte[] buffer, int offset, int count)
	public final int Read(byte[] buffer, int offset, int count)
	{
		int k = 0;
		try
		{
			k = Instance.Read(buffer, offset, count);
		}
		catch (RuntimeException e)
		{
		}
		return k;
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public int ReadList(List<byte[]> list_buffer)
	public final int ReadList(ArrayList<byte[]> list_buffer)
	{
		int k = 0;
		try
		{
			k = Instance.ReadList(list_buffer);
		}
		catch (RuntimeException e)
		{
		}
		return k;
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public void Write(byte[] buffer, int offset, int count)
	public final void Write(byte[] buffer, int offset, int count)
	{
		try
		{
			Instance.Write(buffer, offset, count);
		}
		catch (RuntimeException e)
		{
		}
	}

	public final void Dispose()
	{
		synchronized (StationItem.EquipCategoryDict)
		{
			if (!StationItem.EquipCategoryDict.containsKey(szLocal_addr)) //确保该端口没有任何占用的的时候才关闭
			{
				Instance.Dispose();
			}
		}
	}

}