﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace GWDataCenter
{
    public class SerialPort : ICommunication
    {
        bool bFlag = false;
        public ICommunication Instance = null;
        string szLocal_addr;
        public SerialPort()
        {
        }
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
                if (strFlag.Substring(0, 3) == "COM")
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        Instance = new SerialPort4Linux();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        Instance = new SerialPort();
                }
                if (strFlag.Substring(0, 2) == "TS" || strFlag.Substring(0, 2) == "TC")
                    Instance = new GWNetPort();
            }
            if (Instance == null)
            {
                if (!bFlag)
                {
                    string msg = string.Format("{0}号设备数据库通讯参数配置有误,无法生成通讯实例,请查看!", item.iEquipno);
                    MessageService.AddMessage(MessageLevel.Fatal, msg, 0, false);
                    bFlag = true;
                }
                return false;
            }
            else
            {
                bFlag = false;
            }
            try
            {
                flag = Instance.Initialize(item);
                if (item.communication_drv.ToUpper().Contains("DATASIMU.NET.DLL"))
                    flag = true;
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Debug, e.ToString(), 0, false);
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
                MessageService.AddMessage(MessageLevel.Debug, e.ToString(), 0, false);
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
                MessageService.AddMessage(MessageLevel.Debug, e.ToString(), 0, false);
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
                MessageService.AddMessage(MessageLevel.Debug, e.ToString(), 0, false);
            }
        }
        public void Dispose()
        {
            lock (StationItem.EquipCategoryDict)
            {
                if (!StationItem.EquipCategoryDict.ContainsKey(szLocal_addr))
                {
                    Instance.Dispose();
                }
            }
        }
    }
}
