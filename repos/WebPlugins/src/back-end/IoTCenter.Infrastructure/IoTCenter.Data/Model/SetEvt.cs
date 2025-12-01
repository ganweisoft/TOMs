// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public class SetEvt
    {
        public int Id { get; set; }
        public int StaN { get; set; }
        public int SetNo { get; set; }
        [Required]
        public int EquipNo { get; set; }
        public string Gwevent { get; set; }
        public DateTime Gwtime { get; set; }
        public string Gwoperator { get; set; }
        public string Confirmremark { get; set; }
        public DateTime? ConfirmTime { get; set; }

        public string ConfirmName { get; set; }

        public string GUID { get; set; }

        public string Gwsource { get; set; }
    }
}
