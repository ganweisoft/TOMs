﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("Yxp")]
    public class YxpTableRow
    {
        [Required]
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Required]
        [Display(Name = "设备号")]
        public int equip_no { get; set; }
        [Required]
        [Display(Name = "测点号")]
        public int yx_no { get; set; }
        [Required]
        [Display(Name = "测点名称")]
        public string yx_nm { get; set; }
        [Display(Name = "处理意见0-1")]
        public string proc_advice_r { get; set; }
        [Display(Name = "处理意见1-0")]
        public string proc_advice_d { get; set; }
        [Required]
        [Display(Name = "级别0变1")]
        public int level_r { get; set; }
        [Required]
        [Display(Name = "级别1变0")]
        public int level_d { get; set; }
        [Display(Name = "0到1事件")]
        public string evt_01 { get; set; }
        [Display(Name = "1到0事件")]
        public string evt_10 { get; set; }
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
        [Display(Name = "声音文件")]
        public string wave_file { get; set; }
        [Display(Name = "关联页面")]
        public string related_pic { get; set; }
        [Display(Name = "报警方式")]
        public int alarm_scheme { get; set; }
        [Display(Name = "是否取反")]
        public bool inversion { get; set; }
        [Display(Name = "曲线记录")]
        public bool curve_rcd { get; set; }
        [Display(Name = "初始状态")]
        public int initval { get; set; }
        [Display(Name = "转换特征")]
        public int val_trait { get; set; }
        [Display(Name = "报警屏蔽")]
        public string alarm_shield { get; set; }
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
