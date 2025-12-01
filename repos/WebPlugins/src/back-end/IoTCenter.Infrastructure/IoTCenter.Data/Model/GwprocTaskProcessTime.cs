// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public partial class GwprocTaskProcessTime
    {
        [Key]
        public int Id { get; set; }
        public int? TableId { get; set; }
        public string ControlType { get; set; }
        public string TimeType { get; set; }
        public DateTime Time { get; set; }
        public string ProcessOrder { get; set; }
    }
}
