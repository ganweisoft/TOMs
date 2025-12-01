// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class AddCycleTaskModel
    {
        [Required]
        public string TableName { get; set; }

        public string Comment { get; set; }

        public CycleTaskConfig CycleTask { get; set; }


        public List<CycleTaskContentList> CycleTaskContent { get; set; } = new List<CycleTaskContentList>();
    }
}