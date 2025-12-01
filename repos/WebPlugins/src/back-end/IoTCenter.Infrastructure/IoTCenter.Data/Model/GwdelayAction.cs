// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("gwdelayaction")]
    public class GwdelayAction
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("gw_sta_n")]
        public int GwStaN { get; set; }
        [Column("gw_equip_no")]
        public int GwEquipNo { get; set; }
        [Column("gw_set_no")]
        public int GwSetNo { get; set; }
        [Column("gw_value")]
        public string GwValue { get; set; }
        [Column("gw_adddatetime")]
        public string GwAddDateTime { get; set; }
        [Column("gw_usernm")]
        public string GwUserNm { get; set; }
        [Column("gw_delaynum")]
        public int GwDelayNum { get; set; }
        [Column("gw_state")]
        public int GwState { get; set; }
        [Column("gw_source")]
        public int GwSource { get; set; }
        [Column("reserve1")]
        public string Reserve1 { get; set; }
        [Column("reserve2")]
        public string Reserve2 { get; set; }
        [Column("reserve3")]
        public string Reserve3 { get; set; }
    }
}
