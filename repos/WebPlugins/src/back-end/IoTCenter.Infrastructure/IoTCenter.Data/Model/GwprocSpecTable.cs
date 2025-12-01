// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public class GwprocSpecTable
    {
        [Key]
        public int Id { get; set; }
        public string DateName { get; set; }

        public string Color { get; set; }
        [Required]
        public DateTime BeginDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string TableId { get; set; }
    }
}
