//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using OpenGWDataCenter.Model;

namespace IoTCenterHost.Core.ModelAdapter
{
    public class YCItemAdapter
    {
        public YCItemAdapter(YCItem yCItem)
        {
            this.Curve_rcd = yCItem.Curve_rcd;
            this.Equip_no = yCItem.Equip_no;
            this.AlarmMsg = yCItem.AlarmMsg;
            this.Sta_n = yCItem.Sta_n;
            this.Proc_advice = yCItem.Proc_advice;
            this.Yc_nm = yCItem.Yc_nm;
            this.Wave_file = yCItem.Wave_file;
            this.Restore_Wave_file = yCItem.Restore_Wave_file;
            this.IsAlarm = yCItem.IsAlarm;
            this.Yc_no = yCItem.Yc_no;
            this.OldValue = yCItem.OldValue;
            this.Alarm_scheme = yCItem.Alarm_scheme;
            this.oldRecCurveTotalMs = yCItem.oldRecCurveTotalMs;
            this.YCValue = yCItem.YCValue;
            this.Related_pic = yCItem.Related_pic;
            this.Comments = yCItem.Comments;
            this.OriginalAlarmState = yCItem.OriginalAlarmState;
            this.ManualReset = yCItem.ManualReset;
            this.Bufang = yCItem.Bufang;
            this.Level = yCItem.Level;
            this.OldYCValue = yCItem.OldYCValue;
        }
        public object m_DefaultValue;
        public YcpAlarmStateTrack AlarmStateTrack;
        public bool bForceUpdate;
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
        public int? Alarm_scheme { get; }
        public int oldRecCurveTotalMs { get; }
        public object YCValue { get; set; }
        public string Related_pic { get; }
        public string Comments { get; set; }
        public bool OriginalAlarmState { get; set; }
        public bool ManualReset { get; set; }
        public bool Bufang { get; set; }
        public int? Level { get; }
        public object OldYCValue { get; }
    }
}
