﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWProcCycleTList")]
    public class GWProcCycleTListTableRow
    {
        [Key]
        public int TableID { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public DateTime BeginTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public bool ZhenDianDo { get; set; }
        public bool ZhidingDo { get; set; }
        public bool CycleMustFinish { get; set; }
        public DateTime ZhidingTime { get; set; }
        public int MaxCycleNum { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
