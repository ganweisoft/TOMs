﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("GWUser")]
    public class GWUserTableRow
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string HomePages { get; set; }
        public string AutoInspectionPages { get; set; }
        public string Remark { get; set; }
        public string ControlLevel { get; set; }
    }
}
