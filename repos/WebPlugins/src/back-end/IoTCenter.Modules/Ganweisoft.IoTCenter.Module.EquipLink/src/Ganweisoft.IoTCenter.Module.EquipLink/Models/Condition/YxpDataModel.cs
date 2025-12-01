// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition
{
    public class YxpDataModel
    {
        public long StaNo { get; set; } = 1;

        [Required]
        public long EquipNo { get; set; }

        [Required]
        public long YxNo { get; set; }

        public string YxNm { get; set; }

        public string ProcAdviceR { get; set; }

        public string ProcAdviceD { get; set; }

        [Required]
        public int LevelR { get; set; }

        [Required]
        public int LevelD { get; set; }

        public string Evt01 { get; set; }

        public string Evt10 { get; set; }

        public string MainInstruction { get; set; }

        public string MinorInstruction { get; set; }

        public string SafeBgn { get; set; }

        public string SafeEnd { get; set; }

        public long AlarmAcceptableTime { get; set; }

        public long RestoreAcceptableTime { get; set; }

        public int AlarmRepeatTime { get; set; }

        public string WaveFile { get; set; }

        public string RelatedPic { get; set; }

        public int AlarmScheme { get; set; }

        public bool Inversion { get; set; }

        public int Initval { get; set; }

        public int ValTrait { get; set; }

        public string AlarmShield { get; set; }

        public int AlarmRiseCycle { get; set; }

        public string RelatedVideo { get; set; }

        public string ZiChanId { get; set; }

        public string PlanNo { get; set; }

        public string SafeTime { get; set; }

        public bool CurveRcd { get; set; }

        public string YxCode { get; set; }

        public string DataType { get; set; }

        public string Expression { get; set; }

        public YxpDataModel(int equipNo, string expression)
        {
            this.EquipNo = equipNo;
            this.StaNo = 1;
            this.ProcAdviceR = "请处理";
            this.ProcAdviceD = "请处理";
            this.LevelR = 2;
            this.LevelD = 3;
            this.Evt01 = "正常";
            this.Evt10 = "报警";
            this.MainInstruction = "0";
            this.MinorInstruction = "0";
            this.AlarmAcceptableTime = 0;
            this.RestoreAcceptableTime = 0;
            this.AlarmRepeatTime = 0;
            this.AlarmScheme = 1;
            this.Inversion = false;
            this.Initval = 0;
            this.ValTrait = 0;
            this.AlarmShield = string.Empty;
            this.AlarmRiseCycle = 0;
            this.RelatedVideo = string.Empty;
            this.ZiChanId = string.Empty;
            this.PlanNo = string.Empty;
            this.SafeTime = string.Empty;
            this.CurveRcd = false;
            this.DataType = string.Empty;
            this.Expression = expression;
        }
    }
}