// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{

    public class TimeTaskData
    {
        public string TableId { get; set; }

        public string TableName { get; set; }

        public string TableType { get; set; }

        public string Comment { get; set; }

        public List<ProcTaskEqpConfig> procTaskEqp { get; set; }

        public List<ProcTaskSysConfig> procTaskSys { get; set; }

        public List<ProcTaskProcessTime> procTaskProcessTimes { get; set; }

        public List<ProcCycleProcessTime> procCycleProcessTimes { get; set; }

        public CycleTaskConfig cycleTask { get; set; }

        public List<CycleTaskContentList> cycleTaskContent { get; set; }

        public List<ProcTaskProcess> procTaskProcess { get; set; }
    }
}
