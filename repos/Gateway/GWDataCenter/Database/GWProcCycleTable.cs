﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWProcCycleTable")]
    public class GWProcCycleTableRow
    {
        public int TableID { get; set; }
        public int DoOrder { get; set; }
        public string Type { get; set; }
        public int equip_no { get; set; }
        public int set_no { get; set; }
        public string value { get; set; }
        public int proc_code { get; set; }
        public string cmd_nm { get; set; }
        public int SleepTime { get; set; }
        public string SleepUnit { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
