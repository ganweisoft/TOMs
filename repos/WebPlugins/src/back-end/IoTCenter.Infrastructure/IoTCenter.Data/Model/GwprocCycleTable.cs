// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public class GwprocCycleTable
    {
        public int TableId { get; set; }
        public int DoOrder { get; set; }
        public string Type { get; set; }
        [Column("equip_no")]
        public int EquipNo { get; set; }
        [Column("set_no")]
        public int SetNo { get; set; }
        public string Value { get; set; }
        [Column("proc_code")]
        public int ProcCode { get; set; }
        [Column("cmd_nm")]
        public string CmdNm { get; set; }
        public int SleepTime { get; set; }
        public string SleepUnit { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public string ProcessOrder { get; set; }
        public string EquipSetNm { get; set; }
    }
}
