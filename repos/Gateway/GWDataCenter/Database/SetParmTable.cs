﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("SetParm")]
    public class SetParmTableRow
    {
        [Required]
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Required]
        [Display(Name = "设备号")]
        public int equip_no { get; set; }
        [Required]
        [Display(Name = "设置号")]
        public int set_no { get; set; }
        [Display(Name = "设置名称")]
        public string set_nm { get; set; }
        [Required]
        [Display(Name = "设置类型")]
        public string set_type { get; set; }
        [Display(Name = "命令1")]
        public string main_instruction { get; set; }
        [Display(Name = "命令2")]
        public string minor_instruction { get; set; }
        [Display(Name = "是否记录")]
        public bool record { get; set; }
        [Display(Name = "动作")]
        public string action { get; set; }
        [Display(Name = "设置值")]
        public string value { get; set; }
        [Display(Name = "是否执行")]
        public bool canexecution { get; set; }
        [Display(Name = "语音标签")]
        public string VoiceKeys { get; set; }
        [Display(Name = "是否接受语音控制")]
        public bool EnableVoice { get; set; }
        [Display(Name = "二维码设备号")]
        public int qr_equip_no { get; set; } = 0;
        [Display(Name = "保留字段1")]
        public string Reserve1 { get; set; }
        [Display(Name = "保留字段2")]
        public string Reserve2 { get; set; }
        [Display(Name = "保留字段3")]
        public string Reserve3 { get; set; }
    }
}
