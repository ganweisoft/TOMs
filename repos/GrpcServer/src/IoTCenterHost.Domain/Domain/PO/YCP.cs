//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.AppServices.Domain.PO
{
    public class YCP
    {
        public int? sta_n { get; set; }
        public int? equip_no { get; set; }
        public int? yc_no { get; set; }
        public string yc_nm { get; set; }
        public byte? mapping { get; set; }
        public float? yc_min { get; set; }
        public float? yc_max { get; set; }
        public float? physic_min { get; set; }
        public float? physic_max { get; set; }
        public float? val_min { get; set; }
        public float? restore_min { get; set; }
        public float? restore_max { get; set; }
        public float? val_max { get; set; }
        public int? val_trait { get; set; }
        public string? main_instruction { get; set; }
        public string? minor_instruction { get; set; }
        public DateTime? safe_bgn { get; set; }
        public DateTime? safe_end { get; set; }
        public int alarm_acceptable_time { get; set; }
        public int restore_acceptable_time { get; set; }
        public int alarm_repeat_time { get; set; }
        public string? proc_advice { get; set; }
        public int lvl_level { get; set; }
        public string? outmin_evt { get; set; }
        public string? outmax_evt { get; set; }
        public string? wave_file { get; set; }
        public string? related_pic { get; set; }
        public int alarm_scheme { get; set; }
        public byte curve_rcd { get; set; }
        public float curve_limit { get; set; }
        public string? alarm_shield { get; set; }
        public string? unit { get; set; }
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
