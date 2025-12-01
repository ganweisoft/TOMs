package gwdatacenter.communication.netPort;

import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;

public class GWTCPClient
{
	private Socket socket;

	public final Socket getSocket()
	{
		return socket;
	}

	private InetAddress ip;
	private int port;

	public final InetAddress getIP()
	{
		return ip;
	}

	public final int getPort()
	{
		return port;
	}

	public GWTCPClient(InetAddress ip, int port) throws IOException {
		this.ip = ip;
		this.port = port;

		socket = new Socket(this.ip, this.port);

	}

}