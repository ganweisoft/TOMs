// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public class YcYxEvt
    {
        public int Id { get; set; }
        public int StaN { get; set; }

        [Required]
        public int EquipNo { get; set; }

        [Required]
        public int YcyxNo { get; set; }


        [Required]
        public string YcyxType { get; set; }

        [Required]
        public string Event { get; set; }

        [Required]
        public DateTime Time { get; set; }
        public string ProcRec { get; set; }
        public string Confirmname { get; set; }
        public DateTime? Confirmtime { get; set; }
        public bool WuBao { get; set; }
        public int Alarmlevel { get; set; }
        public string Confirmremark { get; set; }
        public string Guid { get; set; }
        public int Snapshotlevel { get; set; }

        public int Alarmstate { get; set; }
    }
}
