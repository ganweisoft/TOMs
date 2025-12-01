using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GWDataCenter
{
    public interface ICommunication
    {
        //通讯失败重复次数
        int CommFaultReTryTime
        {
            get;
            set;
        }
        //两次通讯的等待间隔(毫秒)
        int CommWaitTime
        {
            get;
            set;
        }

        bool Initialize(EquipItem item);
        int Read(byte[] buffer, int offset, int count);
        int ReadList(List<byte[]> list_buffer);//用于多个程序与串口服务器同时通讯的时候
        void Write(byte[] buffer, int offset, int count);
        void Dispose();
    }
}
