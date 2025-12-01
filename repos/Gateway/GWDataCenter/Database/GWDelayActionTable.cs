﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWDelayAction")]
    public class GWDelayActionTableRow
    {
        [Key]
        public int ID { get; set; }
        public int GW_Sta_n { get; set; }
        public int GW_Equip_no { get; set; }
        public int GW_Set_no { get; set; }
        public string GW_Value { get; set; }
        public DateTime GW_AddDateTime { get; set; }
        public string GW_UserNm { get; set; }
        public int GW_DelayNum { get; set; }
        public int GW_State { get; set; }
        public int GW_Source { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
