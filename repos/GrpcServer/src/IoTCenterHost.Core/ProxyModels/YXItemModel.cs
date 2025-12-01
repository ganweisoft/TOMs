//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using System;
using System.Collections;

namespace IoTCenterHost.Core
{
    public class YXItemModel
    {
        public static object m_DefaultValue;
        public bool bForceUpdate;
        public ArrayList valuelist;
        public string PlanNo;
        public string ZiChanID;
        public string related_video;
        public string Reserve3;
        public string Reserve2;
        public string Reserve1;
        public int? AlarmRiseCycle;
        public bool IsWuBao;
        public bool bCanMonitor;
        public string alarmflag;
        public string restoreMsg;
        public string alarmMsg;
        public bool RestoreAlarmState;
        public bool Statefor1_0;
        public bool Statefor0_1;
        public bool bRepeatAlarm;
        public string s_alarm_shield;


        public int AlarmLevel { get; }
        public int Restorelevel { get; }
        public int Level_r { get; }
        public int Level_d { get; }
        public int Alarm_scheme { get; }
        public bool IsAlarm { get; set; }
        public string Proc_advice { get; set; }
        public string RestoreMsg { get; }
        public string Wave_file { get; }
        public string Restore_Wave_file { get; }
        public string Evt_01 { get; set; }
        public string Evt_10 { get; set; }
        public string Yx_nm { get; set; }
        public string AlarmMsg { get; }
        public int Yx_no { get; set; }
        public string Comments { get; set; }
        public int Sta_n { get; }
        public bool Bufang { get; set; }
        public int Equip_no { get; set; }
        public bool OriginalAlarmState { get; set; }
        [JsonProperty("related_pic")]
        public string Related_pic { get; }
        public bool ManualReset { get; set; }
        public bool Curve_rcd { get; }
        public object YXValue { get; set; }
        public object OldYXValue { get; }
        public object OldValue { get; }
        public string YXState { get; set; }

        public event YXValueChangedEventHandler ValueChanged;
        public event YXAlarmEventHandler Alarmed;
        public event YXAlarmRestoreEventHandler AlarmRestored;

        public delegate void YXAlarmEventHandler(object sender, EventArgs e);
        public delegate void YXValueChangedEventHandler(object sender, EventArgs e);
        public delegate void YXAlarmRestoreEventHandler(object sender, EventArgs e);
    }
}