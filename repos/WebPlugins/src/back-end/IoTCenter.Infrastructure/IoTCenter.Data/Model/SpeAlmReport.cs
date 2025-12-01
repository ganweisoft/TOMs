// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("spealmreport")]
    public class SpeAlmReport
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("sta_n")]
        public int StaN { get; set; }
        [Column("group_no")]
        public int GroupNo { get; set; }
        [Column("administrator")]
        public string Administrator { get; set; }
        [Column("begin_time")]
        public DateTime BeginTime { get; set; }
        [Column("end_time")]
        public DateTime EndTime { get; set; }
        [Column("remark")]
        public string Remark { get; set; }
        [Column("color")]
        public string Color { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
