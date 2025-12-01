// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition
{
    public class EditConditionModel
    {
        [Required]
        public int Id { get; set; }

        public int ConditionId { get; set; }

        public string ProcName { get; set; }

        [Required]
        public int Delay { get; set; }

        public IEnumerable<AddIConditionItem> IConditionItems { get; set; } = Enumerable.Empty<AddIConditionItem>();

        [Required]
        public int OequipNo { get; set; }

        [Required]
        public int OsetNo { get; set; }

        public string Value { get; set; }

        public string ProcDesc { get; set; }

        public bool Enable { get; set; }
    }
}