// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("iotaccountpasswordrule")]
    public class IoTAccountPasswordRule
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("json")]
        public string JSON { get; set; }
    }
}
