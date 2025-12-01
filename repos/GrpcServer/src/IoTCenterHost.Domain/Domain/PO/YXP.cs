//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.AppServices.Domain.PO
{
    public class YXP
    {
        public int sta_n { get; set; }
        public int equip_no { get; set; }
        public int yx_no { get; set; }
        public string? yx_nm { get; set; }
        public string? proc_advice_r { get; set; }
        public string? proc_advice_d { get; set; }
        public int level_r { get; set; }
        public int level_d { get; set; }
        public string? evt_01 { get; set; }
        public string? evt_10 { get; set; }
        public string? main_instruction { get; set; }
        public string? minor_instruction { get; set; }
        public byte curve_rcd { get; set; }
        public DateTime? safe_bgn { get; set; }
        public DateTime? safe_end { get; set; }
        public int alarm_acceptable_time { get; set; }
        public int restore_acceptable_time { get; set; }
        public int alarm_repeat_time { get; set; }
        public string? wave_file { get; set; }
        public string? related_pic { get; set; }
        public int alarm_scheme { get; set; }
        public byte inversion { get; set; }
        public int initval { get; set; }
        public int val_trait { get; set; }
        public string? alarm_shield { get; set; }
        public int AlarmRiseCycle { get; set; }
        public string? Reserve1 { get; set; }
        public string? Reserve2 { get; set; }
        public string? Reserve3 { get; set; }
        public string? related_video { get; set; }
        public string? ZiChanID { get; set; }
        public string? PlanNo { get; set; }
        public string? SafeTime { get; set; }
    }
}
