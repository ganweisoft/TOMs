﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWProcTimeSysTable")]
    public class GWProcTimeSysTableRow
    {
        [Key]
        public int ID { get; set; }
        public int TableID { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeDur { get; set; }
        public int proc_code { get; set; }
        public string cmd_nm { get; set; }
    }
}
