// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition
{
    public class GetConditionModel : QueryRequest
    {
        public string ProcName { get; set; }

        public int ConditionId { get; set; }

        public int AutoProcId { get; set; }
    }

    public class GetConditionResponse
    {
        public int Id { get; set; }

        public string ProcName { get; set; }

        public int Delay { get; set; }

        public IEnumerable<AddIConditionItem> IConditionItems { get; set; }

        public int OequipNo { get; set; }

        public int OsetNo { get; set; }

        public string Value { get; set; }

        public string ProcDesc { get; set; }

        public bool Enable { get; set; }
    }
}