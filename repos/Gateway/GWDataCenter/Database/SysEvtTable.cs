﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("SysEvt")]
    public class SysEvtTableRow
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Required]
        [Column("event")]
        [Display(Name = "事件")]
        public string gwEvent { get; set; }
        [Required]
        [Display(Name = "时间")]
        public DateTime time { get; set; }
        [Display(Name = "确认人")]
        public string confirmname { get; set; }
        [Display(Name = "确认时间")]
        public DateTime confirmtime { get; set; }
        [Display(Name = "确认说明")]
        public string confirmremark { get; set; }
        [Display(Name = "GUID")]
        public string GUID { get; set; }
    }
}
