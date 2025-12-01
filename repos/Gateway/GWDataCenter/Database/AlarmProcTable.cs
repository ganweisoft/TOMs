﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("AlarmProc")]
    public class AlarmProcTableRow
    {
        [Key]
        public int Proc_Code { get; set; }
        public string Proc_Module { get; set; }
        public string Proc_name { get; set; }
        public string Proc_parm { get; set; }
        public string Comment { get; set; }
    }
}
