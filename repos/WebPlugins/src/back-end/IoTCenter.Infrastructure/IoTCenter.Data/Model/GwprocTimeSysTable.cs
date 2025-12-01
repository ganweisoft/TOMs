// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public class GwprocTimeSysTable
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("tableid")]
        public int TableId { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("timedur")]
        public DateTime TimeDur { get; set; }
        [Column("proc_code")]
        public int ProcCode { get; set; }
        [Column("cmd_nm")]
        public string CmdNm { get; set; }
        [Column("processorder")]
        public string ProcessOrder { get; set; }
        [Column("reserve1")]
        public string Reserve1 { get; set; }
        [Column("reserve2")]
        public string Reserve2 { get; set; }
        [Column("reserve3")]
        public string Reserve3 { get; set; }
    }
}
