// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public partial class IotSetParm
    {
        public int StaN { get; set; }
        public int EquipNo { get; set; }
        public int SetNo { get; set; }
        public string SetNm { get; set; }
        public string SetType { get; set; }
        public string MainInstruction { get; set; }
        public string MinorInstruction { get; set; }
        public bool Record { get; set; }
        public string Action { get; set; }
        public string Value { get; set; }
        public bool Canexecution { get; set; }
        public string VoiceKeys { get; set; }
        public bool EnableVoice { get; set; }
        public int QrEquipNo { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }

        public IotEquip IotEquip { get; set; }

        [Column("set_code")]
        public string SetCode { get; set; }
    }
}
