﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenGWDataCenter.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace GWDataCenter
{
    public enum EquipState
    {
        NoCommunication = 0,
        CommunicationOK = 1,
        HaveAlarm = 2,
        HaveSetParm = 3,
        Initial = 4,
        CheFang = 5,
        BackUp = 6
    }
    public enum DllLoginReturnInfo
    {
        OK, ServerNoStart, OverClientConnectMax, CertificateError, OtherError
    }
    public class RemoteRegisterData
    {
        public int iEquipNo;
        public string strType;
        public int iYCYxNo;
    }
    public class SetParmItem
    {
        public int iEquipNo;
        public int iSetNo;
        public string strValue;
        public bool bShowDlg;
    }
    public class LockSetParmMsg
    {
        public int iEquipNo;
        public int iSetNo;
        public string strValue;
        public string strMsg;
    }
    public class StopSetParmEventArgs : EventArgs
    {
        public int iEqpNo { get; set; }
        public int iSetNo { get; set; }
    }
    public static class StationItem
    {
        static public Dictionary<string, object> EquipCategoryDict = new Dictionary<string, object>();
        static public Dictionary<string, List<EquipTableRow>> SubEquipListDict;
        static public DataTable EqpDt;
        static public event EventHandler SetParmResultEvent;
        static public event EventHandler AppClose;
        static public event EventHandler RightHandWave;
        static public event EventHandler LeftHandWave;
        static public event EventHandler StationCommError;
        static public event EventHandler StationCommOk;
        static public event EventHandler StationHaveAlarm;
        static public event EventHandler StationNoAlarm;
        static public event EventHandler HaveEquipChanged;
        static public event EventHandler HaveEquipReset;
        static public event EventHandler StopSetParmEvent;
        static object resetequip = false;
        static UTF8Encoding enc = new UTF8Encoding();
        static RSA crypt = System.Security.Cryptography.RSA.Create();
        static public bool haveXMLfile = false;
        static public object CanMonitorEqps = 0;
        static public object CanMonitorYcYxs = 0;
        static public object ClientNum = 0;
        static public DateTime sysRunTime = DateTime.Now;
        static public bool bNoXMLAlarm = false;
        static public bool bYcYxNoMonitorAlarm = false;
        static public bool bEquipsNoMonitorAlarm = false;
        static public bool bDataLimitAlarm = false;
        static public bool bAlarmAutoJump = false;
        static readonly string PageJumpProperty = "AlarmCenter.Page";
        static public readonly string CurveProperty = "AlarmCenter.Gui.OptionPanels.CurveOptions";
        static public string CurveRootPathName = @"d:\AlarmCenter\CurveData";
        static public string CurveRemoteSiteUser;
        static public string CurveRemoteSitePwd;
        static public bool bCurveRemoteSite = false;
        static public bool bCurveEquipSaveType = false;
        static public bool bCurveStoreInDB = false;
        static public int iHistory_CurveStoreTime = 365;
        static readonly string WeihuProperty = "AlarmCenter.Gui.OptionPanels.WeihuOptions";
        static public string WeihuPictureRootPathName = @"c:\AlarmCenter\WeihuPicture";
        static bool bEval = false;
        static List<EquipTableRow> db_eqp;
        static List<YcpTableRow> db_ycp;
        static List<YxpTableRow> db_yxp;
        static List<SetParmTableRow> db_setparm;
        static List<GWDataRecordItemsTableRow> db_recorditems;
        static List<AdministratorTableRow> db_adminForAlarm;
        static readonly object db_eqp_lock = true;
        static readonly object db_ycp_lock = true;
        static readonly object db_yxp_lock = true;
        static readonly object db_setparm_lock = true;
        static readonly object db_recorditems_lock = true;
        static readonly object db_adminForAlarm_lock = true;
        public static GWDbProvider MyGWDbProvider = new GWDbProvider();
        public static DBGueueSave2Table MyDBGueueSave2Table = new DBGueueSave2Table();
        static public int m_ClientNum
        {
            get
            {
                lock (ClientNum)
                {
                    return (int)ClientNum;
                }
            }
            set
            {
                lock (ClientNum)
                {
                    ClientNum = value;
                }
            }
        }
        static public List<EquipTableRow> db_Eqp
        {
            get
            {
                lock (db_eqp_lock)
                {
                    return db_eqp;
                }
            }
            set
            {
                lock (db_eqp_lock)
                {
                    db_eqp = value;
                }
            }
        }
        static public List<YcpTableRow> db_Ycp
        {
            get
            {
                lock (db_ycp_lock)
                {
                    return db_ycp;
                }
            }
            set
            {
                lock (db_ycp_lock)
                {
                    db_ycp = value;
                }
            }
        }
        static public List<YxpTableRow> db_Yxp
        {
            get
            {
                lock (db_yxp_lock)
                {
                    return db_yxp;
                }
            }
            set
            {
                lock (db_yxp_lock)
                {
                    db_yxp = value;
                }
            }
        }
        static public List<SetParmTableRow> db_Setparm
        {
            get
            {
                lock (db_setparm_lock)
                {
                    return db_setparm;
                }
            }
            set
            {
                lock (db_setparm_lock)
                {
                    db_setparm = value;
                }
            }
        }
        static public List<GWDataRecordItemsTableRow> db_RecordItems
        {
            get
            {
                lock (db_recorditems_lock)
                {
                    return db_recorditems;
                }
            }
            set
            {
                lock (db_recorditems_lock)
                {
                    db_recorditems = value;
                }
            }
        }
        static public List<AdministratorTableRow> db_AdminForAlarm
        {
            get
            {
                lock (db_adminForAlarm_lock)
                {
                    return db_adminForAlarm;
                }
            }
            set
            {
                lock (db_adminForAlarm_lock)
                {
                    db_adminForAlarm = value;
                }
            }
        }
        public static void FireSetParmResultEvent(SetItem item)
        {
            StationItem.SetParmResultEvent?.Invoke(item, new EventArgs());
        }
        public static void FireAppCloseEvent()
        {
            StationItem.AppClose?.Invoke(null, new EventArgs());
        }
        public static void DoHaveEquipChanged(SubEquipList SE)
        {
            HaveEquipChanged?.Invoke(SE, new EventArgs());
        }
        public static void DoHaveEquipReset(List<int> EqpNoList)
        {
            HaveEquipReset?.Invoke(EqpNoList, new EventArgs());
        }
        public static void SopSetParm(int EqpNo, int SetNo)
        {
            StopSetParmEvent?.Invoke(null, new StopSetParmEventArgs
            {
                iEqpNo = EqpNo,
                iSetNo = SetNo
            });
        }
        static public Dictionary<string, List<EquipTableRow>> GetSubEquipListDataRow(List<EquipTableRow> Rows)
        {
            Dictionary<string, List<EquipTableRow>> DS = new Dictionary<string, List<EquipTableRow>>();
            foreach (EquipTableRow s in Rows)
            {
                string key = s.local_addr.ToUpper().Trim();
                if (DS.ContainsKey(key))
                {
                    DS[key].Add(s);
                }
                else
                {
                    List<EquipTableRow> list = new List<EquipTableRow>();
                    list.Add(s);
                    DS.Add(key, list);
                }
            }
            return DS;
        }
        public static bool init()
        {
            string configDirectory;
            Assembly exe = typeof(StationItem).Assembly;
            string ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
            configDirectory = Path.Combine(ApplicationRootPath,
                                                    "data/AlarmCenter");
            PropertyService.InitializeService(configDirectory, Path.Combine(ApplicationRootPath, "data"),
                                              "AlarmCenterProperties");
            PropertyService.Load();
            ResourceService.InitializeService();
            Console.WriteLine("启动资源服务");
            DataCenter.brunning = true;
            MyGWDbProvider.Initialize<GWDataContext>();
            Console.WriteLine("初始化数据库");
            DataCenter.RunEnvironment = AppEnvironment.Server;
            Properties properties = PropertyService.Get(CurveProperty, new Properties());
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                CurveRootPathName = Path.Combine(General.GetApplicationRootPath(), "CurveData");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CurveRootPathName = properties.Get("Hostory_CurveStorePath", @"d:\IoTCenter\CurveData");
            bCurveRemoteSite = (properties.Get("RemoteSite", @"FALSE").ToUpper() == "TRUE") ? true : false;
            CurveRemoteSiteUser = properties.Get("RemoteSiteUser", @"admin");
            CurveRemoteSitePwd = properties.Get("RemoteSitePwd", @"8888");
            bCurveEquipSaveType = (properties.Get("EquipSaveType", @"FALSE").ToUpper() == "TRUE") ? true : false;
            bCurveStoreInDB = (properties.Get("CurveStoreInDB", @"FALSE").ToUpper() == "TRUE") ? true : false;
            iHistory_CurveStoreTime = Convert.ToInt32(properties.Get("History_CurveStoreTime", @"365"));
            properties = PropertyService.Get(WeihuProperty, new Properties());
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                WeihuPictureRootPathName = Path.Combine(General.GetApplicationRootPath(), "WeihuPicture");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                WeihuPictureRootPathName = properties.Get("WeihuPictureStorePath", @"c:\IoTCenter\WeihuPicture");
            properties = PropertyService.Get(PageJumpProperty, new Properties());
            bAlarmAutoJump = (properties.Get("AlarmAutoJump", "false").ToUpper() == "TRUE") ? true : false;
            double.TryParse(DataCenter.GetPropertyFromPropertyService("DBStoreLimitTime", "", "2"), out DataCenter.ExecSQLTipsSec);
            Console.WriteLine("获取配置信息");
            DataCenter.GetEquipDebugState();
            UpdateMainDataTable();
            UpdateRecordItemsTable();
            SubEquipListDict = GetSubEquipListDataRow(db_Eqp);
            foreach (KeyValuePair<string, List<EquipTableRow>> pair in SubEquipListDict)
            {
                SubEquipList sl = new SubEquipList(pair.Key, pair.Value);
                if (sl.bCanExcute)
                    EquipCategoryDict.Add(pair.Key, sl);
            }
            InitAdminForAlarm();
            AlarmDispatch.init();
            EquipmentLinkage.Run();
            ProcTimeManage.Run();
            Curve.Run();
            MyDBGueueSave2Table.Run();
            Console.WriteLine("初始化其它服务");
            return true;
        }
        static void InitAdminForAlarm()
        {
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    db_adminForAlarm = db.AdministratorTable.FromSqlRaw("SELECT * FROM Administrator").AsNoTracking().ToList();
                }
            }
        }
        static List<EquipTableRow> old_db_eqp;
        static List<YcpTableRow> old_db_ycp;
        static List<YxpTableRow> old_db_yxp;
        static List<SetParmTableRow> old_db_setparm;
        static public void UpdateMainDataTable()
        {
            List<int> iEqpList = new List<int>();
            try
            {
                old_db_eqp = db_eqp;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        db_eqp = db.EquipTable.FromSqlRaw("SELECT * FROM Equip").AsNoTracking().ToList();
                        iEqpList = (from s in db_eqp select s.equip_no).ToList();
                    }
                }
                old_db_ycp = db_ycp;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        db_ycp = db.YcpTable.FromSqlRaw("SELECT * FROM Ycp").AsNoTracking().ToList();
                        db_ycp = db_ycp.Where(x => iEqpList.Contains(x.equip_no)).ToList();
                    }
                }
                old_db_yxp = db_yxp;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        db_yxp = db.YxpTable.FromSqlRaw("SELECT * FROM Yxp").AsNoTracking().ToList();
                        db_yxp = db_yxp.Where(x => iEqpList.Contains(x.equip_no)).ToList();
                    }
                }
                old_db_setparm = db_setparm;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        db_setparm = db.SetParmTable.FromSqlRaw("SELECT * FROM SetParm").AsNoTracking().ToList();
                        db_setparm = db_setparm.Where(x => iEqpList.Contains(x.equip_no)).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        static public void UpdateRecordItemsTable()
        {
            lock (db_recorditems_lock)
            {
                try
                {
                    lock (GWDbProvider.lockstate)
                    {
                        using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                        {
                            db_recorditems = db.GWDataRecordItemsTable.FromSqlRaw("SELECT * FROM GWDataRecordItems").AsNoTracking().ToList();
                        }
                    }
                }
                catch (Exception e)
                {
                    DataCenter.WriteLogFile(e.ToString());
                }
            }
        }
        static DateTime DT_Alarm_LimitDate;
        static public EquipItem GetEquipItemFromEquipNo(int iEquipNo)
        {
            lock (EquipCategoryDict)
            {
                foreach (KeyValuePair<string, object> pair in EquipCategoryDict)
                {
                    SubEquipList EquipList = (SubEquipList)pair.Value;
                    foreach (EquipItem i in EquipList.EquipList)
                    {
                        if (i.iEquipno == iEquipNo)
                            return i;
                    }
                }
            }
            return null;
        }
        static object stationCommState = 0;
        static object stationAlarmState = 0;
        static public object StationCommState
        {
            get
            {
                lock (stationCommState)
                {
                    return stationCommState;
                }
            }
            set
            {
                try
                {
                    lock (stationCommState)
                    {
                        if (stationCommState != value)
                        {
                            if ((int)stationCommState == 0 && (int)value > 0)
                            {
                                StationCommError?.Invoke(null, new EventArgs());
                                stationCommState = value;
                                return;
                            }
                            if ((int)stationCommState > 0 && (int)value == 0)
                            {
                                StationCommOk?.Invoke(null, new EventArgs());
                                stationCommState = value;
                                return;
                            }
                        }
                        stationCommState = value;
                    }
                }
                catch (Exception e)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), 0);
                }
            }
        }
        static public object StationAlarmState
        {
            get
            {
                lock (stationAlarmState)
                {
                    return stationAlarmState;
                }
            }
            set
            {
                try
                {
                    lock (stationAlarmState)
                    {
                        if (stationAlarmState != value)
                        {
                            if ((int)stationAlarmState == 0 && (int)value > 0)
                            {
                                StationHaveAlarm?.Invoke(null, new EventArgs());
                                stationAlarmState = value;
                                return;
                            }
                            if ((int)stationAlarmState > 0 && (int)value == 0)
                            {
                                StationNoAlarm?.Invoke(null, new EventArgs());
                                stationAlarmState = value;
                                return;
                            }
                        }
                        stationAlarmState = value;
                    }
                }
                catch (Exception e)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), 0);
                }
            }
        }
    }
    public enum ChangedEquipState
    {
        Add, Delete, Edit
    }
    public class ChangedEquip
    {
        public int iStaNo;
        public int iEqpNo;
        public ChangedEquipState State;
    }
    public class DelEquip
    {
        public int iStaNo;
        public int iEqpNo;
        public DateTime DelTime;
    }
    public partial class SubEquipList
    {
        bool bStartThread = true;
        bool bStartRefreshThread = false;
        bool bStartSetParmThread = false;
        const int ThreadInterval = 10;
        const int DelayDelEqpSecond = 2;
        public string local_addr;
        ArrayList equiplist = new ArrayList();
        public ArrayList EquipList
        {
            get
            {
                lock (equiplist)
                {
                    return equiplist;
                }
            }
            set
            {
                lock (equiplist)
                {
                    equiplist = value;
                }
            }
        }
        DataTable dt;
        Thread RefreshThread, SetParmThread;
        public EquipItem OldEquip;
        object existsetparm = (object)false;
        object datarefreshbreak = (object)false;
        List<int> SetParmEquipList = new List<int>();
        public bool bCanExcute = true;
        public event SubEquipListChangedEventHandler SubEquipListChanged;
        public delegate void SubEquipListChangedEventHandler(object sender, EventArgs e);
        public event EquipAddEventHandler EquipAdd;
        public delegate void EquipAddEventHandler(object sender, EventArgs e);
        public event EquipDelEventHandler EquipDel;
        public delegate void EquipDelEventHandler(object sender, EventArgs e);
        public event EquipEditEventHandler EquipEdit;
        public delegate void EquipEditEventHandler(object sender, EventArgs e);
        System.Timers.Timer T = new System.Timers.Timer();
        public bool ExistSetParm
        {
            get
            {
                lock (existsetparm)
                {
                    return (bool)existsetparm;
                }
            }
            set
            {
                lock (existsetparm)
                {
                    existsetparm = (object)value;
                }
            }
        }
        public bool DataRefreshBreak
        {
            get
            {
                lock (datarefreshbreak)
                {
                    return (bool)datarefreshbreak;
                }
            }
            set
            {
                lock (datarefreshbreak)
                {
                    datarefreshbreak = (object)value;
                }
            }
        }
        object resetequip = false;
        public bool ResetEquips
        {
            get
            {
                lock (resetequip)
                {
                    return (bool)resetequip;
                }
            }
            set
            {
                lock (resetequip)
                {
                    resetequip = value;
                }
            }
        }
        public SubEquipList(string str_local_addr, List<EquipTableRow> DataRowList)
        {
            SerialPort spt = new SerialPort();
            local_addr = str_local_addr;
            foreach (EquipTableRow r in DataRowList)
            {
                int stano = r.sta_n;
                int eqpno = r.equip_no;
                EquipItem e = new EquipItem(stano, eqpno, spt);
                EquipList.Add(e);
                if (!DataCenter.EquipItemDict.ContainsKey(eqpno))
                {
                    DataCenter.EquipItemDict.Add(eqpno, e);
                }
                else
                {
                    DataCenter.EquipItemDict.Remove(eqpno);
                    DataCenter.EquipItemDict.Add(eqpno, e);
                }
                if (DataCenter.IsDebugState(e.iEquipno))
                    e.IsDebug = true;
            }
        }
        public void Refresh()
        {
            while (bStartThread)
            {
                while (ExistSetParm && DataRefreshBreak && bStartThread)
                {
                    Thread.Sleep(ThreadInterval);
                }
                while (!DataRefreshBreak && bStartThread)
                {
                    if (ResetEquips)
                    {
                        try
                        {
                            foreach (EquipItem e in EquipList)
                            {
                                e.ResetWhenDBChanged(e.iStano, e.iEquipno);
                                if (e.ICommunication != null)
                                    e.ICommunication.ResetFlag = true;
                            }
                        }
                        catch (Exception e1)
                        {
                            DataCenter.WriteLogFile(e1.ToString());
                        }
                        ResetEquips = false;
                    }
                    try
                    {
                        GetEquipsData();
                    }
                    catch (Exception e)
                    {
                        DataCenter.WriteLogFile(e.ToString());
                    }
                    Thread.Sleep(ThreadInterval);
                }
            }
        }
        void GetEquipsData()
        {
            foreach (EquipItem e in EquipList)
            {
                if (e.ICommunication == null)
                    continue;
                e.iAcc_num++;
                if ((e.iAcc_num % e.iAcc_cyc) == 0)
                {
                    e.iAcc_num = 0;
                    e.Reset = false;
                    OldEquip = e;
                    if (e.ICommunication == null)
                        continue;
                    if (!e.ICommunication.init(e))
                    {
                        e.iCommFaultRetryCount += 1;
                        e.ICommunication.ResetFlag = true;
                        if (e.ICommunication.m_retrytime <= e.iCommFaultRetryCount)
                        {
                            lock (e.EquipRWstate)
                            {
                                e.bCommunicationOk = false;
                                e.State = EquipState.NoCommunication;
                                e.iCommFaultRetryCount = 0;
                                e.bInitOk = false;
                            }
                            e.OnValueFrashed();
                        }
                        continue;
                    }
                    else
                    {
                        e.bInitOk = true;
                    }
                    CommunicationState ret = e.ICommunication.GetData((CEquipBase)e.ICommunication);
                    if (ret == CommunicationState.ok)
                    {
                        e.bCommunicationOk = true;
                        e.iCommFaultRetryCount = 0;
                        lock (e.YCItemDict)
                        {
                            foreach (KeyValuePair<int, object> pair in e.ICommunication.YCResults)
                            {
                                if (e.YCItemDict.ContainsKey(pair.Key))
                                {
                                    e.YCItemDict[pair.Key].YCValue = pair.Value;
                                }
                            }
                        }
                        lock (e.YXItemDict)
                        {
                            foreach (KeyValuePair<int, object> pair in e.ICommunication.YXResults)
                            {
                                if (e.YXItemDict.ContainsKey(pair.Key))
                                {
                                    e.YXItemDict[pair.Key].YXValue = pair.Value;
                                }
                            }
                        }
                        lock (e.ICommunication.EquipEventList)
                        {
                            if (e.ICommunication.EquipEventList.Count() > 0)
                            {
                                foreach (EquipEvent evt in e.ICommunication.EquipEventList)
                                {
                                    MessageService.AddMessage(evt.level, evt.msg, e.iEquipno);
                                }
                                e.ICommunication.EquipEventList.Clear();
                            }
                        }
                        e.DataFrash = true;
                    }
                    if (ret == CommunicationState.setreturn)
                    {
                        if (e.DoSetParm && e.State == EquipState.CommunicationOK)
                        {
                            lock (e.EquipRWstate)
                            {
                                e.State = EquipState.HaveSetParm;
                            }
                        }
                        DataRefreshBreak = true;
                        return;
                    }
                    if (ret == CommunicationState.fail)
                    {
                        e.iCommFaultRetryCount += 1;
                        e.ICommunication.ResetFlag = true;
                        if (e.ICommunication.m_retrytime <= e.iCommFaultRetryCount)
                        {
                            lock (e.EquipRWstate)
                            {
                                e.bCommunicationOk = false;
                                e.State = EquipState.NoCommunication;
                                e.iCommFaultRetryCount = 0;
                            }
                        }
                    }
                    e.OnValueFrashed();
                }
                else
                {
                    continue;
                }
            }
        }
        public void StartRefreshThread()
        {
            if (bStartRefreshThread)
                return;
            ThreadStart entryPoint = new ThreadStart(Refresh);
            RefreshThread = new Thread(entryPoint);
            RefreshThread.Start();
            bStartRefreshThread = true;
        }
        public void SuspendRefreshThread()
        {
        }
        public void SetParmScan()
        {
            while (bStartThread)
            {
                try
                {
                    foreach (EquipItem e in EquipList)
                    {
                        lock (e.SetItemQueue)
                        {
                            if (e.SetItemQueue.Count > 0)
                            {
                                ExistSetParm = true;
                                SendSetParmFlag(true);
                                e.DoSetParm = true;
                                if (e.Reserve3.ToUpper().Trim() != "FASTSET")
                                {
                                    while (!DataRefreshBreak)
                                    {
                                        Thread.Sleep(ThreadInterval);
                                    }
                                }
                                try
                                {
                                    SetParm(e);
                                    lock (SetParmEquipList)
                                    {
                                        if (!SetParmEquipList.Contains(e.iEquipno))
                                            SetParmEquipList.Add(e.iEquipno);
                                    }
                                }
                                catch (Exception e1)
                                {
                                    DataCenter.WriteLogFile(e1.ToString());
                                }
                                e.DoSetParm = false;
                            }
                        }
                    }
                    ExistSetParm = false;
                    DataRefreshBreak = false;
                    SendSetParmFlag(false);
                }
                catch (Exception e)
                {
                    DataCenter.WriteLogFile(e.ToString());
                }
                Thread.Sleep(ThreadInterval);
            }
        }
        void SetParm(EquipItem e)
        {
            if (e.ICommunication == null)
                return;
            e.State = EquipState.HaveSetParm;
            e.OnValueFrashed();
            string msg = "";
            e.ICommunication.init(e);
            SetItem setitem;
            lock (e.SetItemQueue)
            {
                setitem = (SetItem)e.SetItemQueue.Dequeue();
                if (setitem.bStopSetParm)
                {
                    setitem.DisposeEvent();
                    return;
                }
            }
            string s1, s2, s3, s4;
            s1 = ResourceService.GetString("AlarmCenter.DataCenter.Msg2");
            s2 = ResourceService.GetString("AlarmCenter.DataCenter.Msg3");
            s3 = ResourceService.GetString("AlarmCenter.DataCenter.Msg4");
            s4 = ResourceService.GetString("AlarmCenter.DataCenter.Msg5");
            string desc = setitem.GetSetItemDesc();
            if (setitem.Type == null)
            {
                MessageService.AddMessage(MessageLevel.Warn, desc + "---没有加入到执行队列，因为type==null", 0, false);
                return;
            }
            e.ICommunication.SetParmExecutor = setitem.Executor;
            string csExecutor;
            csExecutor = setitem.Executor;
            if (string.IsNullOrEmpty(setitem.Executor))
                csExecutor = setitem.sysExecutor;
            if (e.ICommunication.SetParm(setitem.MainInstruct, setitem.MinorInstruct, setitem.Value))
            {
                setitem.WaitSetParmIsFinish = true;
                if (desc == null)
                {
                    msg += string.Format(s1 + "{0}---" + s2 + s4 + "{1}-{2}-{3}", e.Equip_nm, setitem.MainInstruct, setitem.MinorInstruct, setitem.Value) + "---by:" + csExecutor;
                }
                else
                {
                    msg += desc + ">>" + s2 + "---by:" + csExecutor;
                }
            }
            else
            {
                setitem.WaitSetParmIsFinish = false;
                if (desc == null)
                {
                    msg += string.Format(s1 + "{0}---" + s3 + s4 + "{1}-{2}-{3}", e.Equip_nm, setitem.MainInstruct, setitem.MinorInstruct, setitem.Value) + "---by:" + csExecutor;
                }
                else
                {
                    msg += desc + ">>" + s3 + "---by:" + csExecutor;
                }
            }
            StationItem.FireSetParmResultEvent(setitem);
            if (setitem.bRecord)
            {
                MessageService.AddMessage(MessageLevel.SetParm, msg, e.iEquipno, false);
                DateTime t = General.Convert2DT(DateTime.Now);
                StationItem.MyDBGueueSave2Table.AddSetEvtTableRow2List(new SetEvtTableRow
                {
                    equip_no = setitem.EquipNo,
                    set_no = setitem.m_SetNo,
                    GWEvent = msg,
                    GWTime = t,
                    GWOperator = csExecutor,
                    GWSource = setitem.UserIPandPort
                });
            }
        }
        void SendSetParmFlag(bool flag)
        {
            foreach (EquipItem e in EquipList)
            {
                if (e.ICommunication != null)
                    e.ICommunication.RunSetParmFlag = flag;
            }
        }
        public void StartSetParmThread()
        {
            if (bStartSetParmThread)
                return;
            ThreadStart entryPoint = new ThreadStart(SetParmScan);
            SetParmThread = new Thread(entryPoint);
            SetParmThread.Start();
            bStartSetParmThread = true;
        }
        public void SuspendSetParmThread()
        {
        }
    }
}
