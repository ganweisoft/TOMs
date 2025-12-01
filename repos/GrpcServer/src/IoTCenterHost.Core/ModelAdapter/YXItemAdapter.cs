//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using OpenGWDataCenter.Model;

namespace IoTCenterHost.Core.ModelAdapter
{
    public class YXItemAdapter
    {
        public YXItemAdapter(YXItem item)
        {
            this.AlarmLevel = item.AlarmLevel;
            this.Restorelevel = item.Restorelevel;
            this.Level_r = item.Level_r;
            this.Level_d = item.Level_d;
            this.Alarm_scheme = item.Alarm_scheme;
            this.IsAlarm = item.IsAlarm;
            this.Proc_advice = item.Proc_advice;
            this.RestoreMsg = item.RestoreMsg;
            this.Wave_file = item.Wave_file;
            this.Restore_Wave_file = item.Restore_Wave_file;
            this.Evt_01 = item.Evt_01;
            this.Evt_10 = item.Evt_10;
            this.Yx_nm = item.Yx_nm;
            this.AlarmMsg = item.AlarmMsg;
            this.Yx_no = item.Yx_no;
            this.Comments = item.Comments;
            this.Sta_n = item.Sta_n;
            this.Bufang = item.Bufang;
            this.Equip_no = item.Equip_no;
            this.OriginalAlarmState = item.OriginalAlarmState;
            this.Related_pic = item.Related_pic;
            this.ManualReset = item.ManualReset;
            this.Curve_rcd = item.Curve_rcd;
            this.YXValue = item.YXValue;
            this.OldYXValue = item.OldYXValue;
            this.OldValue = item.OldValue;
            this.YXState = item.YXState;
        }
        public int? AlarmLevel { get; }
        public int? Restorelevel { get; }
        public int? Level_r { get; }
        public int? Level_d { get; }
        public int? Alarm_scheme { get; }
        public bool IsAlarm { get; set; }
        public string Proc_advice { get; set; }
        public string RestoreMsg { get; }
        public string Wave_file { get; }
        public string Restore_Wave_file { get; }
        public string Evt_01 { get; set; }
        public string Evt_10 { get; set; }
        public string Yx_nm { get; set; }
        public string AlarmMsg { get; }
        public int Yx_no { get; }
        public string Comments { get; set; }
        public int Sta_n { get; }
        public bool Bufang { get; set; }
        public int Equip_no { get; }
        public bool OriginalAlarmState { get; set; }
        public string Related_pic { get; }
        public bool ManualReset { get; set; }
        public bool Curve_rcd { get; }
        public object YXValue { get; set; }
        public object OldYXValue { get; }
        public object OldValue { get; }
        public string YXState { get; set; }
    }
}
