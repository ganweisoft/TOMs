﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("SetEvt")]
    public class SetEvtTableRow
    {
        [Key]
        public int ID { get; set; }
        public int sta_n { get; set; }
        [Required]
        public int equip_no { get; set; }
        public int set_no { get; set; }
        public string GWEvent { get; set; }
        public DateTime GWTime { get; set; }
        public string GWOperator { get; set; }
        public string GWSource { get; set; }
    }
}
