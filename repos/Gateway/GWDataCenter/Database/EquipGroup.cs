﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("EquipGroup")]
    public class EquipGroupTableRow
    {
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Key]
        [Display(Name = "分组号")]
        public int group_no { get; set; }
        [Required]
        [Display(Name = "分组名称")]
        public string group_name { get; set; }
        [Display(Name = "设备组合字符串")]
        public string equipcomb { get; set; }
    }
}
