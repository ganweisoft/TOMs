﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
namespace GWDataCenter
{
    public enum GWTcpType
    {
        TS,
        TC,
        Other
    }
    public class GWTCPManager
    {
        public static List<GWTCPServer> TSList = new List<GWTCPServer>();
        public static List<GWTCPClient> TCList = new List<GWTCPClient>();
        public static List<Socket> GetSocket(string localAddrParams)
        {
            string[] comparams = localAddrParams.Split('-');
            string commparam = comparams[0];
            GWTcpType gwType = GetNetTypeByParam(commparam);
            if (gwType == GWTcpType.TS)
            {
                int port = Convert.ToInt32(commparam.Split('/')[1]);
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                int rePort = -1;
                if (comparams.Length == 2)
                {
                    string sIPEndPort = comparams[1];
                    string[] sIpParam = sIPEndPort.Split(':');
                    if (sIpParam.Length == 1)
                    {
                        ip = IPAddress.Parse(sIpParam[0]);
                    }
                    else if (sIpParam.Length == 2)
                    {
                        ip = IPAddress.Parse(sIpParam[0]);
                        rePort = Convert.ToInt32(sIpParam[1]);
                    }
                }
                GWTCPServer tcpserver = GetTCPServerByPort(port);
                if (tcpserver == null)
                {
                    tcpserver = new GWTCPServer(port);
                    lock (TSList)
                    {
                        TSList.Add(tcpserver);
                    }
                }
                if (ip.ToString() == "127.0.0.1" && rePort == -1)
                {
                    return new List<Socket>(tcpserver.GetAllClientSocket());
                }
                if (ip.ToString() == "127.0.0.1")
                {
                    return new List<Socket>() { tcpserver.GetClientSocketByPort(rePort) };
                }
                return new List<Socket>() { tcpserver.GetClientSocket(ip, rePort) };
            }
            if (gwType == GWTcpType.TC)
            {
                string sIPParam = commparam.Split('/')[1];
                string[] sIPParams = sIPParam.Split(':');
                if (sIPParams.Length == 2)
                {
                    IPAddress ip = IPAddress.Parse(sIPParams[0]);
                    int port = Convert.ToInt32(sIPParams[1]);
                    var client = GetTCPClient(ip, port);
                    if (client == null)
                    {
                        try
                        {
                            client = new GWTCPClient(ip, port);
                            lock (TCList)
                            {
                                TCList.Add(client);
                            }
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }
                    return new List<Socket>() { client.Socket };
                }
            }
            return null;
        }
        public static GWTcpType GetNetTypeByParam(string sParm)
        {
            GWTcpType gwType;
            string[] commparam = sParm.Split('/');
            if (commparam[0].ToLower() == "ts")
            {
                gwType = GWTcpType.TS;
            }
            else if (commparam[0].ToLower() == "tc")
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
            foreach (GWTCPServer gwtcpServer in TSList)
            {
                if (gwtcpServer.Port == port)
                {
                    return gwtcpServer;
                }
            }
            return null;
        }
        public static GWTCPClient GetTCPClient(IPAddress ip, int port)
        {
            return TCList.FirstOrDefault(tc => tc.IP.ToString() == ip.ToString() && tc.Port == port);
        }
        public static void RemoveTCPServerSocket(Socket socket)
        {
            lock (TSList)
            {
                foreach (GWTCPServer gwtcpServer in TSList)
                {
                    if (gwtcpServer.Port == (socket.LocalEndPoint as IPEndPoint).Port)
                    {
                        gwtcpServer.RemoveSocket(socket);
                    }
                }
            }
            lock (TCList)
            {
                foreach (GWTCPClient client in TCList.ToArray())
                {
                    if (client.Socket == socket)
                        TCList.Remove(client);
                }
            }
        }
    }
}