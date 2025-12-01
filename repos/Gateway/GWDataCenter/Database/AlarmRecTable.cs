﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("AlarmRec")]
    public class AlarmRecTableRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string proc_name { get; set; }
        [Required]
        public string Administrator { get; set; }
        [Required]
        [Column("event")]
        public string gwEvent { get; set; }
        [Required]
        public DateTime time { get; set; }
        public string comment { get; set; }
    }
}
