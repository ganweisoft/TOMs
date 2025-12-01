﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.Extensions.DependencyInjection;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace GWDataCenter
{
    public enum MessageLevel
    {
        Wubao = -1, Info = 0, SpecalInfo = 9999, Debug = 10000, SetParm, ZiChan, Warn, Error, Fatal
    }
    public enum LogType
    {
        Error, Debug, Config
    }
    public enum ClientType
    {
        WinPC, Linux, Mac, WebServer, iPhone, iPad, iPod, WP, Android, Dll
    }
    public class RealTimeEventItem
    {
        int m_level;
        string m_EventMsg;
        string m_Proc_advice_Msg;
        int m_equipno;
        string m_type;
        int m_ycyxno;
        string m_related_pic;
        DateTime DT;
        bool bconfirmed;
        string user_confirm;
        DateTime dt_confirm = new DateTime(1970, 1, 1);
        string m_guid;
        public string Related_video;
        public string ZiChanID;
        public string PlanNo;
        string wavefile;
        public int Level
        {
            get
            {
                return m_level;
            }
            set
            {
                m_level = value;
            }
        }
        public string EventMsg
        {
            get
            {
                return m_EventMsg;
            }
            set
            {
                m_EventMsg = value;
            }
        }
        public string Proc_advice_Msg
        {
            get
            {
                return m_Proc_advice_Msg;
            }
            set
            {
                m_Proc_advice_Msg = value;
            }
        }
        public string Related_pic
        {
            get
            {
                return m_related_pic;
            }
            set
            {
                m_related_pic = value;
            }
        }
        public string Wavefile
        {
            get
            {
                return wavefile;
            }
            set
            {
                wavefile = value;
            }
        }
        public int Equipno
        {
            get
            {
                return m_equipno;
            }
            set
            {
                m_equipno = value;
            }
        }
        public string Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
        public int Ycyxno
        {
            get
            {
                return m_ycyxno;
            }
            set
            {
                m_ycyxno = value;
            }
        }
        public DateTime Time
        {
            get
            {
                return DT;
            }
            set
            {
                DT = value;
            }
        }
        public bool bConfirmed
        {
            get
            {
                return bconfirmed;
            }
            set
            {
                bconfirmed = value;
            }
        }
        public string User_confirm
        {
            get
            {
                return user_confirm;
            }
            set
            {
                user_confirm = value;
            }
        }
        public DateTime DT_Confirm
        {
            get
            {
                return dt_confirm;
            }
            set
            {
                dt_confirm = value;
            }
        }
        public string GUID
        {
            get
            {
                return m_guid;
            }
            set
            {
                m_guid = value;
            }
        }
        public RealTimeEventItem()
        {
        }
        public RealTimeEventItem(RealTimeEventItem item)
        {
            m_level = item.Level;
            m_EventMsg = item.EventMsg;
            m_Proc_advice_Msg = item.Proc_advice_Msg;
            m_related_pic = item.Related_pic;
            m_equipno = item.Equipno;
            m_type = item.Type;
            m_ycyxno = item.Ycyxno;
            DT = item.Time;
            wavefile = item.Wavefile;
            dt_confirm = item.DT_Confirm;
            user_confirm = item.User_confirm;
            bconfirmed = item.bConfirmed;
            Related_video = item.Related_video;
            ZiChanID = item.ZiChanID;
            PlanNo = item.PlanNo;
            GUID = item.GUID;
        }
        public RealTimeEventItem(int level, string msgstr, int equipno, string guid)
        {
            m_level = level;
            m_EventMsg = msgstr;
            m_Proc_advice_Msg = "";
            m_equipno = equipno;
            DT = General.Convert2DT(DateTime.Now);
            m_guid = guid;
        }
        public RealTimeEventItem(int level, string msgstr, string advicestr, int equipno, string type, int ycyxno, string wav, string related_pic, string related_v, string ZCID, string PN, string guid)
        {
            m_level = level;
            m_EventMsg = msgstr;
            m_Proc_advice_Msg = advicestr;
            m_equipno = equipno;
            m_type = type;
            m_related_pic = related_pic;
            m_ycyxno = ycyxno;
            DT = General.Convert2DT(DateTime.Now);
            wavefile = wav;
            m_guid = guid;
            Related_video = related_v;
            ZiChanID = ZCID;
            PlanNo = PN;
        }
    }
    public static class MessageService
    {
        static ObservableCollection<RealTimeEventItem> EventList = new ObservableCollection<RealTimeEventItem>();
        static EquipItem equip = null;
        static object idebugcount = 0;
        static object iinfocount = 0;
        static int iDebugCount
        {
            get
            {
                lock (idebugcount)
                {
                    return Convert.ToInt32(idebugcount);
                }
            }
            set
            {
                lock (idebugcount)
                {
                    idebugcount = value;
                }
            }
        }
        static MessageService()
        {
        }
        static void GetMaxCountFromLevel(out int iCount, out int iMin, out int iMax, int iLevel)
        {
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    foreach (GWSnapshotConfigTableRow r in db.GWSnapshotConfigTable)
                    {
                        iMin = r.SnapshotLevelMin;
                        iMax = r.SnapshotLevelMax;
                        if (iLevel >= iMin && iLevel <= iMax)
                        {
                            iCount = r.MaxCount;
                            return;
                        }
                    }
                    iMin = 0;
                    iMax = 0;
                    iCount = -1;
                }
            }
        }
        public static ObservableCollection<RealTimeEventItem> GetEventList()
        {
            lock (EventList)
            {
                return EventList;
            }
        }
        public static void DeleteDebugInfo(int iEquipNo)
        {
            lock (EventList)
            {
                for (int k = 0; k < EventList.Count; k++)
                {
                    if (EventList[k].Equipno == iEquipNo && EventList[k].Level == (int)MessageLevel.Debug)
                    {
                        if (iDebugCount > 0)
                            iDebugCount--;
                        EventList.Remove(EventList[k]);
                    }
                }
            }
        }
        static string oldmsg = "";
        public static void AddMessage(MessageLevel level, string msgstr, int equipno, bool CanRepeat = true)
        {
            string guid = Guid.NewGuid().ToString();
            AddMessagehelp(level, msgstr, equipno, CanRepeat, guid);
        }
        public static void AddMessage(string guid, MessageLevel level, string msgstr, int equipno, bool CanRepeat = true)
        {
            AddMessagehelp(level, msgstr, equipno, CanRepeat, guid);
        }
        static void AddMessagehelp(MessageLevel level, string msgstr, int equipno, bool CanRepeat, string guid)
        {
            if (level == MessageLevel.Fatal || level == MessageLevel.Error)
            {
                if (msgstr != oldmsg)
                {
                    DataCenter.WriteLogFile(msgstr);
                }
            }
            if (string.IsNullOrEmpty(msgstr))
                return;
            if (equipno == 0 && level == MessageLevel.Debug)
                return;
            equip = DataCenter.GetEquipItem(equipno);
            if (equip != null)
            {
                if (!Convert.ToBoolean(equip.IsDebug) && level == MessageLevel.Debug)
                {
                    return;
                }
                if (level == MessageLevel.Debug)
                {
                    string msg = string.Format("设备号：{0}---设备名：{1}\r\n {2}", equipno, equip.Equip_nm, msgstr);
                    DataCenter.WriteLogFile(msg, LogType.Debug);
                    return;
                }
                int iMaxCount, iMinLevel, iMaxLevel;
                GetMaxCountFromLevel(out iMaxCount, out iMinLevel, out iMaxLevel, (int)level);
                lock (EventList)
                {
                    var result = from m in EventList orderby m.Time where (m.Level >= iMinLevel && m.Level <= iMaxLevel) select m;
                    List<RealTimeEventItem> resultlist = result.ToList();
                    while ((iMaxCount != -1) && (resultlist.Count >= iMaxCount))
                    {
                        EventList.Remove(resultlist[0]);
                        resultlist.Remove(resultlist[0]);
                    }
                }
            }
            try
            {
                RealTimeEventItem r = new RealTimeEventItem((int)level, msgstr, equipno, guid);
                if (equipno == 0)
                {
                    if (level == MessageLevel.Fatal || level == MessageLevel.Error || level == MessageLevel.Warn)
                    {
                        if (msgstr != oldmsg)
                        {
                            InsertMsg2SysEvt(msgstr, r.GUID);
                        }
                    }
                    else
                    {
                        InsertMsg2SysEvt(msgstr, r.GUID);
                    }
                }
                lock (EventList)
                {
                    if (!CanRepeat)
                    {
                        foreach (RealTimeEventItem e in EventList)
                        {
                            if (e.Equipno == equipno)
                            {
                                if (e.EventMsg == msgstr)
                                {
                                    EventList.Remove(e);
                                    break;
                                }
                            }
                        }
                    }
                    EventList.Add(r);
                }
                oldmsg = msgstr;
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        public static void InsertMsg2SysEvt(string msgstr, string guid)
        {
            string msg = msgstr;
            msg = msg.Replace('\'', '"');
            StationItem.MyDBGueueSave2Table.AddSysEvtTableRow2List(new SysEvtTableRow
            {
                gwEvent = msg,
                GUID = guid,
                time = General.Convert2DT(DateTime.Now),
                confirmtime = new DateTime(2000, 1, 1)
            });
        }
        public static void DelMessage(string msgstr, int equipno)
        {
            lock (EventList)
            {
                for (int k = EventList.Count - 1; k >= 0; k--)
                {
                    if (EventList[k].Equipno == equipno || equipno == 0)
                    {
                        EventList.RemoveAt(k);
                    }
                }
            }
        }
        public static void DelMessage4Equip(int equipno)
        {
            lock (EventList)
            {
                for (int k = EventList.Count - 1; k >= 0; k--)
                {
                    if (EventList[k].Equipno == equipno)
                    {
                        EventList.RemoveAt(k);
                    }
                }
            }
        }
        public static void DelAllMessageAndSetParm()
        {
            lock (EventList)
            {
                for (int k = EventList.Count - 1; k >= 0; k--)
                {
                    if ((EventList[k].Level == (int)MessageLevel.Info) || (EventList[k].Level == (int)MessageLevel.SetParm))
                    {
                        EventList.RemoveAt(k);
                    }
                }
            }
        }
        public static void AddZCMessage(MessageLevel level, string msgstr, int equipno, string related_pic, bool CanRepeat = true)
        {
            try
            {
                int iMaxCount, iMinLevel, iMaxLevel;
                GetMaxCountFromLevel(out iMaxCount, out iMinLevel, out iMaxLevel, (int)MessageLevel.ZiChan);
                RealTimeEventItem r = new RealTimeEventItem((int)level, msgstr, null, equipno, null, 0, null, related_pic, null, null, null, null);
                lock (EventList)
                {
                    EventList.Add(r);
                    var result = from m in EventList orderby m.Time where (m.Level >= iMinLevel && m.Level <= iMaxLevel) select m;
                    List<RealTimeEventItem> resultlist = result.ToList();
                    while ((iMaxCount != -1) && (resultlist.Count >= iMaxCount))
                    {
                        EventList.Remove(resultlist[0]);
                        resultlist.Remove(resultlist[0]);
                    }
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        public static void AddMessage(MessageLevel level, string msgstr, string msgstr1, int equipno, string type, int ycyxno, string wav, string related_pic, string related_v, string ZCID, string PN, string guid)
        {
            Edit_AddMessage((int)level, msgstr, msgstr1, equipno, type, ycyxno, wav, related_pic, related_v, ZCID, PN, guid);
        }
        public static void Edit_AddMessage(int level, string msgstr, string msgstr1, int equipno, string type, int ycyxno, string wav, string related_pic, string related_v, string ZCID, string PN, string guid)
        {
            try
            {
                int iMaxCount, iMinLevel, iMaxLevel;
                GetMaxCountFromLevel(out iMaxCount, out iMinLevel, out iMaxLevel, level);
                lock (EventList)
                {
                    var result = from m in EventList orderby m.Time where (m.Level >= iMinLevel && m.Level <= iMaxLevel) select m;
                    List<RealTimeEventItem> resultlist = result.ToList();
                    while ((iMaxCount != -1) && (resultlist.Count >= iMaxCount))
                    {
                        EventList.Remove(resultlist[0]);
                        resultlist.Remove(resultlist[0]);
                    }
                }
                RealTimeEventItem r = new RealTimeEventItem(level, msgstr, msgstr1, equipno, type, ycyxno, wav, related_pic, related_v, ZCID, PN, guid);
                lock (EventList)
                {
                    foreach (RealTimeEventItem e in EventList)
                    {
                        if (e.Equipno == equipno)
                        {
                            if (e.Type == type && e.Ycyxno == ycyxno)
                            {
                                EventList.Remove(e);
                                break;
                            }
                        }
                    }
                    EventList.Add(r);
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        public static void AddConfirmedMessage(RealTimeEventItem ConfirmRealTimeEventItem)
        {
            try
            {
                lock (EventList)
                {
                    foreach (RealTimeEventItem e in EventList)
                    {
                        if (e.Equipno == ConfirmRealTimeEventItem.Equipno)
                        {
                            if (e.Equipno > 0 && e.Ycyxno == ConfirmRealTimeEventItem.Ycyxno && e.Type == ConfirmRealTimeEventItem.Type)
                            {
                                if (e.EventMsg == ConfirmRealTimeEventItem.EventMsg)
                                {
                                    EventList.Remove(e);
                                    break;
                                }
                            }
                            else
                            {
                                if (e.EventMsg == ConfirmRealTimeEventItem.EventMsg && e.Ycyxno == ConfirmRealTimeEventItem.Ycyxno)
                                {
                                    EventList.Remove(e);
                                    break;
                                }
                            }
                        }
                    }
                    EventList.Add(ConfirmRealTimeEventItem);
                }
                if (ConfirmRealTimeEventItem.Proc_advice_Msg.Length > 255)
                    ConfirmRealTimeEventItem.Proc_advice_Msg = ConfirmRealTimeEventItem.Proc_advice_Msg.Substring(0, 255);
                bool iswubao = ConfirmRealTimeEventItem.Level == (int)MessageLevel.Wubao ? true : false;
                string SQL = "";
                if (ConfirmRealTimeEventItem.Equipno == 0 || ConfirmRealTimeEventItem.Equipno == -1)
                {
                    lock (GWDbProvider.lockstate)
                    {
                        using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                        {
                            var item = db.SysEvtTable.Single(m => (m.GUID == ConfirmRealTimeEventItem.GUID));
                            item.confirmname = ConfirmRealTimeEventItem.User_confirm;
                            item.confirmtime = ConfirmRealTimeEventItem.DT_Confirm;
                            item.confirmremark = ConfirmRealTimeEventItem.Proc_advice_Msg;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    lock (GWDbProvider.lockstate)
                    {
                        using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                        {
                            try
                            {
                                var item = db.YcYxEvtTable.Single(m => (m.GUID == ConfirmRealTimeEventItem.GUID));
                                item.confirmname = ConfirmRealTimeEventItem.User_confirm;
                                item.confirmtime = ConfirmRealTimeEventItem.DT_Confirm;
                                item.confirmremark = ConfirmRealTimeEventItem.Proc_advice_Msg;
                                item.WuBao = iswubao;
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                var item = db.SysEvtTable.Single(m => (m.GUID == ConfirmRealTimeEventItem.GUID));
                                item.confirmname = ConfirmRealTimeEventItem.User_confirm;
                                item.confirmtime = ConfirmRealTimeEventItem.DT_Confirm;
                                item.confirmremark = ConfirmRealTimeEventItem.Proc_advice_Msg;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        public static void AddConfirmedMessage1(int EquipNo, int YcYxNo, string EvtStr, int Level, string ConfirmStr, string ConfirmDT, string Proc_advice_Msg)
        {
            try
            {
                RealTimeEventItem ConfirmRealTimeEventItem = new RealTimeEventItem();
                lock (EventList)
                {
                    foreach (RealTimeEventItem e in EventList)
                    {
                        if (e.Equipno == EquipNo)
                        {
                            if (e.Equipno > 0 && e.Ycyxno == ConfirmRealTimeEventItem.Ycyxno && e.Type == ConfirmRealTimeEventItem.Type)
                            {
                                ConfirmRealTimeEventItem.bConfirmed = true;
                                ConfirmRealTimeEventItem.DT_Confirm = DateTime.Parse(ConfirmDT);
                                ConfirmRealTimeEventItem.Equipno = EquipNo;
                                ConfirmRealTimeEventItem.EventMsg = EvtStr;
                                ConfirmRealTimeEventItem.Level = Level;
                                ConfirmRealTimeEventItem.Proc_advice_Msg = Proc_advice_Msg;
                                ConfirmRealTimeEventItem.Related_pic = e.Related_pic;
                                ConfirmRealTimeEventItem.Time = e.Time;
                                ConfirmRealTimeEventItem.User_confirm = ConfirmStr;
                                ConfirmRealTimeEventItem.Wavefile = e.Wavefile;
                                ConfirmRealTimeEventItem.Ycyxno = e.Ycyxno;
                                ConfirmRealTimeEventItem.Related_video = e.Related_video;
                                ConfirmRealTimeEventItem.ZiChanID = e.ZiChanID;
                                ConfirmRealTimeEventItem.PlanNo = e.PlanNo;
                                EventList.Remove(e);
                                break;
                            }
                            else
                            {
                                if (e.EventMsg == EvtStr && e.Ycyxno == YcYxNo)
                                {
                                    ConfirmRealTimeEventItem.bConfirmed = true;
                                    ConfirmRealTimeEventItem.DT_Confirm = DateTime.Parse(ConfirmDT);
                                    ConfirmRealTimeEventItem.Equipno = EquipNo;
                                    ConfirmRealTimeEventItem.EventMsg = EvtStr;
                                    ConfirmRealTimeEventItem.Level = Level;
                                    ConfirmRealTimeEventItem.Proc_advice_Msg = Proc_advice_Msg;
                                    ConfirmRealTimeEventItem.Related_pic = e.Related_pic;
                                    ConfirmRealTimeEventItem.Time = e.Time;
                                    ConfirmRealTimeEventItem.User_confirm = ConfirmStr;
                                    ConfirmRealTimeEventItem.Wavefile = e.Wavefile;
                                    ConfirmRealTimeEventItem.Ycyxno = e.Ycyxno;
                                    ConfirmRealTimeEventItem.Related_video = e.Related_video;
                                    ConfirmRealTimeEventItem.ZiChanID = e.ZiChanID;
                                    ConfirmRealTimeEventItem.PlanNo = e.PlanNo;
                                    EventList.Remove(e);
                                    break;
                                }
                            }
                        }
                    }
                    EventList.Add(ConfirmRealTimeEventItem);
                }
                if (ConfirmRealTimeEventItem.Proc_advice_Msg.Length > 255)
                    ConfirmRealTimeEventItem.Proc_advice_Msg = ConfirmRealTimeEventItem.Proc_advice_Msg.Substring(0, 255);
                string SQL = "";
                if (ConfirmRealTimeEventItem.Equipno == 0 || ConfirmRealTimeEventItem.Equipno == -1)
                {
                    lock (GWDbProvider.lockstate)
                    {
                        using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                        {
                            var item = db.SysEvtTable.Single(m => (m.time == ConfirmRealTimeEventItem.Time && m.gwEvent == ConfirmRealTimeEventItem.EventMsg));
                            item.confirmname = ConfirmRealTimeEventItem.User_confirm;
                            item.confirmtime = ConfirmRealTimeEventItem.DT_Confirm;
                            item.confirmremark = ConfirmRealTimeEventItem.Proc_advice_Msg;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        var item = db.YcYxEvtTable.Single(m => (m.equip_no == ConfirmRealTimeEventItem.Equipno && m.ycyx_no == ConfirmRealTimeEventItem.Ycyxno && m.ycyx_type == ConfirmRealTimeEventItem.Type && m.time == ConfirmRealTimeEventItem.Time));
                        item.confirmname = ConfirmRealTimeEventItem.User_confirm;
                        item.confirmtime = ConfirmRealTimeEventItem.DT_Confirm;
                        item.confirmremark = ConfirmRealTimeEventItem.Proc_advice_Msg;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
    }
}
