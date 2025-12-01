// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class EditTimeTaskModel
    {
        public int TableId { get; set; }

        [Required]
        public string TableName { get; set; }

        public string TableType { get; set; }

        public string Comment { get; set; }

        public List<ProcTaskEqpConfig> ProcTaskEqp { get; set; }

        public List<ProcTaskSysConfig> ProcTaskSys { get; set; }

        public List<ProcTaskProcessTime> ProcTaskProcessTimes { get; set; }

        public List<ProcCycleProcessTime> ProcCycleProcessTimes { get; set; }

        public CycleTaskConfig CycleTask { get; set; }

        public List<CycleTaskContentList> CycleTaskContent { get; set; }

        public List<ProcTaskProcess> ProcTaskProcess { get; set; }
    }
}
