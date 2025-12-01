// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("administrator")]
    public class Administrator
    {
        [Key]
        [Column("administrator")]
        public string AdministratorName { get; set; }
        [Column("telphone")]
        public string Telphone { get; set; }
        [Column("mobiletel")]
        public string MobileTel { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("acklevel")]
        public int AckLevel { get; set; }
        [Column("reserve1")]
        public string Reserve1 { get; set; }
        [Column("reserve2")]
        public string Reserve2 { get; set; }
        [Column("reserve3")]
        public string Reserve3 { get; set; }
        [Column("photoimage")]
        public string PhotoImage { get; set; }
        [Column("username")]
        public string UserName { get; set; }
    }
}
