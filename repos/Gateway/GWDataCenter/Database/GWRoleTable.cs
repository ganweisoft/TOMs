﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWRole")]
    public class GWRoleTableRow
    {
        [Key]
        public string Name { get; set; }
        public string ControlEquips { get; set; }
        public string ControlEquips_Unit { get; set; }
        public string BrowseEquips { get; set; }
        public string BrowsePages { get; set; }
        public string remark { get; set; }
        public string SpecialBrowseEquip { get; set; }
        public string SystemModule { get; set; }
    }
}
