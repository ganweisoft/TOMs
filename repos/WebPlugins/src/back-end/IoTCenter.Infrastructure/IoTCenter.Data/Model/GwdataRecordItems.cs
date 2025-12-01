// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("gwdatarecorditems")]
    public class GwdataRecordItems
    {
        [Column("equip_no")]
        public int EquipNo { get; set; }
        [Column("data_type")]
        public string DataType { get; set; }
        [Column("ycyx_no")]
        public int YcyxNo { get; set; }
        [Column("data_name")]
        public string DataName { get; set; }
        [Column("rulename")]
        public string RuleName { get; set; }
    }
}
