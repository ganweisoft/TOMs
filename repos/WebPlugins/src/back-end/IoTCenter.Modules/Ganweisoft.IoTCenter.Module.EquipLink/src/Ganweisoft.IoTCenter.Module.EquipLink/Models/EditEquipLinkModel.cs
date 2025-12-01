// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models
{
    public class EditEquipLinkModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int IequipNo { get; set; }

        [Required]
        public int? IycyxNo { get; set; }

        [Required]
        public string IycyxType { get; set; }

        [Required]
        public int Delay { get; set; }

        [Required]
        public int OequipNo { get; set; }

        [Required]
        public int OsetNo { get; set; }

        public string Value { get; set; }

        public string ProcDesc { get; set; }

        public bool Enable { get; set; }
    }
}
