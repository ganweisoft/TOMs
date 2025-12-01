﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.Net;
using System.Net.Sockets;
namespace GWDataCenter
{
    public class GWTCPClient
    {
        private Socket socket;
        public Socket Socket
        {
            get { return socket; }
        }
        private IPAddress ip;
        private int port;
        public IPAddress IP
        {
            get { return ip; }
        }
        public int Port
        {
            get { return port; }
        }
        public GWTCPClient(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ip, port);
        }
    }
}