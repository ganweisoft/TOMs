// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("alarmrec")]
    public partial class AlarmRec
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("proc_name")]
        public string ProcName { get; set; }
        [Column("administrator")]
        public string Administrator { get; set; }
        [Column("event")]
        public string Event { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
    }
}
