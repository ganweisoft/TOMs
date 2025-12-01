package gwdatacenter.communication.netPort;

import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.*;

public class GWTCPServer
{
	private int _port;
	public final int getPort()
	{
		return _port;
	}
	public final void setPort(int value)
	{
		_port = value;
	}

	private ArrayList<Socket> _clientList = new ArrayList<Socket>();
	private boolean disposed = false;

	public GWTCPServer()
	{

	}


	public GWTCPServer(int port)
	{
		setPort(port);
		StartListen();
	}

	public final Socket GetClientSocket(InetAddress ip, int port)
	{
		for (Socket socket : _clientList)
		{
			if (port != -1 && socket.getInetAddress() == ip && port == socket.getPort()) {
				return socket;
			}
		}
		return null;
	}

	public final Socket GetClientSocketByPort(int port)
	{
		if (port == -1)
		{
			return _clientList.stream().findFirst().orElse(null);
		}

		for (Socket socket : _clientList)
		{
			var endpoint = socket.getPort();
			if (port == endpoint)
			{
				return socket;
			}
		}
		return null;
	}

	public final ArrayList<Socket> GetAllClientSocket()
	{
        ArrayList<Socket> sockets = new ArrayList<Socket>(_clientList);

		return sockets;
	}

	public final void StartListen() {
		Thread thListen = new Thread() {
			public void run() {
				Listen();
			}
		};
		thListen.start();
	}

	private void Listen()
	{
		try
		{
			var server = new ServerSocket(getPort(), 20, InetAddress.getByName("0.0.0.0"));

			while (!disposed)
			{
				var clientSocket = server.accept();
				new Thread(()->{
					for (Socket socket : _clientList.toArray(new Socket[0]))
					{
						String tempVar = socket.getInetAddress().toString() + ":" + socket.getPort();
						String tempVar2 = clientSocket.getInetAddress().toString() + ":" + socket.getPort();
						if (tempVar.equals(tempVar2))
						{
							_clientList.remove(socket);
						}
					}

					_clientList.add(clientSocket);
				}).start();
			}
		}
		catch (RuntimeException | IOException ex)
		{
			System.out.println(ex.getMessage());
		}
    }

	public final void RemoveSocket(Socket socket)
	{
		_clientList.remove(socket);
	}
}