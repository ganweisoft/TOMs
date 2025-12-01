﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.IO;
namespace GWDataCenter
{
    public enum CurveRecordInterval
    {
        Millisecond, Second
    }
    public class YCItem
    {
        public EquipItem equipitem;
        public YcpAlarmStateTrack AlarmStateTrack;
        YcpTableRow ycrowitem;
        int sta_n;
        object equip_no, yc_no;
        object ycvalue, oldvalue, oldycvalue = "??", oldCurveValue;
        string EquipNm, yc_nm;
        bool mapping;
        double yc_min, yc_max, physic_min, physic_max, val_min, val_max, restore_min, restore_max;
        double? curve_limit;
        int val_trait;
        int alarm_acceptable_time, restore_acceptable_time, alarm_repeat_time, alarm_scheme, lvl_level;
        string proc_advice, outmin_evt, outmax_evt, restore_wave_file, wave_file, related_pic;
        DateTime safe_bgn, safe_end;
        List<SafeTimeSpan> SafeTimeSpanList = new List<SafeTimeSpan>();
        public string alarmflag;
        public static object m_DefaultValue = "***";
        public string main_instruction, minor_instruction;
        object curve_rcd = false;
        TimeSpan AlarmSpan, RestoreSpan, RepeatAlarmSpan;
        DateTime StartAlarmTime, StopAlarmTime, RepeatAlarmTime;
        DateTime CurveRecordTime;
        TimeSpan CurveRecordTimeSpan;
        public bool bRepeatAlarm = false;
        public bool IsWuBao = false;
        public bool AlarmState, RestoreAlarmState, alreadyAlarm;
        object isAlarm = false;
        string alarmMsg = " ";
        string comments = " ";
        public bool bCanMonitor = false;
        public string strUnit = "";
        public int DotBitNum = -1;
        public string s_alarm_shield = "";
        object bufang = true;
        object oldbufang = true;
        public string RestoreMsg;
        public int? AlarmRiseCycle;
        public string Reserve1;
        public string Reserve2;
        public string Reserve3;
        public string related_video, ZiChanID, PlanNo;
        bool bReset = false;
        public CurveRecordInterval m_CurveRecordInterval;
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
                        YCValue = ycvalue;
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
                    return comments;
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
        public System.Collections.ArrayList valuelist = new System.Collections.ArrayList();
        public event YCValueChangedEventHandler ValueChanged;
        public delegate void YCValueChangedEventHandler(object sender, EventArgs e);
        public event YCAlarmEventHandler Alarmed;
        public delegate void YCAlarmEventHandler(object sender, EventArgs e);
        public event YCAlarmRestoreEventHandler AlarmRestored;
        public delegate void YCAlarmRestoreEventHandler(object sender, EventArgs e);
        int RecCurveTotalMs, OldRecCurveTotalMs;
        const int SetRecordSpan = 300;
        public void YCToPhysic(ref object val)
        {
            if (val.GetType() == typeof(string))
                return;
            if (equipitem.communication_drv.IndexOf("DataSimu") >= 0)
                return;
            if (mapping)
            {
                try
                {
                    double fDefYC = yc_max - yc_min;
                    if (Math.Abs(fDefYC) > 0.00001)
                    {
                        double f;
                        f = Convert.ToDouble(val);
                        val = physic_min + ((f - yc_min) * (physic_max - physic_min)) / fDefYC;
                    }
                }
                catch (Exception e)
                {
                    MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), 0, false);
                }
            }
        }
        public object YCValue
        {
            get
            {
                lock (ycvalue)
                {
                    return ycvalue;
                }
            }
            set
            {
                lock (ycvalue)
                {
                    ycvalue = value;
                    oldvalue = value;
                    YCToPhysic(ref ycvalue);
                }
                if (Curve_rcd)
                {
                    RecCurveTotalMs = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond;
                    try
                    {
                        if (RecCurveTotalMs != OldRecCurveTotalMs)
                        {
                            if (ycvalue.GetType() != typeof(string))
                            {
                                if (oldycvalue == m_DefaultValue)
                                {
                                    RecordCurve();
                                    oldCurveValue = ycvalue;
                                }
                                else
                                {
                                    if (Math.Abs(Convert.ToDouble(ycvalue) - Convert.ToDouble(oldCurveValue)) >= curve_limit)
                                    {
                                        RecordCurve();
                                        oldCurveValue = ycvalue;
                                    }
                                    CurveRecordTimeSpan = DateTime.Now - CurveRecordTime;
                                    if (CurveRecordTimeSpan.TotalSeconds >= SetRecordSpan)
                                    {
                                        RecordCurve();
                                        oldCurveValue = ycvalue;
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
                lock (ycvalue)
                {
                    if (ycvalue.GetType() != typeof(string))
                    {
                        if (DotBitNum >= 0)
                        {
                            try
                            {
                                ycvalue = Convert.ToDouble(Math.Round(Convert.ToDouble(ycvalue), DotBitNum));
                            }
                            catch (Exception e)
                            {
                                string s = this.equip_no.ToString() + "-" + this.Yc_no.ToString();
                                DataCenter.WriteLogFile(s + e.ToString());
                            }
                        }
                    }
                }
                if (Bufang)
                {
                    SetEvent();
                }
                else
                {
                    SetNoAlarm();
                }
                oldycvalue = ycvalue;
            }
        }
        public void SetNoAlarm()
        {
            if (IsAlarm)
            {
                isAlarm = false;
                alreadyAlarm = false;
                AlarmState = false;
                RestoreAlarm();
            }
            ValueChanged?.Invoke(this, new EventArgs());
        }
        public int oldRecCurveTotalMs
        {
            get
            {
                return OldRecCurveTotalMs;
            }
        }
        public object OldYCValue
        {
            get
            {
                return oldycvalue;
            }
        }
        public object OldValue
        {
            get
            {
                return oldvalue;
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
        public string AlarmMsg
        {
            get
            {
                return alarmMsg;
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
        public int Yc_no
        {
            get
            {
                lock (yc_no)
                {
                    return (int)yc_no;
                }
            }
        }
        public string Yc_nm
        {
            get
            {
                lock (yc_nm)
                {
                    return yc_nm;
                }
            }
            set
            {
                lock (yc_nm)
                {
                    yc_nm = value;
                }
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
        }
        public int Level
        {
            get
            {
                return lvl_level;
            }
        }
        public int Alarm_scheme
        {
            get
            {
                return alarm_scheme;
            }
        }
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
            if (!oldycvalue.Equals(ycvalue) || bForceUpdate)
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
            if (ycvalue.GetType() != typeof(string))
            {
                double f = Convert.ToDouble(ycvalue);
                bool alm = (f > val_max || f < val_min);
                if (Convert.ToBoolean(val_trait & 0x1))
                    alm = !alm;
                if (alm)
                {
                    if (!AlarmState)
                    {
                        StartAlarmTime = DateTime.Now;
                        AlarmState = true;
                    }
                }
                else
                {
                    AlarmState = false;
                }
                bRepeatAlarm = false;
                if (AlarmState && !alreadyAlarm)
                {
                    AlarmSpan = DateTime.Now - StartAlarmTime;
                    if (AlarmSpan.TotalSeconds >= alarm_acceptable_time && !IsSafeTime() && lvl_level > 0)
                    {
                        if (Convert.ToBoolean(val_trait))
                        {
                            alarmMsg = EquipNm + "-" + yc_nm + ":" + ResourceService.GetString("AlarmCenter.Alarm");
                            comments = ResourceService.GetString("AlarmCenter.Alarm") + " : " + proc_advice;
                        }
                        else
                        {
                            string strRealValue = string.Format("{0}", YCValue);
                            if (f > val_max)
                            {
                                outmax_evt = ycrowitem.outmax_evt;
                                if (outmax_evt.IndexOf("{}") >= 0)
                                {
                                    outmax_evt = outmax_evt.Replace("{}", strRealValue);
                                }
                                alarmMsg = EquipNm + "-" + yc_nm + ":" + outmax_evt;
                                comments = outmax_evt + " : " + proc_advice;
                                alarmflag = "0-1";
                            }
                            if (f < val_min)
                            {
                                outmin_evt = ycrowitem.outmin_evt;
                                if (outmin_evt.IndexOf("{}") >= 0)
                                {
                                    outmin_evt = outmin_evt.Replace("{}", strRealValue);
                                }
                                alarmMsg = EquipNm + "-" + yc_nm + ":" + outmin_evt;
                                comments = outmin_evt + " : " + proc_advice;
                                alarmflag = "1-0";
                            }
                        }
                        isAlarm = true;
                        if (Alarmed != null)
                        {
                            evtGUID = Guid.NewGuid().ToString();
                            OriginalAlarmState = true;
                            Alarmed(this, new EventArgs());
                            AlarmStateTrack.SetState4EquipmentLinkage();
                            ValueChanged?.Invoke(this, new EventArgs());
                            if ((Alarm_scheme & 0x01) > 0 && !bRepeatAlarm)
                            {
                                if (AlarmStateTrack.IsDifferentState(1))
                                    AlarmDispatch.Add2MessageService(Level, AlarmMsg, Proc_advice, Equip_no, "C", Yc_no, Wave_file, Related_pic, related_video, ZiChanID, PlanNo, evtGUID);
                            }
                        }
                        RepeatAlarmTime = DateTime.Now;
                        alreadyAlarm = true;
                    }
                }
            }
        }
        bool IsSafeTime()
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
            else
            {
                foreach (SafeTimeSpan ts in equipitem.SafeTimeSpanList)
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
        void OnAlarmRestored()
        {
            if (Convert.ToBoolean(isAlarm) || IsWuBao || bReset || RestoreAlarmState)
            {
                bReset = false;
                if (ycvalue.GetType() != typeof(string))
                {
                    double f = Convert.ToDouble(ycvalue);
                    bool alm = (f > restore_min && f < restore_max);
                    if (!Convert.ToBoolean(isAlarm) && OriginalAlarmState)
                    {
                        if (f < val_max || f > val_min)
                            alm = true;
                    }
                    if (Convert.ToBoolean(val_trait))
                        alm = !alm;
                    if (IsSafeTime() && Convert.ToBoolean(isAlarm) && !IsWuBao)
                    {
                        alm = !alm;
                    }
                    if (alm)
                    {
                        if (!RestoreAlarmState)
                        {
                            RestoreAlarmState = true;
                            StopAlarmTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        RestoreAlarmState = false;
                    }
                    if (Level == 0)
                    {
                        if (IsWuBao)
                            IsWuBao = false;
                        RestoreAlarm();
                        return;
                    }
                    if (RestoreAlarmState)
                    {
                        RestoreSpan = DateTime.Now - StopAlarmTime;
                        if (RestoreSpan.TotalSeconds >= restore_acceptable_time)
                        {
                            if (IsWuBao)
                                IsWuBao = false;
                            RestoreAlarm();
                        }
                    }
                }
            }
        }
        void RestoreAlarm()
        {
            if (!OriginalAlarmState)
                return;
            isAlarm = false;
            AlarmState = false;
            alreadyAlarm = false;
            alarmMsg = string.Format("{0}:{1}---{2}:{3}", EquipNm + "-" + yc_nm, ResourceService.GetString("AlarmCenter.DataCenter.Msg1"), ResourceService.GetString("AlarmCenter.DataCenter.YCMsg1"), YCValue);
            comments = "";
            if (AlarmRestored != null)
            {
                evtGUID = Guid.NewGuid().ToString();
                OriginalAlarmState = false;
                AlarmRestored(this, new EventArgs());
                AlarmStateTrack.SetState4EquipmentLinkage();
                RestoreAlarmState = false;
                ValueChanged?.Invoke(this, new EventArgs());
                if ((Alarm_scheme & 0x01) > 0)
                {
                    if (AlarmStateTrack.IsDifferentState(1))
                        AlarmDispatch.Add2MessageService(0, AlarmMsg, "", Equip_no, "C", Yc_no, null, null, related_video, ZiChanID, PlanNo, evtGUID);
                }
                ;
            }
        }
        public YCItem(YcpTableRow YCRow, EquipItem ei)
        {
            equipitem = ei;
            AlarmStateTrack = new YcpAlarmStateTrack(YCRow, this);
            try
            {
                Init(YCRow);
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), (int)equip_no);
            }
            ycvalue = oldvalue = m_DefaultValue;
            AlarmState = false;
            RestoreAlarmState = false;
            isAlarm = false;
            alreadyAlarm = false;
        }
        public void Init(YcpTableRow YCRow)
        {
            isAlarm = false;
            AlarmState = false;
            alreadyAlarm = false;
            RestoreAlarmState = false;
            ycrowitem = YCRow;
            sta_n = YCRow.sta_n;
            equip_no = YCRow.equip_no;
            yc_no = YCRow.yc_no;
            yc_nm = YCRow.yc_nm;
            EquipNm = equipitem.Equip_nm;
            mapping = YCRow.mapping;
            yc_min = YCRow.yc_min;
            yc_max = YCRow.yc_max;
            physic_min = YCRow.physic_min;
            physic_max = YCRow.physic_max;
            val_min = YCRow.val_min;
            val_max = YCRow.val_max;
            curve_limit = YCRow.curve_limit == null ? 0 : YCRow.curve_limit;
            restore_min = YCRow.restore_min;
            restore_max = YCRow.restore_max;
            val_trait = YCRow.val_trait;
            manualreset = val_trait & 0x2;
            m_CurveRecordInterval = (val_trait & 0x4) > 0 ? CurveRecordInterval.Millisecond : CurveRecordInterval.Second;
            lvl_level = YCRow.lvl_level;
            curve_rcd = YCRow.curve_rcd;
            alarm_acceptable_time = (int)(YCRow.alarm_acceptable_time == null ? 0 : YCRow.alarm_acceptable_time);
            restore_acceptable_time = (int)(YCRow.restore_acceptable_time == null ? 0 : YCRow.restore_acceptable_time);
            alarm_repeat_time = YCRow.alarm_repeat_time;
            alarm_scheme = YCRow.alarm_scheme;
            main_instruction = YCRow.main_instruction;
            minor_instruction = YCRow.minor_instruction;
            proc_advice = YCRow.proc_advice;
            outmin_evt = YCRow.outmin_evt;
            outmax_evt = YCRow.outmax_evt;
            string wf = YCRow.wave_file ?? "";
            string[] fs = wf.Split('/');
            wave_file = fs[0];
            if (fs.Length == 2)
                restore_wave_file = fs[1];
            related_pic = YCRow.related_pic ?? "";
            GetSafeTimeSpanList(YCRow.SafeTime ?? "");
            strUnit = YCRow.unit ?? "";
            AlarmRiseCycle = YCRow.AlarmRiseCycle == null ? 0 : YCRow.AlarmRiseCycle;
            Reserve1 = YCRow.Reserve1 ?? "";
            Reserve2 = YCRow.Reserve2 ?? "";
            Reserve3 = YCRow.Reserve3 ?? "";
            related_video = YCRow.related_video;
            if (string.IsNullOrEmpty(related_video))
                related_video = equipitem.related_video;
            ZiChanID = YCRow.ZiChanID;
            if (string.IsNullOrEmpty(ZiChanID))
                ZiChanID = equipitem.ZiChanID;
            PlanNo = YCRow.PlanNo;
            if (string.IsNullOrEmpty(PlanNo))
                PlanNo = equipitem.PlanNo;
            if (!string.IsNullOrEmpty(strUnit))
            {
                string[] DotS = strUnit.Split('[', ']');
                if (DotS.Length >= 3 && strUnit[0] == '[')
                {
                    int.TryParse(DotS[1], out DotBitNum);
                    strUnit = DotS[2];
                }
                else
                {
                    DotBitNum = 1;
                }
            }
            s_alarm_shield = YCRow.alarm_shield ?? "";
            if (s_alarm_shield.Length > 0)
            {
                if (alarm_acceptable_time == 0)
                {
                    alarm_acceptable_time = 30;
                }
            }
            oldCurveValue = 0;
            bReset = true;
            if (OriginalAlarmState && restore_acceptable_time > 0)
            {
                isAlarm = true;
                AlarmState = true;
            }
        }
        public void ClearAllEvents()
        {
            DataCenter.ClearAllEvents(this);
        }
        ~YCItem()
        {
            if (valuelist.Count > 0)
            {
                valuelist.Clear();
            }
        }
        #region 记录历史曲线
        byte[] DotData = new byte[13];
        byte[] OldDotData = new byte[13];
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
        Int64 key;
        void RecordCurve(bool bSpanDay)
        {
            try
            {
                if (Curve_rcd)
                {
                    Curve.PushRecordData2List(bSpanDay, (byte[])valuelist.ToArray(typeof(byte)), "C", Equip_no, Yc_no);
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile("RecordCurve error>>" + General.GetExceptionInfo(e));
            }
        }
        void RecordCurve()
        {
            if (ycvalue.GetType() == typeof(string))
                return;
            CurveRecordTime = DateTime.Now;
            if (!resetrecord)
            {
                resetrecord = true;
                startrecord = DateTime.Now;
            }
            double v;
            int M;
            if (DotBitNum >= 0)
            {
                ycvalue = Convert.ToDouble(Math.Round(Convert.ToDouble(ycvalue), DotBitNum));
                if (oldCurveValue is double)
                {
                    oldCurveValue = Convert.ToDouble(Math.Round(Convert.ToDouble(oldCurveValue), DotBitNum));
                }
            }
            v = Convert.ToDouble(ycvalue);
            M = DateTime.Now.Hour * 3600 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
            DotData[1] = BitConverter.GetBytes(M)[0];
            DotData[2] = BitConverter.GetBytes(M)[1];
            DotData[3] = BitConverter.GetBytes(M)[2];
            DotData[4] = BitConverter.GetBytes(M)[3];
            DotData[5] = BitConverter.GetBytes(v)[0];
            DotData[6] = BitConverter.GetBytes(v)[1];
            DotData[7] = BitConverter.GetBytes(v)[2];
            DotData[8] = BitConverter.GetBytes(v)[3];
            DotData[9] = BitConverter.GetBytes(v)[4];
            DotData[10] = BitConverter.GetBytes(v)[5];
            DotData[11] = BitConverter.GetBytes(v)[6];
            DotData[12] = BitConverter.GetBytes(v)[7];
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
                    if (YCValue.Equals(oldCurveValue))
                    {
                        double v1;
                        v1 = Convert.ToDouble(oldCurveValue);
                        OldDotData[0] = 255;
                        OldDotData[1] = BitConverter.GetBytes(M)[0];
                        OldDotData[2] = BitConverter.GetBytes(M)[1];
                        OldDotData[3] = BitConverter.GetBytes(M)[2];
                        OldDotData[4] = BitConverter.GetBytes(M)[3];
                        OldDotData[5] = BitConverter.GetBytes(v1)[0];
                        OldDotData[6] = BitConverter.GetBytes(v1)[1];
                        OldDotData[7] = BitConverter.GetBytes(v1)[2];
                        OldDotData[8] = BitConverter.GetBytes(v1)[3];
                        OldDotData[9] = BitConverter.GetBytes(v1)[4];
                        OldDotData[10] = BitConverter.GetBytes(v1)[5];
                        OldDotData[11] = BitConverter.GetBytes(v1)[6];
                        OldDotData[12] = BitConverter.GetBytes(v1)[7];
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
