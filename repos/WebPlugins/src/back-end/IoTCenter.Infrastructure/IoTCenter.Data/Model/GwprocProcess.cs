// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public partial class GwprocProcess
    {
        [Key]
        public int Id { get; set; }
        public int? TableId { get; set; }
        public string TaskType { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
