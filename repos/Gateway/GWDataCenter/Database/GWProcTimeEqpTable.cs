﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWProcTimeEqpTable")]
    public class GWProcTimeEqpTableRow
    {
        [Key]
        public int ID { get; set; }
        public int TableID { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeDur { get; set; }
        public int equip_no { get; set; }
        public int set_no { get; set; }
        public string value { get; set; }
    }
}
