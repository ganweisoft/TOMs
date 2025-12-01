// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition
{
    public class AddIConditionItem
    {
        [Required]
        public int IequipNo { get; set; }

        public string IequipNm { get; set; }

        public IEnumerable<AddIConditionYcYxItem> IYcYxItems { get; set; }
    }

    public class AddIConditionYcYxItem
    {
        [Required]
        public int IycyxNo { get; set; }

        public string IycyxType { get; set; }

        public string IycyxValue { get; set; }

        public ConditionExpression Condition { get; set; }
    }

    public class AddConditionModel
    {
        [Required]
        public string ProcName { get; set; }

        [Required]
        public int Delay { get; set; }

        [Required]
        public IEnumerable<AddIConditionItem> IConditionItems { get; set; }

        [Required]
        public int OequipNo { get; set; }

        [Required]
        public int OsetNo { get; set; }

        public string Value { get; set; }

        public string ProcDesc { get; set; }

        public bool Enable { get; set; }
    }
}