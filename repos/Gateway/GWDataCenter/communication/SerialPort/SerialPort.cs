﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace GWDataCenter
{
    public class SZ_SerialPort : ICommunication, IDisposable
    {
        int equipno;
        EquipItem Equip;
        public int EquipNo
        {
            get
            {
                return equipno;
            }
            set
            {
                equipno = value;
            }
        }
        public int CommFaultReTryTime
        {
            get
            {
                return commFaultReTryTime;
            }
            set
            {
                commFaultReTryTime = value;
            }
        }
        int commFaultReTryTime;
        public int CommWaitTime
        {
            get
            {
                return commWaitTime;
            }
            set
            {
                commWaitTime = value;
            }
        }
        int commWaitTime = 100;
        private System.IO.Ports.SerialPort MyserialPort = null;
        string[] ss = System.IO.Ports.SerialPort.GetPortNames();
        string portName;
        string param, timeparam;
        bool bSerialError = false;
        public SZ_SerialPort()
        {
            if (MyserialPort == null)
                MyserialPort = new System.IO.Ports.SerialPort();
        }
        public void Dispose()
        {
            if (MyserialPort != null)
            {
                if (MyserialPort.IsOpen)
                    MyserialPort.Close();
                MyserialPort.Dispose();
            }
        }
        public System.IO.Ports.SerialPort GetSerialPort()
        {
            return MyserialPort;
        }
        public bool VerifyPortNm(string portName)
        {
            string s = @"[cC][oO][mM][1-9]\d{0,}";
            if (General.VerifyStringFormat(s, portName))
                return true;
            else
            {
                MessageService.AddMessage(MessageLevel.Fatal, ResourceService.GetString("AlarmCenter.DataCenter.SerialPort.Error1"), equipno, false);
                return false;
            }
        }
        public bool VerifyParam(string param)
        {
            string s = @"[\d]+/[\d]/[\d]/[\w]";
            if (General.VerifyStringFormat(s, param))
                return true;
            else
            {
                MessageService.AddMessage(MessageLevel.Fatal, ResourceService.GetString("AlarmCenter.DataCenter.SerialPort.Error2"), equipno, false);
                return false;
            }
        }
        public bool VerifyTimeParam(string param)
        {
            string s = @"[\d]+/[\d]+/[\d]+/[\d]+";
            if (General.VerifyStringFormat(s, param))
                return true;
            else
            {
                MessageService.AddMessage(MessageLevel.Fatal, ResourceService.GetString("AlarmCenter.DataCenter.SerialPort.Error3"), equipno, false);
                return false;
            }
        }
        string GetPortNm(int ista, int ieqp)
        {
            try
            {
                return StationItem.db_Eqp.Single(m => m.sta_n == ista && m.equip_no == ieqp).local_addr;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        string GetParam(int ista, int ieqp)
        {
            try
            {
                return StationItem.db_Eqp.Single(m => m.sta_n == ista && m.equip_no == ieqp).communication_param;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        string GetTimeParam(int ista, int ieqp)
        {
            try
            {
                return StationItem.db_Eqp.Single(m => m.sta_n == ista && m.equip_no == ieqp).communication_time_param;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool Initialize(EquipItem item)
        {
            /*为了提高串口的可靠性（有时候串口本身有问题），强制要求每次通讯都初始化。
            if (portName == item.Local_addr && param == item.communication_param && timeparam == item.communication_time_param)
                return true;
             */
            /*
            RetStr.Clear();
            MyserialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(MyserialPort_DataReceived);
            MyserialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(MyserialPort_DataReceived);
            MyBufLen = 0;
            */
            portName = item.Local_addr;
            param = item.communication_param;
            timeparam = item.communication_time_param;
            equipno = item.iEquipno;
            Equip = item;
            if (VerifyPortNm(portName))
            {
                portName = portName.Trim().ToUpper();
                if (!ss.Contains(portName))
                {
                    if (!bSerialError)
                    {
                        MessageService.AddMessage(MessageLevel.Fatal, portName + ResourceService.GetString("AlarmCenter.DataCenter.SerialPort.Error4"), equipno, false);
                        bSerialError = true;
                    }
                    return false;
                }
                else if (IsOpen)
                {
                    Close();
                }
                MyserialPort.PortName = portName.Trim().ToUpper();
            }
            else
                return false;
            if (VerifyParam(param))
            {
                string[] ParamArray;
                ParamArray = param.Split('/');
                MyserialPort.BaudRate = Convert.ToInt32(ParamArray[0]);
                MyserialPort.DataBits = Convert.ToInt32(ParamArray[1]);
                ParamArray[2] = ParamArray[2].Trim().ToUpper();
                switch (ParamArray[2])
                {
                    case "0":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case "1":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case "1.5":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case "2":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        return false;
                }
                ParamArray[3] = ParamArray[3].Trim().ToUpper();
                switch (ParamArray[3])
                {
                    case "NO":
                        MyserialPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "EVEN":
                        MyserialPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "ODD":
                        MyserialPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case "MARK":
                        MyserialPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                    case "SPACE":
                        MyserialPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                    default:
                        return false;
                }
            }
            else
                return false;
            if (VerifyTimeParam(timeparam))
            {
                string[] TimeParamArray;
                TimeParamArray = timeparam.Split('/');
                MyserialPort.WriteTimeout = Convert.ToInt32(TimeParamArray[0]);
                MyserialPort.ReadTimeout = Convert.ToInt32(TimeParamArray[0]);
                commFaultReTryTime = Convert.ToInt32(TimeParamArray[2]);
                commWaitTime = Convert.ToInt32(TimeParamArray[3]);
            }
            else
            {
                MyserialPort.WriteTimeout = 1000;
                MyserialPort.ReadTimeout = 1000;
                commFaultReTryTime = 3;
                commWaitTime = 500;
            }
            MyserialPort.RtsEnable = true;
            MyserialPort.DtrEnable = true;
            if (IsOpen)
            {
                MyserialPort.DiscardInBuffer();
                MyserialPort.DiscardOutBuffer();
            }
            return true;
        }
        public bool Initialize(string PortName, string Param, int iequipno)
        {
            portName = PortName;
            param = Param;
            equipno = iequipno;
            if (VerifyPortNm(portName))
                MyserialPort.PortName = portName.Trim().ToUpper();
            else
                return false;
            if (!ss.Contains(MyserialPort.PortName))
            {
                if (!bSerialError)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, MyserialPort.PortName + ResourceService.GetString("AlarmCenter.DataCenter.SerialPort.Error4"), equipno, false);
                    bSerialError = true;
                }
                return false;
            }
            else if (MyserialPort.IsOpen)
            {
                if (!bSerialError)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, MyserialPort.PortName + ResourceService.GetString("AlarmCenter.DataCenter.SerialPort.Error5"), equipno, false);
                    bSerialError = true;
                }
                return false;
            }
            if (VerifyParam(param))
            {
                string[] ParamArray;
                ParamArray = param.Split('/');
                MyserialPort.BaudRate = Convert.ToInt32(ParamArray[0]);
                MyserialPort.DataBits = Convert.ToInt32(ParamArray[1]);
                ParamArray[2] = ParamArray[2].Trim().ToUpper();
                switch (ParamArray[2])
                {
                    case "0":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case "1":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case "1.5":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case "2":
                        MyserialPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        return false;
                }
                ParamArray[3] = ParamArray[3].Trim().ToUpper();
                MyserialPort.RtsEnable = true;
                MyserialPort.DtrEnable = true;
                switch (ParamArray[3])
                {
                    case "NO":
                        MyserialPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "EVEN":
                        MyserialPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "ODD":
                        MyserialPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case "MARK":
                        MyserialPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                    case "SPACE":
                        MyserialPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                    default:
                        return false;
                }
            }
            else
                return false;
            return true;
        }
        public bool Open()
        {
            try
            {
                if (MyserialPort.IsOpen)
                    MyserialPort.Close();
                System.Threading.Thread.Sleep(20);
                MyserialPort.Open();
                System.Threading.Thread.Sleep(20);
                MyserialPort.DiscardInBuffer();
                MyserialPort.DiscardOutBuffer();
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Debug, General.GetExceptionInfo(e), equipno, false);
                return false;
            }
            return true;
        }
        public void Close()
        {
            MyserialPort.Close();
        }
        public void Write(byte[] buffer, int offset, int count)
        {
            if (!ss.Contains(portName))
                return;
            string msg = "";
            try
            {
                if (!IsOpen)
                {
                    Open();
                    System.Threading.Thread.Sleep(20);
                }
                if (IsOpen)
                    MyserialPort.Write(buffer, offset, count);
                MyserialPort.DiscardOutBuffer();
            }
            catch (Exception ex)
            {
                MessageService.AddMessage(MessageLevel.Debug, General.GetExceptionInfo(ex), equipno, false);
            }
            if (Equip != null)
            {
                if (Equip.IsDebug)
                {
                    msg = portName + ">>Write" + ">>";
                    msg += System.Text.Encoding.Default.GetString(buffer, offset, count);
                    msg += "(";
                    for (int k = 0; k < count; k++)
                    {
                        msg += buffer[k + offset];
                        msg += "/";
                    }
                    msg += ")";
                    MessageService.AddMessage(MessageLevel.Debug, msg, equipno);
                }
            }
        }
        public int ReadList(List<byte[]> list_buffer)
        {
            return 0;
        }
        public int Read(byte[] buffer, int offset, int count)
        {
            if (!ss.Contains(portName))
                return 0;
            string msg = "";
            int iRet = 0;
            try
            {
                if (!IsOpen)
                {
                    Open();
                    System.Threading.Thread.Sleep(20);
                }
                iRet = MyserialPort.Read(buffer, offset, count);
                MyserialPort.DiscardInBuffer();
            }
            catch (System.TimeoutException e)
            {
                msg += "Timeout  ";
            }
            catch (Exception ex)
            {
                MessageService.AddMessage(MessageLevel.Debug, General.GetExceptionInfo(ex), equipno, false);
            }
            if (Equip != null)
            {
                if (Equip.IsDebug)
                {
                    msg += portName + ">>Read" + "[" + iRet + "/" + count + "]" + ">>";
                    msg += System.Text.Encoding.Default.GetString(buffer, offset, iRet);
                    msg += "(";
                    for (int k = 0; k < iRet; k++)
                    {
                        msg += buffer[k + offset];
                        msg += "/";
                    }
                    msg += ")";
                    MessageService.AddMessage(MessageLevel.Debug, msg.ToString(), equipno);
                }
            }
            return iRet;
        }
        public bool IsOpen
        {
            get
            {
                return MyserialPort.IsOpen;
            }
        }
    }
}
