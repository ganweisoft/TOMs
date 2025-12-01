// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public class Yxp
    {
        [Required]
        public int StaN { get; set; }
        [Required]
        [Description("设备号")]
        public int EquipNo { get; set; }
        [Required]
        [Description("状态量编号")]
        public int YxNo { get; set; }
        [Required]
        [Description("状态量名称")]
        public string YxNm { get; set; }
        [Description("处理意见0-1")]
        public string ProcAdviceR { get; set; }
        [Description("处理意见1-0")]
        public string ProcAdviceD { get; set; }
        [Description("0-1级别")]
        public int LevelR { get; set; }
        [Description("1-0级别")]
        public int LevelD { get; set; }
        [Description("0-1事件")]
        public string Evt01 { get; set; }
        [Description("1-0事件")]
        public string Evt10 { get; set; }
        [Description("操作命令")]
        public string MainInstruction { get; set; }
        [Description("操作参数")]
        public string MinorInstruction { get; set; }
        [Description("越线滞纳时间（秒）")]
        public int AlarmAcceptableTime { get; set; }
        [Description("恢复滞纳时间（秒）")]
        public int RestoreAcceptableTime { get; set; }
        [Description("重复报警时间（分钟）")]
        public int AlarmRepeatTime { get; set; }
        [Description("声音文件")]
        public string WaveFile { get; set; }
        [Description("关联页面")]
        public string RelatedPic { get; set; }
        [Description("报警方式")]
        public int AlarmScheme { get; set; }
        [Description("取反")]
        public bool Inversion { get; set; }
        [Description("初始状态")]
        public int Initval { get; set; }
        [Description("属性值")]
        public int ValTrait { get; set; }
        [Description("报警屏蔽")]
        public string AlarmShield { get; set; }
        [Description("报警升级周期（分钟）")]
        public int? AlarmRiseCycle { get; set; }
        [Description("关联视频")]
        public string RelatedVideo { get; set; }
        [Description("资产编号")]
        public string ZiChanId { get; set; }
        [Description("预案号")]
        public string PlanNo { get; set; }
        [Description("安全时段")]
        public string SafeTime { get; set; }
        public bool CurveRcd { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }

        public DateTime? SafeBgn { get; set; }
        public DateTime? SafeEnd { get; set; }
        public string GWValue { get; set; }
        public DateTime? GWTime { get; set; }
        public string DataType { get; set; }
        public Equip Equip { get; set; }

        [Column("yx_code")]
        public string YxCode { get; set; }
    }
}
