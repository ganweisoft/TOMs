// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    [Table("alarmproc")]
    public partial class AlarmProc
    {
        [Key]
        [Column("proc_code")]
        public int ProcCode { get; set; }
        [Column("proc_module")]
        public string ProcModule { get; set; }
        [Column("proc_name")]
        public string ProcName { get; set; }
        [Column("proc_parm")]
        public string ProcParm { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
        [Column("reserve1")]
        public string Reserve1 { get; set; }
        [Column("reserve2")]
        public string Reserve2 { get; set; }
        [Column("reserve3")]
        public string Reserve3 { get; set; }
    }
}
