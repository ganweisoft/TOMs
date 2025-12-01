// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("almreport")]
    public class AlmReport
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("sta_n")]
        public int? StaN { get; set; }
        [ForeignKey("equipgroup")]
        [Column("group_no")]
        public int GroupNo { get; set; }
        [Column("administrator")]
        public string Administrator { get; set; }
    }
}
