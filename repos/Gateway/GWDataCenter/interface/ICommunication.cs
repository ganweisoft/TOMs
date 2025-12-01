﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using OpenGWDataCenter.Model;
using System.Collections.Generic;
namespace GWDataCenter
{
    public interface ICommunication
    {
        int CommFaultReTryTime
        {
            get;
            set;
        }
        int CommWaitTime
        {
            get;
            set;
        }
        bool Initialize(EquipItem item);
        int Read(byte[] buffer, int offset, int count);
        int ReadList(List<byte[]> list_buffer);
        void Write(byte[] buffer, int offset, int count);
        void Dispose();
    }
}
