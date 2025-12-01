//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections;

namespace IoTCenterHost.Core
{
    public class YCItemModel
    {
        public static object m_DefaultValue;
        public bool bForceUpdate;
        public ArrayList valuelist;
        public string PlanNo;
        public string ZiChanID;
        public string related_video;
        public string Reserve2;
        public string Reserve1;
        public int? AlarmRiseCycle;
        public string RestoreMsg;
        public string s_alarm_shield;
        public string Reserve3;
        public string strUnit;
        public int DotBitNum;
        public string alarmflag;
        public string main_instruction;
        public bool bRepeatAlarm;
        public bool IsWuBao;
        public string minor_instruction;
        public bool RestoreAlarmState;
        public bool alreadyAlarm;
        public bool bCanMonitor;
        public bool AlarmState;
        public bool Curve_rcd { get; }
        public int Equip_no { get; }
        public string AlarmMsg { get; }
        public int Sta_n { get; }
        public string Proc_advice { get; }
        public string Yc_nm { get; set; }
        public string Wave_file { get; }
        public string Restore_Wave_file { get; }
        public bool IsAlarm { get; set; }
        public int Yc_no { get; }
        public object OldValue { get; }
        public int Alarm_scheme { get; }
        public int oldRecCurveTotalMs { get; }
        public object YCValue { get; set; }
        public string Related_pic { get; }
        public string Comments { get; set; }
        public bool OriginalAlarmState { get; set; }
        public bool ManualReset { get; set; }
        public bool Bufang { get; set; }
        public int Level { get; }
        public object OldYCValue { get; }

        public event YCValueChangedEventHandler ValueChanged;
        public event YCAlarmEventHandler Alarmed;
        public event YCAlarmRestoreEventHandler AlarmRestored;

        public delegate void YCValueChangedEventHandler(object sender, EventArgs e);
        public delegate void YCAlarmEventHandler(object sender, EventArgs e);
        public delegate void YCAlarmRestoreEventHandler(object sender, EventArgs e);
    }
}