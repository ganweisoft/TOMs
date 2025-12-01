using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GWDataCenter
{
    /// <summary>
    /// 监控系统数据库包装类
    /// </summary>
    public class SerialPort : ICommunication
    {
        public ICommunication Instance = null;
        string szLocal_addr;
        public SerialPort()
        {
        }


        //通讯失败重复次数
        public int CommFaultReTryTime
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.CommFaultReTryTime;
                }
                return 3;
            }
            set
            {
                if (Instance != null)
                {
                    Instance.CommFaultReTryTime = value;
                }
            }
        }
        //两次通讯的等待间隔(毫秒)
        public int CommWaitTime
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.CommWaitTime;
                }
                return 500;
            }
            set
            {
                if (Instance != null)
                {
                    Instance.CommFaultReTryTime = value;
                }
            }
        }

        public bool Initialize(EquipItem item)
        {
            string strFlag = item.Local_addr;
            szLocal_addr = item.Local_addr;
            string strFlag1 = item.communication_param;
            strFlag = strFlag.ToUpper().Trim();
            strFlag1 = strFlag1.ToUpper().Trim();
            bool flag = false;
            if (Instance == null)
            {
                Instance = new SZ_SerialPort();

                if (strFlag.Substring(0, 2) == "TS" || strFlag.Substring(0, 2) == "TC")
                    Instance = new GWNetPort();
            }

            if (Instance == null)
            {
                return false;
            }
            try
            {
                flag = Instance.Initialize(item);
                if (item.communication_drv.ToUpper().Contains("DATASIMU.NET.DLL"))//如果是模拟动态库，就初始化成功，便于显示模拟数据
                    flag = true;
            }
            catch (Exception e)
            {
            }
            return flag;
        }


        public int Read(byte[] buffer, int offset, int count)
        {
            int k = 0;
            try
            {
                k = Instance.Read(buffer, offset, count);
            }
            catch (Exception e)
            {
            }
            return k;
        }

        public int ReadList(List<byte[]> list_buffer)
        {
            int k = 0;
            try
            {
                k = Instance.ReadList(list_buffer);
            }
            catch (Exception e)
            {
            }
            return k;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                Instance.Write(buffer, offset, count);
            }
            catch (Exception e)
            {
            }
        }

        public void Dispose()
        {
            lock (StationItem.EquipCategoryDict)
            {
                if (!StationItem.EquipCategoryDict.ContainsKey(szLocal_addr))//确保该端口没有任何占用的的时候才关闭
                {
                    Instance.Dispose();
                }
            }
        }

    }
}
