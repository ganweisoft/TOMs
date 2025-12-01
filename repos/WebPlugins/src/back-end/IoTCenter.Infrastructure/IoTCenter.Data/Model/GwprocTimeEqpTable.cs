// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public class GwprocTimeEqpTable
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeDur { get; set; }
        [Column("equip_no")]
        public int EquipNo { get; set; }
        [Column("set_no")]
        public int SetNo { get; set; }
        [Column("value")]
        public string Value { get; set; }
        [Column("processorder")]
        public string ProcessOrder { get; set; }
        [Column("equipsetnm")]
        public string EquipSetNm { get; set; }
        [Column("reserve1")]
        public string Reserve1 { get; set; }
        [Column("reserve2")]
        public string Reserve2 { get; set; }
        [Column("reserve3")]
        public string Reserve3 { get; set; }
    }
}
