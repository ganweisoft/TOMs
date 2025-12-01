﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
namespace GWDataCenter
{
    public class GWNetPort : ICommunication
    {
        private Queue<byte[]> _queue = new Queue<byte[]>();
        private int commFaultReTryTime = 3;
        private int outTime = 1500;
        private int waitTime = 100;
        private List<Socket> sockets;
        public Queue<byte[]> BufferQueue
        {
            get { return _queue; }
            set { _queue = value; }
        }
        public int OutTime
        {
            get { return outTime; }
            set { outTime = value; }
        }
        public int CommWaitTime
        {
            get { return waitTime; }
            set { waitTime = value; }
        }
        public int CommFaultReTryTime
        {
            get { return commFaultReTryTime; }
            set { commFaultReTryTime = value; }
        }
        public bool Initialize(EquipItem item)
        {
            try
            {
                if (item == null)
                {
                    return false;
                }
                InitParam(item.communication_time_param);
                BufferQueue.Clear();
                sockets = GWTCPManager.GetSocket(item.Local_addr);
                if (sockets == null || sockets.Count <= 0)
                {
                    Thread.Sleep(OutTime);
                    return false;
                }
                foreach (Socket socket in sockets)
                {
                    socket.ReceiveTimeout = outTime;
                }
                return true;
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Debug, e.ToString(), item.iEquipno);
                Thread.Sleep(OutTime);
                return false;
            }
        }
        private void InitParam(string communicationParam)
        {
            if (string.IsNullOrEmpty(communicationParam))
            {
                return;
            }
            string[] param = communicationParam.Split('/');
            if (param.Length == 3)
            {
                CommWaitTime = Convert.ToInt32(param[0]);
                OutTime = Convert.ToInt32(param[1]);
                CommFaultReTryTime = Convert.ToInt32(param[2]);
            }
        }
        public void Write(byte[] buffer, int offset, int size)
        {
            Write(buffer, offset, size, SocketFlags.None);
        }
        public int Read(byte[] buffer, int offset, int size)
        {
            return Read(buffer, offset, size, SocketFlags.None);
        }
        public int ReadList(List<byte[]> list_buffer)
        {
            if (list_buffer != null)
            {
                while (BufferQueue.Count > 0)
                {
                    list_buffer.Add(BufferQueue.Dequeue());
                }
                return list_buffer.Count;
            }
            return 0;
        }
        public void Write(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            foreach (Socket socket in sockets)
            {
                try
                {
                    if (socket != null)
                    {
                        if (socket.Available > 0)
                        {
                            Read(new byte[socket.Available], 0, socket.Available);
                            Thread.Sleep(10);
                        }
                        socket.Send(buffer, offset, size, socketFlags);
                    }
                }
                catch (SocketException e)
                {
                    if (e.NativeErrorCode.Equals(10035))
                    {
                    }
                    else
                    {
                        GWTCPManager.RemoveTCPServerSocket(socket);
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }
            }
        }
        public int Read(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            foreach (Socket socket in sockets)
            {
                try
                {
                    int i = socket.Receive(buffer, offset, size, socketFlags);
                    if (i > 0)
                    {
                        return i;
                    }
                }
                catch (SocketException e)
                {
                }
                catch (Exception e)
                {
                }
            }
            return 0;
        }
        public void Dispose()
        {
            foreach (Socket socket in sockets)
            {
                GWTCPManager.RemoveTCPServerSocket(socket);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
    public class StateObject
    {
        public static int BufferSize = 1024;
        public byte[] buffer = new byte[1024];
        public List<byte[]> bufferList = new List<byte[]>();
        public Socket workSocket;
    }
}