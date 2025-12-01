// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public  class SysEvt
    {
        public int Id { get; set; }
        public int StaN { get; set; }

        [Required]
        public string Event { get; set; }
        public DateTime Time { get; set; }
        public string Confirmname { get; set; }
        public DateTime Confirmtime { get; set; }
        public string Confirmremark { get; set; }
        public string Guid { get; set; }
    }
}
