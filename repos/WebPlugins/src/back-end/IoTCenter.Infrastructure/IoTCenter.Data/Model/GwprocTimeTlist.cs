// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public class GwprocTimeTlist
    {
        [Key]
        public int TableId { get; set; }

        [Required]
        public string TableName { get; set; }
        public string Comment { get; set; }
    }
}
