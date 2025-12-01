// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public class GwprocCycleTlist
    {
        [Key]
        public int TableId { get; set; }
        public string TableName { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool ZhenDianDo { get; set; }
        public bool ZhidingDo { get; set; }
        public bool CycleMustFinish { get; set; }
        public DateTime ZhidingTime { get; set; }
        public int MaxCycleNum { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
