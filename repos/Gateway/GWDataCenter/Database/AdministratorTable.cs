﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("Administrator")]
    public class AdministratorTableRow
    {
        [Key]
        public string Administrator { get; set; }
        public string Telphone { get; set; }
        public string MobileTel { get; set; }
        public string EMail { get; set; }
        public int AckLevel { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
