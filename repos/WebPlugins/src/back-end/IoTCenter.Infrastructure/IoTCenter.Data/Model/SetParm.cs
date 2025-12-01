// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public class SetParm
    {
        [Required]
        public int StaN { get; set; }
        [Required]
        [Description("设备号")]
        public int EquipNo { get; set; }
        [Required]
        [Description("设置号")]
        public int SetNo { get; set; }
        [Description("设置名称")]
        public string SetNm { get; set; }
        [Required]
        [Description("设置类型")]
        public string SetType { get; set; }
        [Description("操作命令")]
        public string MainInstruction { get; set; }
        [Description("操作参数")]
        public string MinorInstruction { get; set; }
        [Description("记录")]
        public bool Record { get; set; }
        [Description("动作")]
        public string Action { get; set; }
        [Description("值")]
        public string Value { get; set; }
        [Description("是否可以执行")]
        public bool Canexecution { get; set; }
        public string VoiceKeys { get; set; }
        public bool EnableVoice { get; set; }
        public int QrEquipNo { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }

        public Equip Equip { get; set; }

        [Column("set_code")]
        public string SetCode { get; set; }
    }
}
