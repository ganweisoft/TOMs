package gwdatacenter.Interface;

import gwdatacenter.EquipItem;

import java.util.*;

public interface ICommunication
{
	//通讯失败重复次数
	int getCommFaultReTryTime();
	void setCommFaultReTryTime(int value);
	//两次通讯的等待间隔(毫秒)
	int getCommWaitTime();
	void setCommWaitTime(int value);

	boolean Initialize(EquipItem item);
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: int Read(byte[] buffer, int offset, int count);
	int Read(byte[] buffer, int offset, int count);
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: int ReadList(List<byte[]> list_buffer);
	int ReadList(ArrayList<byte[]> list_buffer); //用于多个程序与串口服务器同时通讯的时候
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: void Write(byte[] buffer, int offset, int count);
	void Write(byte[] buffer, int offset, int count);
	void Dispose();
}