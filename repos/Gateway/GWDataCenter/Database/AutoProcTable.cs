﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("AutoProc")]
    public class AutoProcTableRow
    {
        [Key]
        public int ID { get; set; }
        public int iequip_no { get; set; }
        public int iycyx_no { get; set; }
        public string iycyx_type { get; set; }
        public int delay { get; set; }
        public int oequip_no { get; set; }
        public int oset_no { get; set; }
        public string value { get; set; }
        public string ProcDesc { get; set; }
        public bool Enable { get; set; }
    }
}
