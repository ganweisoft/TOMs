﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace GWDataCenter
{
    public class GWTCPServer
    {
        private int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        private ManualResetEvent _allDone = new ManualResetEvent(false);
        private List<Socket> _clientList = new List<Socket>();
        private bool disposed = false;
        public GWTCPServer()
        {
        }
        public GWTCPServer(int port)
        {
            Port = port;
            StartListen();
        }
        public Socket GetClientSocket(IPAddress ip, int port)
        {
            foreach (Socket socket in _clientList)
            {
                if (port != -1 && socket.RemoteEndPoint.ToString() == (new IPEndPoint(ip, port)).ToString())
                {
                    return socket;
                }
                if ((socket.RemoteEndPoint as IPEndPoint).Address.ToString() == ip.ToString())
                {
                    return socket;
                }
            }
            return null;
        }
        public Socket GetClientSocketByPort(int port)
        {
            if (port == -1)
            {
                return _clientList.FirstOrDefault();
            }
            foreach (Socket socket in _clientList)
            {
                string endpoint = socket.RemoteEndPoint.ToString();
                if (endpoint.Split(':')[1] == port.ToString())
                {
                    return socket;
                }
            }
            return null;
        }
        public List<Socket> GetAllClientSocket()
        {
            List<Socket> sockets = new List<Socket>();
            foreach (Socket socket in _clientList)
            {
                sockets.Add(socket);
            }
            return sockets;
        }
        public void StartListen()
        {
            Thread thListen = new Thread(new ThreadStart(Listen));
            thListen.Start();
        }
        private void Listen()
        {
            try
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, Port));
                server.Listen(20);
                while (!disposed)
                {
                    _allDone.Reset();
                    server.BeginAccept(new AsyncCallback(Accept), server);
                    _allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                MessageService.AddMessage(MessageLevel.Error, ex.ToString(), 0);
                return;
            }
        }
        public void Accept(IAsyncResult iar)
        {
            _allDone.Set();
            Socket MyServer = (Socket)iar.AsyncState;
            Socket client = MyServer.EndAccept(iar);
            foreach (Socket socket in _clientList.ToArray())
            {
                IPEndPoint ip1 = (socket.RemoteEndPoint as IPEndPoint);
                IPEndPoint ip2 = (client.RemoteEndPoint as IPEndPoint);
                if (ip1.ToString() == ip2.ToString())
                {
                    _clientList.Remove(socket);
                }
            }
            _clientList.Add(client);
        }
        public void RemoveSocket(Socket socket)
        {
            _clientList.Remove(socket);
        }
    }
}
