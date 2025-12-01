using System;

namespace GWDataCenter.Database
{
    public class YcpTableRow
    {
        public int sta_n { get; set; }
        public int equip_no { get; set; }
        public int yc_no { get; set; }
        public string yc_nm { get; set; }
        public bool mapping { get; set; }
        public double yc_min { get; set; }
        public double yc_max { get; set; }
        public double physic_min { get; set; }
        public double physic_max { get; set; }
        public double val_min { get; set; }
        public double restore_min { get; set; }
        public double restore_max { get; set; }
        public double val_max { get; set; }
        public int val_trait { get; set; }
        public string main_instruction { get; set; }
        public string minor_instruction { get; set; }
        public int? alarm_acceptable_time { get; set; }
        public int? restore_acceptable_time { get; set; }
        public int alarm_repeat_time { get; set; }
        public string proc_advice { get; set; }
        public int lvl_level { get; set; }
        public string outmin_evt { get; set; }
        public string outmax_evt { get; set; }
        public string wave_file { get; set; }
        public string related_pic { get; set; }
        public int alarm_scheme { get; set; }
        public bool curve_rcd { get; set; }
        public double? curve_limit { get; set; }
        public string alarm_shield { get; set; }
        public string unit { get; set; }
        public int? AlarmRiseCycle { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public string related_video { get; set; }
        public string ZiChanID { get; set; }
        public string PlanNo { get; set; }
        public string SafeTime { get; set; }
        public string GWValue { get; set; }
        public DateTime? GWTime { get; set; }
        public string datatype { get; set; }
        public string yc_code { get; set; }


    }
}
