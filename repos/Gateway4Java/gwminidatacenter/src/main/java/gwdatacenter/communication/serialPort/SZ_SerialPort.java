package gwdatacenter.communication.serialPort;

import gwdatacenter.DataCenter;
import gwdatacenter.EquipItem;
import gwdatacenter.General;
import gwdatacenter.Interface.ICommunication;
import gwdatacenter.StationItem;

import java.nio.charset.StandardCharsets;
import java.util.*;
import java.io.*;
import java.util.stream.Stream;
import javax.comm.*;

public class SZ_SerialPort implements ICommunication, Closeable
{
	private int equipno;
	private EquipItem Equip;
	public final int getEquipNo()
	{
		return equipno;
	}
	public final void setEquipNo(int value)
	{
		equipno = value;
	}

	//通讯失败重复次数
	public final int getCommFaultReTryTime()
	{
		return commFaultReTryTime;
	}
	public final void setCommFaultReTryTime(int value)
	{
		commFaultReTryTime = value;
	}
	private int commFaultReTryTime;
	//两次通讯的等待间隔(毫秒)
	public final int getCommWaitTime()
	{
		return commWaitTime;
	}
	public final void setCommWaitTime(int value)
	{
		commWaitTime = value;
	}
	private int commWaitTime = 100; //默认值

	private javax.comm.SerialPort MyserialPort = null;
	private String[] ss = Stream.of(CommPortIdentifier.getPortIdentifiers()).toArray(String[]::new);
	private String portName;
	private String param, timeparam;

	private boolean bSerialError = false; //串口是否有错误


	public SZ_SerialPort()
	{
	}

	public final void close() throws IOException
	{
		if (MyserialPort != null)
		{
			MyserialPort.close();
		}
	}

	public final javax.comm.SerialPort GetSerialPort()
	{
		return MyserialPort;
	}

	public final boolean VerifyPortNm(String portName)
	{
		if (System.getProperty("os.name").contains("Windows"))
		{
			String s = "[cC][oO][mM][1-9]\\d{0,}";
			if (General.VerifyStringFormat(s, portName))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	public final boolean VerifyParam(String param)
	{
		String s = "[\\d]+/[\\d]/[\\d]/[\\w]";
		if (General.VerifyStringFormat(s, param))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public final boolean VerifyTimeParam(String param)
	{
		String s = "[\\d]+/[\\d]+/[\\d]+/[\\d]+";
		if (General.VerifyStringFormat(s, param))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private String GetPortNm(int ista, int ieqp)
	{
		try
		{
			return StationItem.getDbEqp().stream().filter(m -> m.getStaN() == ista && m.getEquipNo() == ieqp).findFirst().get().getLocalAddr();
		}
		catch (RuntimeException e)
		{
			return null;
		}
	}
	private String GetParam(int ista, int ieqp)
	{
		try
		{
			return StationItem.getDbEqp().stream().filter(m -> m.getStaN() == ista && m.getEquipNo() == ieqp).findFirst().get().getCommunicationParam();
		}
		catch (RuntimeException e)
		{
			return null;
		}
	}
	private String GetTimeParam(int ista, int ieqp)
	{
		try
		{
			return StationItem.getDbEqp().stream().filter(m -> m.getStaN() == ista && m.getEquipNo() == ieqp).findFirst().get().getCommunicationTimeParam();
		}
		catch (RuntimeException e)
		{
			return null;
		}
	}

	public final boolean Initialize(EquipItem item)
	{

		//if ((item.attrib & 0x1)>0)
		//{                
		//    if (portName == item.Local_addr && param == item.communication_param && timeparam == item.communication_time_param)
		//        return true;     
		//}


		/*为了提高串口的可靠性（有时候串口本身有问题），强制要求每次通讯都初始化。
		//如果串口当前的参数与实际要求的相同，则不需要初始化。（用于多个设备挂在同一串口的情形）
		if (portName == item.Local_addr && param == item.communication_param && timeparam == item.communication_time_param)
		    return true;
		 */
		/*
		RetStr.Clear();
		MyserialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(MyserialPort_DataReceived);
		MyserialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(MyserialPort_DataReceived);
		MyBufLen = 0;
		*/
		portName = item.getLocalAddr(); // GetPortNm(istano, iequipno);
		param = item.communication_param; //GetParam(istano, iequipno);
		timeparam = item.communication_time_param; // GetTimeParam(istano, iequipno);
		equipno = item.getIEquipno();
		Equip = item;

		return this.Initialize(portName, param, equipno);
	}

	/** 
	 短信串口用这个函数初始化
	 
	 @param PortName
	 @param Param
	 @param iequipno
	 @return 
	*/
	public final boolean Initialize(String PortName, String Param, int iequipno)
	{

		portName = PortName;
		param = Param;
		equipno = iequipno;
		CommPortIdentifier portId = null;
		try {
			if (VerifyPortNm(portName))
			{
				if (System.getProperty("os.name").contains("Windows"))
				{
					portName = portName.strip().toUpperCase();
				}
				else
				{
					portName = portName.strip();
				}
				if (System.getProperty("os.name").contains("Windows") && Arrays.stream(ss).noneMatch(s -> portName.equals(s)))
				{
					if (!bSerialError)
					{
						bSerialError = true;
					}
					return false;
				}
				else if (isOpen())
				{
					Close();
				}
				if (System.getProperty("os.name").contains("Windows"))
				{
					portId = CommPortIdentifier.getPortIdentifier(portName.toUpperCase());
				}
				else
				{
					portId = CommPortIdentifier.getPortIdentifier(portName);
				}
			}
			else
			{
				return false;
			}

			int BaudRate = 0, DataBits = 0, StopBits = 0, Parity = 0;
			if (VerifyParam(param))
			{
				String[] ParamArray;
				ParamArray = param.split("[/]", -1);
				BaudRate = Integer.parseInt(ParamArray[0]);
				DataBits = Integer.parseInt(ParamArray[1]);
				ParamArray[2] = ParamArray[2].strip().toUpperCase();
				switch (ParamArray[2])
				{
					case "1":
						StopBits = javax.comm.SerialPort.STOPBITS_1;
						break;
					case "1.5":
						StopBits = javax.comm.SerialPort.STOPBITS_1_5;
						break;
					case "2":
						StopBits = javax.comm.SerialPort.STOPBITS_2;
						break;
					default:
						return false;
				}
				ParamArray[3] = ParamArray[3].strip().toUpperCase();

				switch (ParamArray[3])
				{
					case "NO":
						Parity = javax.comm.SerialPort.PARITY_NONE;
						break;
					case "EVEN":
						Parity = javax.comm.SerialPort.PARITY_EVEN;
						break;
					case "ODD":
						Parity = javax.comm.SerialPort.PARITY_ODD;
						break;
					case "MARK":
						Parity = javax.comm.SerialPort.PARITY_MARK;
						break;
					case "SPACE":
						Parity = javax.comm.SerialPort.PARITY_SPACE;
						break;
					default:
						return false;
				}
			}
			else
			{
				return false;
			}

			int WriteTimeout = 1000, ReadTimeout = 1000;
			if (VerifyTimeParam(timeparam))
			{
				String[] TimeParamArray;
				TimeParamArray = timeparam.split("[/]", -1);
				WriteTimeout = Integer.parseInt(TimeParamArray[0]);
				ReadTimeout = Integer.parseInt(TimeParamArray[0]);
				commFaultReTryTime = Integer.parseInt(TimeParamArray[2]);
				commWaitTime = Integer.parseInt(TimeParamArray[3]);

			}
			else
			{
				WriteTimeout = 1000;
				ReadTimeout = 1000;
				commFaultReTryTime = 3;
				commWaitTime = 500;
			}

			MyserialPort.setRTS(true);
			MyserialPort.setDTR(true);

			MyserialPort = (javax.comm.SerialPort) portId.open("gwminidatacenter", 1000);
			MyserialPort.setSerialPortParams(BaudRate, DataBits, StopBits, Parity);
			MyserialPort.enableReceiveTimeout(ReadTimeout);
		}
		catch (Exception e) {
			throw new RuntimeException(e);
		}

		return true;
	}

	public final boolean Open()
	{
		try
		{
			MyserialPort.notifyOnDataAvailable(true);
			MyserialPort.notifyOnBreakInterrupt(true);
			if (MyserialPort == null)
			{
				return false;
			}
			Thread.sleep(20);

			MyserialPort.setRTS(true);
			MyserialPort.setDTR(true);
		}
		catch (RuntimeException e)
		{
			return false;
		} catch (InterruptedException e) {

        }

        return true;
	}

	public final void Close()
	{
		MyserialPort.close();
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public void Write(byte[] buffer, int offset, int count)
	public final void Write(byte[] buffer, int offset, int count)
	{
		if (System.getProperty("os.name").contains("Windows") && Arrays.stream(ss).noneMatch(s -> portName.equals(s)))
		{
			return;
		}

		String msg = "";

		try
		{
			if (!isOpen())
			{
				Open();
				Thread.sleep(20);
			}
			if (isOpen())
			{
				OutputStream os = MyserialPort.getOutputStream();//发送命令到外设
				os.write(buffer, offset, count);
				MyserialPort.setDTR(true);
			}

			if (Equip != null && Equip.isDebug())
			{
				//输出读取信息
				msg = portName + ">>Write" + ">>";
				msg += new String(buffer, offset, count, StandardCharsets.UTF_8);
				msg += "(";
				for (int k = 0; k < count; k++)
				{
					msg += buffer[k + offset];
					msg += "/";
				}
				msg += ")";

				DataCenter.WriteLogFile(msg);
			}
		}
		catch (RuntimeException | InterruptedException ex)
		{
		} catch (IOException e) {
			throw new RuntimeException(e);
		}
	}

	@Override
	public void Dispose() {

	}

	//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public int ReadList(List<byte[]> list_buffer)
	public final int ReadList(ArrayList<byte[]> list_buffer)
	{
		//不需要实现该接口
		return 0;
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public int Read(byte[] buffer, int offset, int count)
	public final int Read(byte[] buffer, int offset, int count)
	{
		if (System.getProperty("os.name").contains("Windows") && Arrays.stream(ss).noneMatch(s -> portName.equals(s)))
		{
			return 0;
		}


		String msg = "";
		int iRet = 0;

		try
		{
			if (!isOpen())
			{
				Open();
				Thread.sleep(20);
			}

			var is = MyserialPort.getInputStream();
			iRet = is.read(buffer, offset, count);
			MyserialPort.setRTS(true);

			if (Equip != null && Equip.isDebug()){

				//输出读取信息
				msg += portName + ">>Read" + "[" + iRet + "/" + count + "]" + ">>";
				msg += new String(buffer, offset, iRet, StandardCharsets.UTF_8);
				msg += "(";
				for (int k = 0; k < iRet; k++)
				{
					msg += buffer[k + offset];
					msg += "/";
				}
				msg += ")";

				DataCenter.WriteLogFile(msg);
			}
		}
		catch (RuntimeException | InterruptedException ex)
		{
		} catch (IOException e) {
			throw new RuntimeException(e);
		}

		return iRet;
	}

	public final boolean isOpen()
	{
		return MyserialPort != null;
	}
}