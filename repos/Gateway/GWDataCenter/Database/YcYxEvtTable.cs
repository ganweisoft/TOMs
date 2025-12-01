﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("YcYxEvt")]
    public class YcYxEvtTableRow
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Required]
        [Display(Name = "设备号")]
        public int equip_no { get; set; }
        [Required]
        [Display(Name = "测点号")]
        public int ycyx_no { get; set; }
        [Required]
        [Display(Name = "测点类型")]
        public string ycyx_type { get; set; }
        [Required]
        [Column("event")]
        [Display(Name = "事件")]
        public string gwEvent { get; set; }
        public int snapshotlevel { get; set; }
        [Required]
        [Display(Name = "时间")]
        public DateTime time { get; set; }
        [Display(Name = "处理意见")]
        public string proc_rec { get; set; }
        [Display(Name = "确认人")]
        public string confirmname { get; set; }
        [Display(Name = "确认时间")]
        public DateTime confirmtime { get; set; }
        [Display(Name = "是否误报")]
        public bool WuBao { get; set; }
        [Display(Name = "报警级别")]
        public int alarmlevel { get; set; }
        [Display(Name = "备注")]
        public string confirmremark { get; set; }
        [Display(Name = "GUID")]
        public string GUID { get; set; }
    }
}
