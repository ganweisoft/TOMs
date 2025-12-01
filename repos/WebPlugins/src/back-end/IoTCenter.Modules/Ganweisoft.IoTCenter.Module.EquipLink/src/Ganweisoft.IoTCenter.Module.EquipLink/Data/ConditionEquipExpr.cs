// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Data
{
    [Table("ConditionEquipExpr")]
    public class ConditionEquipExpr
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("condition_no")]
        public int ConditionId { get; set; }

        [Column("iequip_no")]
        public int IequipNo { get; set; }

        [Column("iequip_nm")]
        public string IequipNm { get; set; }

        [Column("iycyx_no")]
        public int IycyxNo { get; set; }

        [Column("iycyx_type")]
        public string IycyxType { get; set; }

        [Column("iycyx_value")]
        public string IycyxValue { get; set; }

        [Column("condition_expr")]
        public ConditionExpression Condition { get; set; }

        public ConditionAutoProc ConditionAutoProc { get; set; }
    }

    public enum ConditionExpression
    {
        Equal = 0x00,
        NotEqual = 0x01,
        GreaterThan = 0x02,
        GreaterThanOrEqual = 0x04,
        LessThan = 0x08,
        LessThanOrEqual = 0x10,
    }
}