﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace OpenGWDataCenter.Model
{
    public class SafeTimeSpan
    {
        public TimeSpan tStart;
        public TimeSpan tEnd;
        public SafeTimeSpan(TimeSpan t, TimeSpan t1)
        {
            tStart = t;
            tEnd = t1;
        }
    }
    public class NoSetItemPermissionEventArgs : EventArgs
    {
        public string strGUID { get; set; }
    }
    public class EquipItem : ICanReset, IComparable
    {
        int istano;
        object iequipno;
        int iacc_cyc, iacc_num, alarm_scheme;
        string equip_nm;
        string local_addr, equip_addr;
        public string communication_drv, communication_param, communication_time_param;
        public string alarmMsg, RestorealarmMsg, advice_Msg, wave_file, restore_wave_file, related_pic, equip_detail;
        public int attrib;
        public int? AlarmRiseCycle;
        public string Reserve1;
        public string Reserve2;
        public string Reserve3;
        public string related_video, ZiChanID, PlanNo;
        public List<SafeTimeSpan> SafeTimeSpanList = new List<SafeTimeSpan>();
        Assembly dll;
        IEquip icommunication;
        object datafrash = false;
        object state;
        public Dictionary<int, YCItem> YCItemDict = new Dictionary<int, YCItem>();
        public Dictionary<int, YXItem> YXItemDict = new Dictionary<int, YXItem>();
        public Queue SetItemQueue = new Queue();
        public Queue OffLineSetItemQueue = new Queue();
        public event EquipValueFrashEventHandler ValueFrashed;
        public delegate void EquipValueFrashEventHandler(object sender, EventArgs e);
        public event EqpStateChangedEventHandler EqpStateChanged;
        public delegate void EqpStateChangedEventHandler(object sender, EventArgs e);
        public event SetItemNoPermissionEventHandler NoSetItemPermissionEvent;
        public delegate void SetItemNoPermissionEventHandler(object sender, NoSetItemPermissionEventArgs e);
        public event EventHandler EquipCommError;
        public event EventHandler EquipCommOk;
        public event EventHandler EquipHaveAlarm;
        public event EventHandler EquipNoAlarm;
        public int iCommFaultRetryCount = 0;
        public bool bInitOk = false;
        public bool bCommunicationOk = false;
        public SerialPort serialport;
        public bool bCanMonitor = false;
        public bool DoSetParm = false;
        public object EquipRWstate = false;
        public object EquipResetLock = false;
        object reset = false;
        object debug = false;
        object canconfirm = false;
        object isbackup = false;
        public object Reset
        {
            get
            {
                lock (reset)
                {
                    return reset;
                }
            }
            set
            {
                lock (reset)
                {
                    reset = value;
                }
            }
        }
        public bool IsDebug
        {
            get
            {
                lock (debug)
                {
                    return (bool)debug;
                }
            }
            set
            {
                lock (debug)
                {
                    debug = value;
                }
            }
        }
        public bool IsBackupEquip = false;
        public bool IsBackupState
        {
            get
            {
                lock (isbackup)
                {
                    return (bool)isbackup;
                }
            }
            set
            {
                lock (isbackup)
                {
                    isbackup = value;
                }
            }
        }
        public bool bCanConfirm2NormalState
        {
            get
            {
                lock (canconfirm)
                {
                    return (bool)canconfirm;
                }
            }
            set
            {
                lock (canconfirm)
                {
                    debug = value;
                }
            }
        }
        string evtGUID;
        public string EventGUID
        {
            get
            {
                return evtGUID;
            }
            set
            {
                evtGUID = value;
            }
        }
        public int CompareTo(object obj)
        {
            EquipItem eqp = obj as EquipItem;
            if (iEquipno > eqp.iEquipno)
            {
                return 1;
            }
            else if (iEquipno == eqp.iEquipno)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public bool IsSafeTime()
        {
            DateTime t = DateTime.Now;
            TimeSpan s = t.TimeOfDay;
            if (SafeTimeSpanList.Count > 0)
            {
                foreach (SafeTimeSpan ts in SafeTimeSpanList)
                {
                    if (s >= ts.tStart && s <= ts.tEnd)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        void GetSafeTimeSpanList(string s)
        {
            try
            {
                SafeTimeSpanList.Clear();
                s = s.Trim();
                if (string.IsNullOrEmpty(s))
                    return;
                string[] ss = s.Split('+');
                foreach (string s1 in ss)
                {
                    string[] tt = s1.Split('-');
                    if (tt.Length == 2)
                    {
                        TimeSpan t1 = new TimeSpan(Convert.ToInt32(tt[0].Split(':')[0]), Convert.ToInt32(tt[0].Split(':')[1]), Convert.ToInt32(tt[0].Split(':')[2]));
                        TimeSpan t2 = new TimeSpan(Convert.ToInt32(tt[1].Split(':')[0]), Convert.ToInt32(tt[1].Split(':')[1]), Convert.ToInt32(tt[1].Split(':')[2]));
                        SafeTimeSpanList.Add(new SafeTimeSpan(t1, t2));
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        public string Related_pic
        {
            get
            {
                lock (related_pic)
                    return related_pic;
            }
        }
        public YCItem GetYCItem(int iYcNo)
        {
            lock (YCItemDict)
            {
                if (YCItemDict.ContainsKey(iYcNo))
                {
                    return YCItemDict[iYcNo];
                }
            }
            return null;
        }
        public YXItem GetYXItem(int iYxNo)
        {
            lock (YXItemDict)
            {
                if (YXItemDict.ContainsKey(iYxNo))
                {
                    return YXItemDict[iYxNo];
                }
            }
            return null;
        }
        public EquipState State
        {
            get
            {
                lock (state)
                    return (EquipState)state;
            }
            set
            {
                if (value == EquipState.Initial)
                    state = value;
                if (value != (EquipState)state)
                {
                    if (value != EquipState.NoCommunication && (EquipState)state == EquipState.NoCommunication)
                    {
                        state = value;
                        if (EquipCommOk != null)
                        {
                            evtGUID = Guid.NewGuid().ToString();
                            EquipCommOk(this, new EventArgs());
                            if ((Alarm_scheme & 0x01) > 0)
                            {
                                AlarmDispatch.Add2MessageService(0, RestorealarmMsg, "", iEquipno, "E", 0, null, null, related_video, ZiChanID, PlanNo, evtGUID);
                            }
                        }
                        StationItem.StationCommState = (int)StationItem.StationCommState - 1;
                    }
                    if (value == EquipState.HaveAlarm)
                    {
                        state = value;
                        EquipHaveAlarm?.Invoke(this, new EventArgs());
                        StationItem.StationAlarmState = (int)StationItem.StationAlarmState + 1;
                        EqpStateChanged?.Invoke(this, new EventArgs());
                        return;
                    }
                    if (value == EquipState.CommunicationOK && (EquipState)state == EquipState.HaveAlarm)
                    {
                        state = value;
                        EquipNoAlarm?.Invoke(this, new EventArgs());
                        StationItem.StationAlarmState = (int)StationItem.StationAlarmState - 1;
                        EqpStateChanged?.Invoke(this, new EventArgs());
                        return;
                    }
                    if (value == EquipState.NoCommunication)
                    {
                        if ((EquipState)state == EquipState.HaveAlarm)
                        {
                            StationItem.StationAlarmState = (int)StationItem.StationAlarmState - 1;
                        }
                        state = value;
                        if (EquipCommError != null)
                        {
                            evtGUID = Guid.NewGuid().ToString();
                            EquipCommError(this, new EventArgs());
                            if ((Alarm_scheme & 0x01) > 0)
                            {
                                AlarmDispatch.Add2MessageService(9, alarmMsg, advice_Msg, iEquipno, "E", 0, wave_file, related_pic, related_video, ZiChanID, PlanNo, evtGUID);
                            }
                        }
                        StationItem.StationCommState = (int)StationItem.StationCommState + 1;
                        SetYcYxNoCommState();
                        EqpStateChanged?.Invoke(this, new EventArgs());
                        return;
                    }
                    if (value == EquipState.BackUp)
                    {
                        state = value;
                        SetYcYxNoCommState();
                        EqpStateChanged?.Invoke(this, new EventArgs());
                    }
                    state = value;
                    EqpStateChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public bool DataFrash
        {
            get
            {
                lock (datafrash)
                {
                    return (bool)datafrash;
                }
            }
            set
            {
                lock (datafrash)
                {
                    datafrash = value;
                }
            }
        }
        public Assembly Dll
        {
            get
            {
                return dll;
            }
        }
        public IEquip ICommunication
        {
            get
            {
                return icommunication;
            }
        }
        public int iStano
        {
            get
            {
                return istano;
            }
        }
        public int iEquipno
        {
            get
            {
                lock (iequipno)
                {
                    return (int)iequipno;
                }
            }
        }
        public int Alarm_scheme
        {
            get
            {
                return alarm_scheme;
            }
        }
        public int iAcc_cyc
        {
            get
            {
                return iacc_cyc;
            }
        }
        public int iAcc_num
        {
            get
            {
                return iacc_num;
            }
            set
            {
                iacc_num = value;
            }
        }
        public string Local_addr
        {
            get
            {
                return local_addr;
            }
        }
        public string Equip_addr
        {
            get
            {
                return equip_addr;
            }
        }
        public string Equip_nm
        {
            get
            {
                lock (equip_nm)
                {
                    return equip_nm;
                }
            }
            set
            {
                lock (equip_nm)
                {
                    equip_nm = value;
                }
            }
        }
        EquipTableRow dr;
        object bufang = true;
        object oo = 1;
        public bool? Bufang
        {
            get
            {
                lock (oo)
                {
                    return (bool?)bufang;
                }
            }
            set
            {
                lock (oo)
                {
                    if ((bool?)bufang != value)
                    {
                        bufang = value;
                        EqpStateChanged?.Invoke(this, new EventArgs());
                    }
                }
            }
        }
        public EquipItem(int sta, int eqp, SerialPort p)
        {
            istano = sta;
            iequipno = eqp;
            State = EquipState.Initial;
            serialport = p;
            ResetWhenDBChanged(sta, eqp);
            iacc_num = 0;
        }
        public bool ResetWhenDBChanged(params object[] o)
        {
            lock (EquipResetLock)
            {
                int sta = (int)o[0];
                int eqp = (int)o[1];
                try
                {
                    lock (GWDbProvider.lockstate)
                    {
                        using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                        {
                            try
                            {
                                dr = StationItem.db_Eqp.Single(m => m.equip_no == eqp);
                            }
                            catch (Exception e)
                            {
                                return false;
                            }
                        }
                    }
                    equip_nm = dr.equip_nm;
                    equip_detail = dr.equip_detail;
                    iacc_cyc = dr.acc_cyc;
                    alarm_scheme = dr.alarm_scheme;
                    local_addr = dr.local_addr;
                    equip_addr = dr.equip_addr;
                    related_pic = dr.related_pic;
                    communication_drv = dr.communication_drv.Trim();
                    alarmMsg = equip_nm + ":" + dr.out_of_contact;
                    advice_Msg = dr.proc_advice;
                    attrib = dr.attrib;
                    RestorealarmMsg = equip_nm + ":" + dr.contacted;
                    AlarmRiseCycle = dr.AlarmRiseCycle == null ? 0 : dr.AlarmRiseCycle;
                    Reserve1 = dr.Reserve1 ?? "";
                    Reserve2 = dr.Reserve2 ?? "";
                    Reserve3 = dr.Reserve3 ?? "";
                    related_video = dr.related_video ?? "";
                    ZiChanID = dr.ZiChanID ?? "";
                    PlanNo = dr.PlanNo ?? "";
                    if (!string.IsNullOrEmpty(dr.backup))
                        IsBackupEquip = dr.backup.ToUpper() == "TRUE" ? true : false;
                    GetSafeTimeSpanList(dr.SafeTime ?? "");
                    string wf = dr.event_wav ?? "";
                    string[] fs = wf.Split('/');
                    wave_file = fs[0];
                    if (fs.Length == 2)
                        restore_wave_file = fs[1];
                    communication_param = dr.communication_param ?? "";
                    communication_time_param = dr.communication_time_param ?? "";
                    InitYCItemDict(sta, eqp);
                    InitYXItemDict(sta, eqp);
                    GetInterfaceOfEquip();
                    OnValueFrashed();
                }
                catch (Exception e)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, e.ToString(), (int)iequipno);
                }
                return true;
            }
        }
        ~EquipItem()
        {
            int k = 0;
        }
        public void OnValueFrashed()
        {
            SetEquipState();
            SetBufangState();
            ValueFrashed?.Invoke(this, new EventArgs());
        }
        void SetEquipState()
        {
            if (!bCommunicationOk)
            {
                if (State == EquipState.NoCommunication)
                {
                    return;
                }
                if (State == EquipState.HaveSetParm)
                {
                    return;
                }
            }
            foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
            {
                if (pair.Value.IsAlarm)
                {
                    State = EquipState.HaveAlarm;
                    return;
                }
            }
            foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
            {
                if (pair.Value.IsAlarm)
                {
                    State = EquipState.HaveAlarm;
                    return;
                }
            }
            State = EquipState.CommunicationOK;
            return;
        }
        void SetBufangState()
        {
            bool BF_C = false;
            bool CF_C = false;
            bool BF_X = false;
            bool CF_X = false;
            bool? bf_c = false;
            bool? bf_x = false;
            bool HaveC = false;
            bool HaveX = false;
            foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
            {
                HaveC = true;
                if (pair.Value.Bufang)
                {
                    BF_C = true;
                }
                else
                {
                    CF_C = true;
                }
            }
            if (HaveC)
            {
                if (BF_C && CF_C)
                    bf_c = null;
                if (!BF_C && CF_C)
                    bf_c = false;
                if (BF_C && !CF_C)
                    bf_c = true;
            }
            foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
            {
                HaveX = true;
                if (pair.Value.Bufang)
                {
                    BF_X = true;
                }
                else
                {
                    CF_X = true;
                }
            }
            if (HaveX)
            {
                if (BF_X && CF_X)
                    bf_x = null;
                if (!BF_X && CF_X)
                    bf_x = false;
                if (BF_X && !CF_X)
                    bf_x = true;
            }
            if (HaveC && HaveX)
            {
                if (bf_c == true && bf_x == true)
                    Bufang = true;
                if (bf_c == null || bf_x == null)
                    Bufang = null;
                if (bf_c == false && bf_x == false)
                    Bufang = false;
            }
            if (HaveC && !HaveX)
            {
                Bufang = bf_c;
            }
            if (!HaveC && HaveX)
            {
                Bufang = bf_x;
            }
        }
        void InitYCItemDict(int sta, int eqp)
        {
            List<YcpTableRow> rs = StationItem.db_Ycp.Where(m => m.equip_no == eqp).ToList();
            if (!rs.Any())
                return;
            List<int> yc_no_list = new List<int>();
            lock (YCItemDict)
            {
                foreach (YcpTableRow r in rs)
                {
                    yc_no_list.Add(r.yc_no);
                    if (YCItemDict.ContainsKey(r.yc_no))
                        YCItemDict[r.yc_no].Init(r);
                    else
                        YCItemDict.Add(r.yc_no, new YCItem(r, this));
                }
                try
                {
                    List<int> YCItemDict_list = (from d in YCItemDict select d.Key).ToList();
                    if (YCItemDict_list.Except(yc_no_list).Count() > 0)
                    {
                        foreach (int k in YCItemDict_list.Except(yc_no_list))
                        {
                            if (YCItemDict.ContainsKey(k))
                            {
                                YCItemDict[k].ClearAllEvents();
                                YCItemDict.Remove(k);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
                foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
                {
                    pair.Value.YCValue = pair.Value.OldValue;
                    pair.Value.bForceUpdate = true;
                }
            }
        }
        void InitYXItemDict(int sta, int eqp)
        {
            List<YxpTableRow> rs = StationItem.db_Yxp.Where(m => m.equip_no == eqp).ToList();
            if (!rs.Any())
                return;
            List<int> yx_no_list = new List<int>();
            lock (YXItemDict)
            {
                foreach (YxpTableRow r in rs)
                {
                    yx_no_list.Add(r.yx_no);
                    if (YXItemDict.ContainsKey(r.yx_no))
                        YXItemDict[r.yx_no].Init(r);
                    else
                        YXItemDict.Add(r.yx_no, new YXItem(r, this));
                }
                try
                {
                    List<int> YXItemDict_list = (from d in YXItemDict select d.Key).ToList();
                    if (YXItemDict_list.Except(yx_no_list).Count() > 0)
                    {
                        foreach (int k in YXItemDict_list.Except(yx_no_list))
                        {
                            if (YXItemDict.ContainsKey(k))
                            {
                                YXItemDict[k].ClearAllEvents();
                                YXItemDict.Remove(k);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
                foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
                {
                    pair.Value.YXValue = pair.Value.OldValue;
                    pair.Value.bForceUpdate = true;
                }
            }
        }
        public int GetYCItemAlarmCount()
        {
            int YCItemAlarmCount = 0;
            lock (YCItemDict)
            {
                foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
                {
                    if (pair.Value.IsAlarm)
                        YCItemAlarmCount += 1;
                }
            }
            return YCItemAlarmCount;
        }
        public int GetYXItemAlarmCount()
        {
            int YXItemAlarmCount = 0;
            lock (YXItemDict)
            {
                foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
                {
                    if (pair.Value.IsAlarm)
                        YXItemAlarmCount += 1;
                }
            }
            return YXItemAlarmCount;
        }
        public void SetBufang()
        {
            if (Bufang == false || Bufang == null)
            {
                Bufang = true;
                lock (YCItemDict)
                {
                    foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
                    {
                        pair.Value.Bufang = true;
                    }
                }
                lock (YXItemDict)
                {
                    foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
                    {
                        pair.Value.Bufang = true;
                    }
                }
            }
        }
        public void SetChefang()
        {
            if (Bufang == true || Bufang == null)
            {
                Bufang = false;
                lock (YCItemDict)
                {
                    foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
                    {
                        pair.Value.Bufang = false;
                    }
                }
                lock (YXItemDict)
                {
                    foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
                    {
                        pair.Value.Bufang = false;
                    }
                }
            }
        }
        void SetYcYxNoCommState()
        {
            foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
            {
                pair.Value.IsAlarm = false;
                pair.Value.AlarmState = false;
                pair.Value.alreadyAlarm = false;
                pair.Value.Comments = "";
                pair.Value.YCValue = "***";
            }
            foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
            {
                pair.Value.IsAlarm = false;
                pair.Value.Statefor0_1 = false;
                pair.Value.Statefor1_0 = false;
                pair.Value.RestoreAlarmState = false;
                pair.Value.Comments = "";
                pair.Value.YXValue = "unknow";
            }
        }
        public void AddSetItem(SetItem item)
        {
            if (item == null)
                return;
            if (!string.IsNullOrEmpty(item.Executor))
            {
                lock (DataCenter.GetLoginUserState)
                {
                    DataCenter.GetLoginUser(item.Executor, false);
                    if (DataCenter.LoginUser == null)
                        return;
                    if (!DataCenter.LoginUser.CanControl2SetItem(item.EquipNo, item.GetSetNo()))
                    {
                        string msg = string.Format("{0}没有权限执行命令:{1}", item.Executor, item.GetSetItemDesc());
                        MessageService.AddMessage(MessageLevel.Info, msg, 0, false);
                        if (NoSetItemPermissionEvent != null && item.bShowDlg)
                        {
                            NoSetItemPermissionEvent(msg, new NoSetItemPermissionEventArgs() { strGUID = item.Client_Instance_GUID });
                        }
                        return;
                    }
                }
            }
            lock (SetItemQueue)
            {
                if (!item.bCanRepeat)
                {
                    foreach (SetItem i in SetItemQueue)
                    {
                        if (i.EquipNo == item.EquipNo && i.MainInstruct == item.MainInstruct && i.MinorInstruct == item.MinorInstruct && i.Value == item.Value)
                            return;
                    }
                }
                if (State != EquipState.NoCommunication && State != EquipState.Initial)
                {
                    SetItemQueue.Enqueue(item);
                }
            }
        }
        public string GetEquipInfo()
        {
            return communication_param.Trim().ToUpper() + communication_time_param.Trim().ToUpper() + communication_drv.Trim().ToUpper() + YCItemDict.Count().ToString() + YXItemDict.Count().ToString();
        }
        void GetInterfaceOfEquip()
        {
            icommunication = null;
            try
            {
                string FullPathName;
                FullPathName = Path.Combine(General.GetApplicationRootPath(), "dll");
                FullPathName = Path.Combine(FullPathName, communication_drv);
                dll = Assembly.LoadFrom(FullPathName);
                if (dll == null)
                {
                    throw new ArgumentException(FullPathName + ":load fault");
                }
                Type[] types = dll.GetTypes();
                foreach (Type t in types)
                {
                    if (t.Name == "CEquip")
                    {
                        icommunication = dll.CreateInstance(t.FullName) as IEquip;
                        icommunication.equipitem = this;
                        break;
                    }
                }
                if (icommunication == null)
                    throw new ArgumentException("icommunication as  IEquip is null");
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Fatal, e.ToString(), iEquipno, false);
                bCommunicationOk = false;
                State = EquipState.NoCommunication;
                OnValueFrashed();
            }
        }
        public void ClearAllEvents()
        {
            DataCenter.ClearAllEvents(this);
        }
        public void DisposeMe()
        {
            lock (YCItemDict)
            {
                foreach (KeyValuePair<int, YCItem> pair in YCItemDict)
                {
                    pair.Value.ClearAllEvents();
                }
            }
            lock (YXItemDict)
            {
                foreach (KeyValuePair<int, YXItem> pair in YXItemDict)
                {
                    pair.Value.ClearAllEvents();
                }
            }
            lock (EquipRWstate)
            {
                ClearAllEvents();
            }
        }
    }
}
