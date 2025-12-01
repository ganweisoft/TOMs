package gwdatacenter.communication.netPort;

import gwdatacenter.EquipItem;
import gwdatacenter.Interface.ICommunication;

import java.io.*;
import java.net.Socket;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.util.*;

public class GWNetPort implements ICommunication
{
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: private Queue<byte[]> _queue = new Queue<byte[]>();
	private LinkedList<byte[]> _queue = new LinkedList<byte[]>();
	private int commFaultReTryTime = 3;

	private long outTime = 1500;
	private int waitTime = 100;

	private ArrayList<Socket> sockets;

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public Queue<byte[]> getBufferQueue()
	public final LinkedList<byte[]> getBufferQueue()
	{
		return _queue;
	}
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public void setBufferQueue(Queue<byte[]> value)
	public final void setBufferQueue(LinkedList<byte[]> value)
	{
		_queue = value;
	}

	public final long getOutTime()
	{
		return outTime;
	}
	public final void setOutTime(int value)
	{
		outTime = value;
	}

	public final int getCommWaitTime()
	{
		return waitTime;
	}
	public final void setCommWaitTime(int value)
	{
		waitTime = value;
	}

	public final int getCommFaultReTryTime()
	{
		return commFaultReTryTime;
	}
	public final void setCommFaultReTryTime(int value)
	{
		commFaultReTryTime = value;
	}

	public final boolean Initialize(EquipItem item)
	{
		try
		{

			if (item == null)
			{
				return false;
			}

			InitParam(item.communication_time_param);

			getBufferQueue().clear();

			sockets = GWTCPManager.GetSocket(item.getLocalAddr());

			if (sockets == null || sockets.isEmpty())
			{
				Thread.sleep(getOutTime());
				return false;
			}

			for (Socket socket : sockets)
			{
				socket.setSoTimeout((int)outTime);
			}

			return true;
		}
		catch (RuntimeException | InterruptedException | UnknownHostException | SocketException e)
		{
			return false;
		}
    }

	private void InitParam(String communicationParam)
	{
		if (sharpSystem.StringHelper.isNullOrEmpty(communicationParam))
		{
			return;
		}
		String[] param = communicationParam.split("[/]", -1);

		if (param.length == 3)
		{
			setCommWaitTime(Integer.parseInt(param[0]));
			setOutTime(Integer.parseInt(param[1]));
			setCommFaultReTryTime(Integer.parseInt(param[2]));
		}
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public void Write(byte[] buffer, int offset, int size)
	public final void Write(byte[] buffer, int offset, int size)
	{
		for (Socket socket : sockets)
		{
			try
			{

				if (socket != null)
				{
					if (socket.getReceiveBufferSize() > 0)
					{
						//读取缓存
						Read(new byte[socket.getReceiveBufferSize()], 0, socket.getReceiveBufferSize());

						Thread.sleep(10);
					}

					var output = socket.getOutputStream();
					output.write(buffer, offset, size);
				}
			}
			catch (SocketException e)
			{
				//连接错误,返回错误代码:
				GWTCPManager.RemoveTCPServerSocket(socket);
				try {
					socket.close();
				} catch (IOException ex) {

				}
			} catch (InterruptedException | IOException ignored) {
			}
        }
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public int ReadList(List<byte[]> list_buffer)
	public final int ReadList(ArrayList<byte[]> list_buffer)
	{
		if (list_buffer != null)
		{
			while (!getBufferQueue().isEmpty())
			{
				list_buffer.add(getBufferQueue().poll());
			}
			return list_buffer.size();
		}
		return 0;
	}

//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public int Read(byte[] buffer, int offset, int size, SocketFlags socketFlags)
	public final int Read(byte[] buffer, int offset, int size)
	{
		for (Socket socket : sockets) {
			try (InputStream input = socket.getInputStream()) {
				int i = input.read(buffer, offset, size);
				if (i > 0)
				{
					return i;
				}
			} catch (Exception e) {
				try {
					socket.close();
				} catch (IOException ioe) {
				}
				System.out.println("client disconnected." + socket.getInetAddress() + ":" + socket.getPort());
			}
		}
		return 0;
	}

	public final void Dispose()
	{
		for (Socket socket : sockets)
		{
			GWTCPManager.RemoveTCPServerSocket(socket);
            try {
                socket.shutdownInput();
				socket.shutdownOutput();
				socket.close();
            } catch (IOException e) {
				System.out.println("client disconnected." + socket.getInetAddress() + ":" + socket.getPort());
            }
		}
	}
}