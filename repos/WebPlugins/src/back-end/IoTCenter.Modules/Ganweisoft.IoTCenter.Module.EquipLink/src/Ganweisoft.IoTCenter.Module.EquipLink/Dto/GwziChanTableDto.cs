// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    [Table("gwzichantable")]
    public class GwziChanTableDto
    {
        [Key]
        public string ZiChanId { get; set; }
        public string ZiChanName { get; set; }
        public string ZiChanType { get; set; }
        public string ZiChanImage { get; set; }
        public string ChangJia { get; set; }
        public string LianxiRen { get; set; }
        public string LianxiTel { get; set; }
        public string LianxiMail { get; set; }
        public DateTime GouMaiDate { get; set; }
        public string ZiChanSite { get; set; }
        public DateTime WeiHuDate { get; set; }
        public int WeiHuCycle { get; set; }
        public DateTime BaoXiuQiXian { get; set; }
        public string LastEditMan { get; set; }
        public DateTime LastEditDate { get; set; }
        public DateTime AnZhuangDate { get; set; }
        public string RelatedPic { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
