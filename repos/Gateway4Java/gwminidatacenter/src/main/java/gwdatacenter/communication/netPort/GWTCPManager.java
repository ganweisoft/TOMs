package gwdatacenter.communication.netPort;

import java.io.IOException;
import java.net.*;
import java.util.*;

public class GWTCPManager
{
	public static ArrayList<GWTCPServer> TSList = new ArrayList<GWTCPServer>();
	public static ArrayList<GWTCPClient> TCList = new ArrayList<GWTCPClient>();

	public static ArrayList<Socket> GetSocket(String localAddrParams) throws UnknownHostException {
		//string[] commparam = item.Local_addr.Split('/');
		//TS/8002-192.168.0.2
		//TC/192.168.0.2:8002

		String[] comparams = localAddrParams.split("[-]", -1);
		String commparam = comparams[0];
		GWTcpType gwType = GetNetTypeByParam(commparam);

		if (gwType == GWTcpType.TS)
		{
			//string[] sIpParam = "10.1.144.200:8002".Split(':');
			int port = Integer.parseInt(commparam.split("[/]", -1)[0]); //port

			InetAddress ip = InetAddress.getLoopbackAddress();
			int rePort = -1;

			if (comparams.length == 2)
			{
				String sIPEndPort = comparams[1];
				//tcp server
				String[] sIpParam = sIPEndPort.split("[:]", -1); //IP

				//int port = 8002;
				if (sIpParam.length == 1)
				{
					ip = InetAddress.getByName(sIpParam[0]);
				}
				else if (sIpParam.length == 2)
				{
					ip = InetAddress.getByName(sIpParam[0]);
					rePort = Integer.parseInt(sIpParam[1]);
				}
			}

			GWTCPServer tcpserver = GetTCPServerByPort(port);
			if (tcpserver == null)
			{
				tcpserver = new GWTCPServer(port);
				synchronized (TSList)
				{
					TSList.add(tcpserver);
				}
			}

			if (ip.toString().equals("127.0.0.1") && rePort == -1)
			{
				return new ArrayList<Socket>(tcpserver.GetAllClientSocket());
			}
			if (ip.toString().equals("127.0.0.1"))
			{
				return new ArrayList<Socket>(Arrays.asList(tcpserver.GetClientSocketByPort(rePort)));
			}
			return new ArrayList<Socket>(Arrays.asList(tcpserver.GetClientSocket(ip, rePort)));
		}
		if (gwType == GWTcpType.TC)
		{
			//tcp client
			String sIPParam = commparam.split("[/]", -1)[1];
			String[] sIPParams = sIPParam.split("[:]", -1);
			if (sIPParams.length == 2)
			{
				InetAddress ip = InetAddress.getByName(sIPParams[0]);
				int port = Integer.parseInt(sIPParams[1]);

				var client = GetTCPClient(ip, port);
				if (client == null)
				{
					try
					{
						client = new GWTCPClient(ip, port);
						synchronized (TCList)
						{
							TCList.add(client);
						}
					}
					catch (RuntimeException ex)
					{
						return null;
					} catch (IOException e) {
                        throw new RuntimeException(e);
                    }

                }
				return new ArrayList<Socket>(Arrays.asList(client.getSocket()));
			}
		}
		return null;
	}

	public static GWTcpType GetNetTypeByParam(String sParm)
	{
		GWTcpType gwType;
		String[] commparam = sParm.split("[/]", -1);
		if (commparam[0].toLowerCase().equals("ts"))
		{
			gwType = GWTcpType.TS;
		}
		else if (commparam[0].toLowerCase().equals("tc"))
		{
			gwType = GWTcpType.TC;
		}
		else
		{
			gwType = GWTcpType.Other;
		}
		return gwType;
	}

	public static GWTCPServer GetTCPServerByPort(int port)
	{
		for (GWTCPServer gwtcpServer : TSList)
		{
			if (gwtcpServer.getPort() == port)
			{
				return gwtcpServer;
			}
		}

		return null;
	}

	public static GWTCPClient GetTCPClient(InetAddress ip, int port) {
		var filter = TCList.stream().filter(tc -> tc.getPort() == port && tc.getIP() == ip).findFirst();
        return filter.orElse(null);
    }

	public static void RemoveTCPServerSocket(Socket socket)
	{
		synchronized (TSList)
		{
			for (GWTCPServer gwtcpServer : TSList)
			{
				int tempVar = socket.getLocalPort();
				if (gwtcpServer.getPort() == tempVar)
				{
					gwtcpServer.RemoveSocket(socket);
				}
			}
		}
		synchronized (TCList)
		{
			for (GWTCPClient client : TCList.toArray(new GWTCPClient[0]))
			{
				if (client.getSocket() == socket)
				{
					TCList.remove(client);
				}
			}
		}
	}
}