﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("Ycp")]
    public class YcpTableRow
    {
        [Required]
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Required]
        [Display(Name = "设备号")]
        public int equip_no { get; set; }
        [Required]
        [Display(Name = "测点号")]
        public int yc_no { get; set; }
        [Required]
        [Display(Name = "测点名称")]
        public string yc_nm { get; set; }
        [Display(Name = "是否映射")]
        public bool mapping { get; set; }
        [Display(Name = "实测最小值")]
        public double yc_min { get; set; }
        [Display(Name = "实测最大值")]
        public double yc_max { get; set; }
        [Display(Name = "最小值")]
        public double physic_min { get; set; }
        [Display(Name = "最大值")]
        public double physic_max { get; set; }
        [Display(Name = "下限值")]
        public double val_min { get; set; }
        [Display(Name = "回复下限值")]
        public double restore_min { get; set; }
        [Display(Name = "回复上限值")]
        public double restore_max { get; set; }
        [Display(Name = "上限值")]
        public double val_max { get; set; }
        [Display(Name = "转换特征值")]
        public int val_trait { get; set; }
        [Display(Name = "命令1")]
        public string main_instruction { get; set; }
        [Display(Name = "命令2")]
        public string minor_instruction { get; set; }
        [Display(Name = "越限容许时间(秒)")]
        public int? alarm_acceptable_time { get; set; }
        [Display(Name = "恢复容许时间(秒)")]
        public int? restore_acceptable_time { get; set; }
        [Display(Name = "重复报警时间(分钟)")]
        public int alarm_repeat_time { get; set; }
        [Display(Name = "处理意见")]
        public string proc_advice { get; set; }
        [Required]
        [Display(Name = "事件级别")]
        public int lvl_level { get; set; }
        [Display(Name = "越下限事件")]
        public string outmin_evt { get; set; }
        [Display(Name = "越上限事件")]
        public string outmax_evt { get; set; }
        [Display(Name = "声音文件")]
        public string wave_file { get; set; }
        [Display(Name = "关联页面")]
        public string related_pic { get; set; }
        [Display(Name = "报警方式")]
        public int alarm_scheme { get; set; }
        [Display(Name = "是否记录曲线")]
        public bool curve_rcd { get; set; }
        [Display(Name = "记录曲线阈值")]
        public double? curve_limit { get; set; }
        [Display(Name = "报警屏蔽")]
        public string alarm_shield { get; set; }
        [Display(Name = "单位")]
        public string unit { get; set; }
        [Display(Name = "报警升级周期")]
        public int? AlarmRiseCycle { get; set; }
        [Display(Name = "保留字段1")]
        public string Reserve1 { get; set; }
        [Display(Name = "保留字段2")]
        public string Reserve2 { get; set; }
        [Display(Name = "保留字段3")]
        public string Reserve3 { get; set; }
        [Display(Name = "关联视频")]
        public string related_video { get; set; }
        [Display(Name = "关联资产")]
        public string ZiChanID { get; set; }
        [Display(Name = "关联预案")]
        public string PlanNo { get; set; }
        [Display(Name = "安全时段")]
        public string SafeTime { get; set; }
    }
}
