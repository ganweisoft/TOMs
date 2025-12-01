// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class AddTimeTaskModel
    {
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

    public class ProcTaskEqpConfig
    {
        [DataType(DataType.DateTime)]
        public string Time { get; set; }

        [DataType(DataType.DateTime)]
        public string TimeDur { get; set; }

        public int EquipNo { get; set; }

        public int SetNo { get; set; }

        public string Value { get; set; }

        public string EquipSetNm { get; set; }

        public string ProcessOrder { get; set; }

        public int? Id { get; set; }
        public bool IsCanRun { get; set; } = true;

        public bool NoEdit { get; set; }
    }

    public class ProcTaskSysConfig
    {
        [DataType(DataType.DateTime)]
        public string Time { get; set; }

        [DataType(DataType.Time)]
        public string TimeDur { get; set; }

        public int ProcCode { get; set; }

        public string CmdNm { get; set; }

        public string ProcessOrder { get; set; }

        public int? Id { get; set; }
        public bool IsCanRun { get; set; } = true;
    }

    public class CycleTaskConfig
    {

        [DataType(DataType.DateTime)]
        public string BeginTime { get; set; }

        [DataType(DataType.DateTime)]
        public string EndTime { get; set; }

        public bool ZhenDianDo { get; set; }

        public bool ZhidingDo { get; set; }

        public bool CycleMustFinish { get; set; }

        [DataType(DataType.DateTime)]

        public string ZhidingTime { get; set; }

        public int MaxCycleNum { get; set; }
    }

    public class CycleTaskContentList
    {

        public int DoOrder { get; set; }

        public string Type { get; set; }

        public int EquipNo { get; set; }

        public int SetNo { get; set; }

        public string Value { get; set; }

        public string EquipSetNm { get; set; }

        public int ProcCode { get; set; }

        public string CmdNm { get; set; }

        public int SleepTime { get; set; }

        public string SleepUnit { get; set; }

        public string ProcessOrder { get; set; }
    }

    public class ProcTaskProcess
    {
        public int? TableId { get; set; }

        [SpecifiedString("C", "T")]
        public string TaskType { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }

    public class ProcTaskProcessTime
    {
        public string TableId { get; set; }

        public string ControlType { get; set; }

        [SpecifiedString("Time", "TimeDur")]
        public string TimeType { get; set; }

        [DataType(DataType.Date)]
        public string Time { get; set; }

        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "输入参数有误")]
        public string ProcessOrder { get; set; }
    }

    public class ProcCycleProcessTime
    {
        public string TableId { get; set; }

        public int? Time { get; set; }

        public string ProcessOrder { get; set; }
    }
}
