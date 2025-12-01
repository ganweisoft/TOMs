// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Data
{
    [Table("ConditionAutoProc")]
    public class ConditionAutoProc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("proc_name")]
        public string ProcName { get; set; }

        [Column("relate_autoproc")]
        public int RelateAutoProc { get; set; }

        [Column("relate_yxNo")]
        public int RelateYxNo { get; set; }

        [Column("delay")]
        public int Delay { get; set; }

        [Column("oequip_no")]
        public int OequipNo { get; set; }

        [Column("oset_no")]
        public int OsetNo { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("proc_desc")]
        public string ProcDesc { get; set; }

        [Column("enable")]
        public bool Enable { get; set; }

        [Column("modifier")]
        public string Modifier { get; set; }

        [Column("modify_date")]
        public DateTime ModifyDate { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        public List<ConditionEquipExpr> ConditionExpressions { get; set; } = new();
    }
}