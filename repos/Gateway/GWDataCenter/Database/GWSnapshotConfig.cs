﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWSnapshotConfig")]
    public class GWSnapshotConfigTableRow
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "快照名称")]
        public string SnapshotName { get; set; }
        [Display(Name = "最小级别")]
        public int SnapshotLevelMin { get; set; }
        [Display(Name = "最大级别")]
        public int SnapshotLevelMax { get; set; }
        [Display(Name = "最大显示条数")]
        public int MaxCount { get; set; }
        [Display(Name = "是否显示")]
        public int IsShow { get; set; }
        [Display(Name = "图标")]
        public string IconRes { get; set; }
        [Display(Name = "保留字段1")]
        public string Reserve1 { get; set; }
        [Display(Name = "保留字段2")]
        public string Reserve2 { get; set; }
        [Display(Name = "保留字段3")]
        public string Reserve3 { get; set; }
    }
}
