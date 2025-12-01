// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class CommonTaskData
    {
        public int TableId { get; set; }

        public string TableName { get; set; }


        public string Comment { get; set; }

        public List<ProcTaskEqpConfig> procTaskEqp { get; set; } = new List<ProcTaskEqpConfig>();

        public List<ProcTaskSysConfig> procTaskSys { get; set; } = new List<ProcTaskSysConfig>();
    }



    public class CycleTaskData
    {
        public int TableId { get; set; }
        [Required]
        public string TableName { get; set; }
        public string Comment { get; set; }
        public CycleTaskConfig cycleTask { get; set; }
        public List<CycleTaskContentList> cycleTaskContent { get; set; }
    }
}
