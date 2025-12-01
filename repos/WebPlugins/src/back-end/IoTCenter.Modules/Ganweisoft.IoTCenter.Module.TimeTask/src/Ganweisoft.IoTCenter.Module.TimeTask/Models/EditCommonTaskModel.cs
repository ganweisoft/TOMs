// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class EditCommonTaskModel
    {
        public int TableId { get; set; }

        [Required]
        public string TableName { get; set; }

        public string Comment { get; set; }

        public List<ProcTaskEqpConfig> ProcTaskEqp { get; set; } = new List<ProcTaskEqpConfig>();

        public List<ProcTaskSysConfig> ProcTaskSys { get; set; } = new List<ProcTaskSysConfig>();

    }
}