// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public class Ycp
    {
        [Required]
        public int StaN { get; set; }
        [Description("设备号")]
        [Required]
        public int EquipNo { get; set; }
        [Required]
        [Description("模拟量编号")]
        public int YcNo { get; set; }
        [Required]
        [Description("模拟量名称")]
        public string YcNm { get; set; }
        [Description("比例变换")]
        public bool? Mapping { get; set; }
        [Description("实测最小值")]
        public double YcMin { get; set; }
        [Description("实测最大值")]
        public double YcMax { get; set; }
        [Description("最小值")]
        public double PhysicMin { get; set; }
        [Description("最大值")]
        public double PhysicMax { get; set; }
        [Description("下限值")]
        public double ValMin { get; set; }
        [Description("回复下限值")]
        public double RestoreMin { get; set; }
        [Description("回复上限值")]
        public double RestoreMax { get; set; }
        [Description("上限值")]
        public double ValMax { get; set; }
        [Description("属性值")]
        public int ValTrait { get; set; }
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
        [Description("处理意见")]
        public string ProcAdvice { get; set; }
        [Description("报警级别")]
        public int LvlLevel { get; set; }
        [Description("越下限事件")]
        public string OutminEvt { get; set; }
        [Description("越上限事件")]
        public string OutmaxEvt { get; set; }
        [Description("声音文件")]
        public string WaveFile { get; set; }
        [Description("关联页面")]
        public string RelatedPic { get; set; }
        [Description("报警方式")]
        public int AlarmScheme { get; set; }
        [Description("曲线记录")]
        public bool CurveRcd { get; set; }
        [Description("曲线记录阈值")]
        public double? CurveLimit { get; set; }
        [Description("报警屏蔽")]
        public string AlarmShield { get; set; }
        [Description("单位")]
        public string Unit { get; set; }
        [Description("报警升级周期")]
        public int? AlarmRiseCycle { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        [Description("关联视频")]
        public string RelatedVideo { get; set; }
        [Description("资产编号")]
        public string ZiChanId { get; set; }
        [Description("预案号")]
        public string PlanNo { get; set; }
        [Description("安全时段")]
        public string SafeTime { get; set; }

        public DateTime? SafeBgn { get; set; }
        public DateTime? SafeEnd { get; set; }

        public string GWValue { get; set; }
        public DateTime? GWTime { get; set; }

        public string DataType { get; set; }

        public Equip Equip { get; set; }

        [Column("yc_code")]
        public string YcCode { get; set; }
    }
}
