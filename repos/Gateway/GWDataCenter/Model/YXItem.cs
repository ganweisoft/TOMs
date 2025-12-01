﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using System;
using System.Collections.Generic;
using System.IO;
namespace OpenGWDataCenter.Model
{
    public enum SafeTimeType
    {
        NoSafeTime, OnlyShowNormalState, ShowRealTimeState
    }
    public class YXItem
    {
        public EquipItem equipitem;
        public YxpAlarmStateTrack AlarmStateTrack;
        int sta_n;
        object equip_no, yx_no;
        object yxvalue, oldvalue, oldyxvalue = "@@@", oldCurveValue;
        string EquipNm, yx_nm, evt_01, evt_10, wave_file, restore_wave_file, related_pic;
        int level_r, level_d, alarm_acceptable_time, restore_acceptable_time, alarm_repeat_time, alarm_scheme, initval, val_trait;
        int alarmLevel, restorelevel;
        DateTime safe_bgn, safe_end;
        List<SafeTimeSpan> SafeTimeSpanList = new List<SafeTimeSpan>();
        bool inversion;
        public static object m_DefaultValue = "unknow";
        TimeSpan SpanFor0_1, SpanFor1_0, RestoreSpan, RepeatAlarmSpan;
        DateTime StartAlarmTimeFor0_1, StartAlarmTimeFor1_0, StopAlarmTime, RepeatAlarmTime;
        public bool bRepeatAlarm = false;
        public bool Statefor0_1, Statefor1_0, RestoreAlarmState;
        bool Level1for0_1 = false;
        bool Level1for1_0 = false;
        object isAlarm = false;
        public string alarmMsg, restoreMsg, alarmflag;
        string proc_advice_r, proc_advice_d, proc_advice;
        string comments = " ";
        public bool bCanMonitor = false;
        public string s_alarm_shield = "";
        object bufang = true;
        object oldbufang = true;
        public int? AlarmRiseCycle;
        public string Reserve1;
        public string Reserve2;
        public string Reserve3;
        public string related_video, ZiChanID, PlanNo;
        bool bReset = false;
        public bool IsWuBao = false;
        string[] fs;
        public bool Bufang
        {
            get
            {
                lock (bufang)
                {
                    return (bool)bufang;
                }
            }
            set
            {
                lock (bufang)
                {
                    bufang = value;
                    if (!oldbufang.Equals(bufang))
                    {
                        ValueChanged?.Invoke(this, new EventArgs());
                    }
                    oldbufang = bufang;
                }
            }
        }
        object manualreset = false;
        public bool ManualReset
        {
            get
            {
                lock (manualreset)
                {
                    return Convert.ToBoolean(manualreset);
                }
            }
            set
            {
                lock (manualreset)
                {
                    manualreset = value;
                }
            }
        }
        object originalalarmstate = false;
        public bool OriginalAlarmState
        {
            get
            {
                lock (originalalarmstate)
                {
                    return Convert.ToBoolean(originalalarmstate);
                }
            }
            set
            {
                lock (originalalarmstate)
                {
                    originalalarmstate = value;
                }
            }
        }
        public string Comments
        {
            get
            {
                lock (comments)
                {
                    return comments;
                }
            }
            set
            {
                lock (comments)
                {
                    comments = value;
                }
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
        string yxState = "unknow";
        public string YXState
        {
            get
            {
                lock (yxState)
                {
                    return yxState;
                }
            }
            set
            {
                lock (yxState)
                {
                    yxState = value;
                }
            }
        }
        object curve_rcd = false;
        public bool Curve_rcd
        {
            get
            {
                lock (curve_rcd)
                {
                    return (bool)curve_rcd;
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
        public event YXValueChangedEventHandler ValueChanged;
        public delegate void YXValueChangedEventHandler(object sender, EventArgs e);
        public event YXAlarmEventHandler Alarmed;
        public delegate void YXAlarmEventHandler(object sender, EventArgs e);
        public event YXAlarmRestoreEventHandler AlarmRestored;
        public delegate void YXAlarmRestoreEventHandler(object sender, EventArgs e);
        bool bValueChanged = false;
        int RecCurveTotalMs, OldRecCurveTotalMs;
        DateTime CurveRecordTime;
        TimeSpan CurveRecordTimeSpan;
        const int SetRecordSpan = 300;
        public System.Collections.ArrayList valuelist = new System.Collections.ArrayList();
        void YXToPhysic(ref object val)
        {
            if (val.GetType() == typeof(string))
                return;
            if (inversion)
            {
                try
                {
                    val = !Convert.ToBoolean(val);
                }
                catch (Exception e)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), 0, false);
                }
            }
        }
        public object YXValue
        {
            get
            {
                lock (yxvalue)
                {
                    return yxvalue;
                }
            }
            set
            {
                bValueChanged = false;
                lock (yxvalue)
                {
                    yxvalue = value;
                    oldvalue = value;
                    YXToPhysic(ref yxvalue);
                }
                if (Bufang)
                {
                    SetEvent();
                }
                else
                {
                    SetNoAlarm();
                }
                if (Curve_rcd)
                {
                    RecCurveTotalMs = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond;
                    try
                    {
                        if (RecCurveTotalMs != OldRecCurveTotalMs)
                        {
                            if (yxvalue.GetType() != typeof(string))
                            {
                                if (oldyxvalue == m_DefaultValue)
                                {
                                    RecordCurve();
                                }
                                else
                                {
                                    if (Convert.ToBoolean(yxvalue) != Convert.ToBoolean(oldyxvalue))
                                    {
                                        RecordCurve();
                                    }
                                    CurveRecordTimeSpan = DateTime.Now - CurveRecordTime;
                                    if (CurveRecordTimeSpan.TotalSeconds >= SetRecordSpan)
                                    {
                                        RecordCurve();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    OldRecCurveTotalMs = RecCurveTotalMs;
                }
                oldyxvalue = yxvalue;
            }
        }
        public object OldYXValue
        {
            get
            {
                return oldyxvalue;
            }
        }
        public object OldValue
        {
            get
            {
                return oldvalue;
            }
        }
        public void SetNoAlarm()
        {
            if (IsAlarm)
            {
                yxvalue = !Convert.ToBoolean(yxvalue);
                RestoreAlarm();
                OnValueChanged();
            }
        }
        public int Sta_n
        {
            get
            {
                return sta_n;
            }
        }
        public int Equip_no
        {
            get
            {
                lock (equip_no)
                {
                    return (int)equip_no;
                }
            }
        }
        public int Yx_no
        {
            get
            {
                lock (yx_no)
                {
                    return (int)yx_no;
                }
            }
        }
        public string Yx_nm
        {
            get
            {
                lock (yx_nm)
                {
                    return yx_nm;
                }
            }
            set
            {
                lock (yx_nm)
                {
                    yx_nm = value;
                }
            }
        }
        public int AlarmLevel
        {
            get
            {
                return alarmLevel;
            }
        }
        public int Restorelevel
        {
            get
            {
                return restorelevel;
            }
        }
        public int Level_r
        {
            get
            {
                return level_r;
            }
        }
        public int Level_d
        {
            get
            {
                return level_d;
            }
        }
        public int Alarm_scheme
        {
            get
            {
                return alarm_scheme;
            }
        }
        public bool IsAlarm
        {
            get
            {
                lock (isAlarm)
                {
                    return (bool)isAlarm;
                }
            }
            set
            {
                lock (isAlarm)
                {
                    isAlarm = value;
                }
            }
        }
        public string AlarmMsg
        {
            get
            {
                return alarmMsg;
            }
        }
        public string RestoreMsg
        {
            get
            {
                return restoreMsg;
            }
        }
        public string Wave_file
        {
            get
            {
                return wave_file;
            }
        }
        public string Restore_Wave_file
        {
            get
            {
                return restore_wave_file;
            }
        }
        public string Proc_advice
        {
            get
            {
                return proc_advice;
            }
            set
            {
                proc_advice = value;
            }
        }
        public string Evt_01 { get => evt_01; set => evt_01 = value; }
        public string Evt_10 { get => evt_10; set => evt_10 = value; }
        void SetEvent()
        {
            lock (equipitem.EquipResetLock)
            {
                OnAlarmed();
                if (Convert.ToBoolean(manualreset) == false)
                {
                    OnAlarmRestored();
                    manualreset = val_trait & 0x2;
                }
                OnValueChanged();
            }
        }
        public bool bForceUpdate = false;
        void OnValueChanged()
        {
            if (!oldyxvalue.Equals(yxvalue) && !bValueChanged || bForceUpdate)
            {
                if (bForceUpdate)
                    bForceUpdate = false;
                ValueChanged?.Invoke(this, new EventArgs());
            }
        }
        void OnAlarmed()
        {
            if (IsWuBao)
                return;
            if (Convert.ToBoolean(isAlarm))
            {
                RepeatAlarmSpan = DateTime.Now - RepeatAlarmTime;
                if (alarm_repeat_time == 0)
                    return;
                if (RepeatAlarmSpan.TotalMinutes >= alarm_repeat_time)
                {
                    if (Alarmed != null)
                    {
                        bRepeatAlarm = true;
                        Alarmed(this, new EventArgs());
                        ValueChanged?.Invoke(this, new EventArgs());
                    }
                    RepeatAlarmTime = DateTime.Now;
                }
                bRepeatAlarm = false;
                return;
            }
            if (yxvalue.GetType() != typeof(string))
            {
                bool b = Convert.ToBoolean(yxvalue);
                if (b)
                {
                    if (!Statefor0_1)
                    {
                        StartAlarmTimeFor0_1 = DateTime.Now;
                        Statefor0_1 = true;
                        yxState = evt_01;
                    }
                }
                else
                {
                    Statefor0_1 = false;
                }
                if (!b)
                {
                    if (!Statefor1_0)
                    {
                        StartAlarmTimeFor1_0 = DateTime.Now;
                        Statefor1_0 = true;
                        yxState = evt_10;
                    }
                }
                else
                {
                    Statefor1_0 = false;
                }
                if (level_r > 0 && level_r >= level_d)
                {
                    if (Statefor0_1)
                    {
                        RestoreAlarmState = false;
                        SpanFor0_1 = DateTime.Now - StartAlarmTimeFor0_1;
                        if (SpanFor0_1.TotalSeconds >= alarm_acceptable_time)
                        {
                            alarmMsg = EquipNm + "-" + yx_nm + ":" + evt_01;
                            if (level_r == 1 && level_d == 1)
                            {
                                if (Level1for0_1)
                                    return;
                                Level1for0_1 = true;
                                Level1for1_0 = false;
                            }
                            else
                            {
                                if (GetSafeTimeState() == SafeTimeType.NoSafeTime)
                                    if (!Convert.ToBoolean(isAlarm))
                                    {
                                        isAlarm = true;
                                    }
                            }
                            if (fs.Length == 2)
                            {
                                wave_file = fs[0];
                                restore_wave_file = fs[1];
                            }
                            alarmflag = "0-1";
                            alarmLevel = level_r;
                            proc_advice = proc_advice_r;
                            if (GetSafeTimeState() == SafeTimeType.ShowRealTimeState || GetSafeTimeState() == SafeTimeType.NoSafeTime)
                                yxState = evt_01;
                            else
                                yxState = evt_10;
                            if (Alarmed != null)
                            {
                                if (GetSafeTimeState() == SafeTimeType.NoSafeTime)
                                {
                                    evtGUID = Guid.NewGuid().ToString();
                                    OriginalAlarmState = true;
                                    Alarmed(this, new EventArgs());
                                    AlarmStateTrack.SetState4EquipmentLinkage();
                                    ValueChanged?.Invoke(this, new EventArgs());
                                    bValueChanged = true;
                                    if ((Alarm_scheme & 0x01) > 0 && !bRepeatAlarm)
                                    {
                                        if (AlarmStateTrack.IsDifferentState(1))
                                            AlarmDispatch.Add2MessageService(AlarmLevel, AlarmMsg, Proc_advice, Equip_no, "X", Yx_no, Wave_file, Related_pic, related_video, ZiChanID, PlanNo, evtGUID);
                                    }
                                }
                            }
                            RepeatAlarmTime = DateTime.Now;
                            comments = proc_advice;
                        }
                        else
                        {
                            bValueChanged = true;
                        }
                    }
                }
                if (level_d > 0 && level_d >= level_r)
                {
                    if (Statefor1_0)
                    {
                        RestoreAlarmState = false;
                        SpanFor1_0 = DateTime.Now - StartAlarmTimeFor1_0;
                        if (SpanFor1_0.TotalSeconds >= alarm_acceptable_time)
                        {
                            alarmMsg = EquipNm + "-" + yx_nm + ":" + evt_10;
                            if (level_r == 1 && level_d == 1)
                            {
                                if (Level1for1_0)
                                    return;
                                Level1for0_1 = false;
                                Level1for1_0 = true;
                            }
                            else
                            {
                                if (GetSafeTimeState() == SafeTimeType.NoSafeTime)
                                {
                                    if (!Convert.ToBoolean(isAlarm))
                                    {
                                        isAlarm = true;
                                    }
                                }
                            }
                            if (fs.Length == 2)
                            {
                                wave_file = fs[1];
                                restore_wave_file = fs[0];
                            }
                            alarmflag = "1-0";
                            alarmLevel = level_d;
                            proc_advice = proc_advice_r;
                            if (GetSafeTimeState() == SafeTimeType.ShowRealTimeState || GetSafeTimeState() == SafeTimeType.NoSafeTime)
                                yxState = evt_10;
                            else
                                yxState = evt_01;
                            if (Alarmed != null)
                            {
                                if (GetSafeTimeState() == SafeTimeType.NoSafeTime)
                                {
                                    OriginalAlarmState = true;
                                    evtGUID = Guid.NewGuid().ToString();
                                    Alarmed(this, new EventArgs());
                                    AlarmStateTrack.SetState4EquipmentLinkage();
                                    ValueChanged?.Invoke(this, new EventArgs());
                                    bValueChanged = true;
                                    if ((Alarm_scheme & 0x01) > 0 && !bRepeatAlarm)
                                    {
                                        if (AlarmStateTrack.IsDifferentState(1))
                                            AlarmDispatch.Add2MessageService(AlarmLevel, AlarmMsg, Proc_advice, Equip_no, "X", Yx_no, Wave_file, Related_pic, related_video, ZiChanID, PlanNo, evtGUID);
                                    }
                                }
                            }
                            RepeatAlarmTime = DateTime.Now;
                            comments = proc_advice;
                        }
                        else
                        {
                            bValueChanged = true;
                        }
                    }
                }
            }
            else
            {
                yxState = yxvalue.ToString();
            }
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
        SafeTimeType GetSafeTimeState()
        {
            DateTime t = DateTime.Now;
            TimeSpan s = t.TimeOfDay;
            if (SafeTimeSpanList.Count > 0)
            {
                foreach (SafeTimeSpan ts in SafeTimeSpanList)
                {
                    if (s >= ts.tStart && s <= ts.tEnd)
                    {
                        if (initval == 0)
                            return SafeTimeType.OnlyShowNormalState;
                        if (initval == 1)
                            return SafeTimeType.ShowRealTimeState;
                    }
                }
            }
            else
            {
                foreach (SafeTimeSpan ts in equipitem.SafeTimeSpanList)
                {
                    if (s >= ts.tStart && s <= ts.tEnd)
                    {
                        if (initval == 0)
                            return SafeTimeType.OnlyShowNormalState;
                        if (initval == 1)
                            return SafeTimeType.ShowRealTimeState;
                    }
                }
            }
            return SafeTimeType.NoSafeTime;
        }
        void GetAlarmFlag()
        {
            if (level_r == level_d)
                return;
            if (level_r > 0 && level_r >= level_d)
                alarmflag = "0-1";
            else if (level_d > 0 && level_d >= level_r)
                alarmflag = "1-0";
        }
        public bool isAllZeroLevel()
        {
            if (level_r == 0 && level_d == 0)
                return true;
            return false;
        }
        void OnAlarmRestored()
        {
            if (Convert.ToBoolean(isAlarm) || bReset || IsWuBao || RestoreAlarmState)
            {
                bReset = false;
                GetAlarmFlag();
                if (yxvalue.GetType() != typeof(string))
                {
                    bool b = Convert.ToBoolean(yxvalue);
                    if (GetSafeTimeState() != SafeTimeType.NoSafeTime)
                    {
                        if (Convert.ToBoolean(isAlarm))
                            b = !b;
                    }
                    if (alarmflag == "0-1")
                    {
                        restorelevel = level_d;
                        if (b)
                        {
                            RestoreAlarmState = false;
                        }
                        if (!b && !RestoreAlarmState)
                        {
                            StopAlarmTime = DateTime.Now;
                            RestoreAlarmState = true;
                            restoreMsg = evt_10;
                            yxState = evt_10;
                        }
                    }
                    if (alarmflag == "1-0")
                    {
                        restorelevel = level_r;
                        if (!b)
                        {
                            RestoreAlarmState = false;
                        }
                        if (b && !RestoreAlarmState)
                        {
                            StopAlarmTime = DateTime.Now;
                            RestoreAlarmState = true;
                            restoreMsg = evt_01;
                            yxState = evt_01;
                        }
                    }
                    if (level_d == 0 && level_r == 0)
                    {
                        alarmflag = "0-0";
                        bool statev = Convert.ToBoolean(yxvalue);
                        yxState = statev == true ? evt_01 : evt_10;
                        RestoreAlarmState = false;
                        if (Convert.ToBoolean(isAlarm))
                        {
                            isAlarm = false;
                        }
                        restoreMsg = EquipNm + "-" + yx_nm + ":" + yxState;
                        comments = "";
                        if (AlarmRestored != null)
                        {
                            evtGUID = Guid.NewGuid().ToString();
                            OriginalAlarmState = false;
                            AlarmRestored(this, new EventArgs());
                            AlarmStateTrack.SetState4EquipmentLinkage();
                            ValueChanged?.Invoke(this, new EventArgs());
                            bValueChanged = true;
                            if ((Alarm_scheme & 0x01) > 0)
                            {
                                AlarmDispatch.Add2MessageService(Restorelevel, RestoreMsg, "", Equip_no, "X", Yx_no, null, null, related_video, ZiChanID, PlanNo, evtGUID);
                            }
                        }
                        return;
                    }
                    if (RestoreAlarmState)
                    {
                        RestoreSpan = DateTime.Now - StopAlarmTime;
                        if (RestoreSpan.TotalSeconds >= restore_acceptable_time)
                        {
                            if (IsWuBao)
                            {
                                IsWuBao = false;
                            }
                            RestoreAlarm();
                        }
                        else
                        {
                            bValueChanged = true;
                        }
                    }
                }
            }
        }
        void RestoreAlarm()
        {
            if (!OriginalAlarmState)
                return;
            yxState = restoreMsg;
            RestoreAlarmState = false;
            if (Convert.ToBoolean(isAlarm))
            {
                isAlarm = false;
            }
            restoreMsg = EquipNm + "-" + yx_nm + ":" + restoreMsg;
            comments = "";
            if (AlarmRestored != null)
            {
                evtGUID = Guid.NewGuid().ToString();
                OriginalAlarmState = false;
                AlarmRestored(this, new EventArgs());
                AlarmStateTrack.SetState4EquipmentLinkage();
                ValueChanged?.Invoke(this, new EventArgs());
                bValueChanged = true;
                if ((Alarm_scheme & 0x01) > 0)
                {
                    if (AlarmStateTrack.IsDifferentState(1))
                        AlarmDispatch.Add2MessageService(Restorelevel, RestoreMsg, "", Equip_no, "X", Yx_no, null, null, related_video, ZiChanID, PlanNo, evtGUID);
                }
            }
        }
        public YXItem(YxpTableRow YXRow, EquipItem ei)
        {
            equipitem = ei;
            AlarmStateTrack = new YxpAlarmStateTrack(YXRow, this);
            try
            {
                Init(YXRow);
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), (int)equip_no);
            }
            yxvalue = oldvalue = m_DefaultValue;
            Statefor0_1 = false;
            Statefor1_0 = false;
            RestoreAlarmState = false;
        }
        public void Init(YxpTableRow YXRow)
        {
            isAlarm = false;
            Statefor0_1 = Statefor1_0 = false;
            Level1for0_1 = Level1for1_0 = false;
            RestoreAlarmState = false;
            sta_n = YXRow.sta_n;
            equip_no = YXRow.equip_no;
            yx_no = YXRow.yx_no;
            yx_nm = YXRow.yx_nm;
            EquipNm = equipitem.Equip_nm;
            proc_advice_r = YXRow.proc_advice_r ?? "";
            proc_advice_d = YXRow.proc_advice_d ?? "";
            evt_01 = YXRow.evt_01 ?? "";
            evt_10 = YXRow.evt_10 ?? "";
            string wf = YXRow.wave_file ?? "";
            fs = wf.Split('/');
            wave_file = fs[0];
            if (fs.Length == 2)
                restore_wave_file = fs[1];
            related_pic = YXRow.related_pic ?? "";
            level_r = YXRow.level_r;
            level_d = YXRow.level_d;
            alarm_acceptable_time = (int)(YXRow.alarm_acceptable_time == null ? 0 : YXRow.AlarmRiseCycle);
            restore_acceptable_time = (int)(YXRow.restore_acceptable_time == null ? 0 : YXRow.restore_acceptable_time);
            alarm_repeat_time = YXRow.alarm_repeat_time;
            alarm_scheme = YXRow.alarm_scheme;
            initval = YXRow.initval;
            val_trait = YXRow.val_trait;
            manualreset = val_trait & 0x2;
            inversion = YXRow.inversion;
            GetSafeTimeSpanList(YXRow.SafeTime ?? "");
            s_alarm_shield = YXRow.alarm_shield ?? "";
            curve_rcd = YXRow.curve_rcd;
            AlarmRiseCycle = YXRow.AlarmRiseCycle == null ? 0 : YXRow.AlarmRiseCycle;
            Reserve1 = YXRow.Reserve1 ?? "";
            Reserve2 = YXRow.Reserve2 ?? "";
            Reserve3 = YXRow.Reserve3 ?? "";
            related_video = YXRow.related_video;
            if (string.IsNullOrEmpty(related_video))
                related_video = equipitem.related_video;
            ZiChanID = YXRow.ZiChanID;
            if (string.IsNullOrEmpty(ZiChanID))
                ZiChanID = equipitem.ZiChanID;
            PlanNo = YXRow.PlanNo;
            if (string.IsNullOrEmpty(PlanNo))
                PlanNo = equipitem.PlanNo;
            if (s_alarm_shield.Length > 0)
            {
                if (alarm_acceptable_time == 0)
                {
                    alarm_acceptable_time = 30;
                }
            }
            bReset = true;
            if (OriginalAlarmState && restore_acceptable_time > 0)
            {
                isAlarm = true;
            }
        }
        public void ClearAllEvents()
        {
            DataCenter.ClearAllEvents(this);
        }
        #region 记录历史曲线
        byte[] DotData = new byte[6];
        byte[] OldDotData = new byte[6];
        TimeSpan span = new TimeSpan();
        int oldDotDataHour;
        bool bSpanDay = false;
        bool FirstDotForDay = false;
        DateTime startrecord;
        bool resetrecord = false;
        bool startrun = true;
        string FullPathName;
        string FileName;
        FileStream filestream;
        List<byte> streambytes = new List<byte>();
        byte[] b;
        int oldtotalmill;
        bool bSameValue = false;
        bool bNewFile = false;
        long key;
        void RecordCurve(bool bSpanDay)
        {
            try
            {
                if (Curve_rcd)
                {
                    Curve.PushRecordData2List(bSpanDay, (byte[])valuelist.ToArray(typeof(byte)), "X", Equip_no, Yx_no);
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile("RecordCurve error>>" + General.GetExceptionInfo(e));
            }
        }
        void RecordCurve()
        {
            if (yxvalue.GetType() == typeof(string))
                return;
            CurveRecordTime = DateTime.Now;
            if (!resetrecord)
            {
                resetrecord = true;
                startrecord = DateTime.Now;
            }
            bool v;
            int M;
            v = Convert.ToBoolean(yxvalue);
            M = DateTime.Now.Hour * 3600 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
            DotData[1] = BitConverter.GetBytes(M)[0];
            DotData[2] = BitConverter.GetBytes(M)[1];
            DotData[3] = BitConverter.GetBytes(M)[2];
            DotData[4] = BitConverter.GetBytes(M)[3];
            if (v == true)
                DotData[5] = 1;
            else
                DotData[5] = 0;
            if (startrun)
            {
                DotData[0] = 254;
                valuelist.AddRange(DotData);
                startrun = false;
                oldDotDataHour = DateTime.Now.Hour;
            }
            else
            {
                if (oldDotDataHour > DateTime.Now.Hour)
                {
                    resetrecord = false;
                    RecordCurve(false);
                    valuelist.Clear();
                    bSpanDay = true;
                    oldDotDataHour = DateTime.Now.Hour;
                    return;
                }
                oldDotDataHour = DateTime.Now.Hour;
                DotData[0] = 255;
                if (bSpanDay)
                {
                    valuelist.AddRange(DotData);
                    bSpanDay = false;
                    oldtotalmill = 0;
                    FirstDotForDay = true;
                }
                else
                {
                    int totalmill = M;
                    if (Convert.ToBoolean(YXValue) == Convert.ToBoolean(oldCurveValue))
                    {
                        bool v1;
                        v1 = Convert.ToBoolean(oldCurveValue);
                        OldDotData[0] = 255;
                        OldDotData[1] = BitConverter.GetBytes(M)[0];
                        OldDotData[2] = BitConverter.GetBytes(M)[1];
                        OldDotData[3] = BitConverter.GetBytes(M)[2];
                        OldDotData[4] = BitConverter.GetBytes(M)[3];
                        if (v1 == true)
                            OldDotData[5] = 1;
                        else
                            OldDotData[5] = 0;
                        oldtotalmill = M;
                        bSameValue = true;
                    }
                    else
                    {
                        if (bSameValue)
                        {
                            bSameValue = false;
                            if (!FirstDotForDay)
                                valuelist.AddRange(OldDotData);
                        }
                    }
                    FirstDotForDay = false;
                    if (!bSameValue)
                    {
                        valuelist.AddRange(DotData);
                        oldtotalmill = totalmill;
                    }
                }
            }
            span = DateTime.Now - startrecord;
            if (span.TotalSeconds > SetRecordSpan)
            {
                if (valuelist.Count == 0)
                {
                    valuelist.AddRange(DotData);
                }
                resetrecord = false;
                RecordCurve(true);
                valuelist.Clear();
            }
        }
        #endregion
    }
}
