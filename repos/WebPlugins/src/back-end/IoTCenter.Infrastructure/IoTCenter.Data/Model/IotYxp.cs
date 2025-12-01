// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public partial class IotYxp
    {
        public int StaN { get; set; }
        public int EquipNo { get; set; }
        public int YxNo { get; set; }
        public string YxNm { get; set; }
        public string ProcAdviceR { get; set; }
        public string ProcAdviceD { get; set; }
        public int LevelR { get; set; }
        public int LevelD { get; set; }
        public string Evt01 { get; set; }
        public string Evt10 { get; set; }
        public string MainInstruction { get; set; }
        public string MinorInstruction { get; set; }
        public DateTime? SafeBgn { get; set; }
        public DateTime? SafeEnd { get; set; }
        public int AlarmAcceptableTime { get; set; }
        public int RestoreAcceptableTime { get; set; }
        public int AlarmRepeatTime { get; set; }
        public string WaveFile { get; set; }
        public string RelatedPic { get; set; }
        public int AlarmScheme { get; set; }
        public bool Inversion { get; set; }
        public bool CurveRcd { get; set; }
        public int Initval { get; set; }
        public int ValTrait { get; set; }
        public string AlarmShield { get; set; }
        public int? AlarmRiseCycle { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public string RelatedVideo { get; set; }
        public string ZiChanId { get; set; }
        public string PlanNo { get; set; }
        public string SafeTime { get; set; }

        public IotEquip IotEquip { get; set; }

        [Column("yx_code")]
        public string YxCode { get; set; }

        public string DataType { get; set; }
    }
}
