// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public class Equip
    {
        [Required]
        public int StaN { get; set; }

        [Required]
        [Description("设备号")]
        public int EquipNo { get; set; }
        [Description("设备名")]
        public string EquipNm { get; set; }
        [Description("设备属性")]
        public string EquipDetail { get; set; }
        [Description("通讯刷新周期")]
        public int AccCyc { get; set; }
        [Description("关联界面")]
        public string RelatedPic { get; set; }
        [Description("通故障处理意见")]
        public string ProcAdvice { get; set; }
        [Description("故障提示")]
        public string OutOfContact { get; set; }
        [Description("故障恢复提示")]
        public string Contacted { get; set; }
        [Description("报警声音文件")]
        public string EventWav { get; set; }
        [Description("驱动文件")]
        public string CommunicationDrv { get; set; }
        [Description("通讯端口")]
        public string LocalAddr { get; set; }
        [Description("设备地址")]
        public string EquipAddr { get; set; }
        [Description("通讯参数")]
        public string CommunicationParam { get; set; }
        [Description("通讯时间参数")]
        public string CommunicationTimeParam { get; set; }
        [Description("模板设备号")]
        public int RawEquipNo { get; set; }
        [Description("附表名称")]
        public string Tabname { get; set; }
        [Description("报警方式")]
        public int AlarmScheme { get; set; }
        [Description("属性")]
        public int Attrib { get; set; }
        public string StaIp { get; set; }
        [Description("报警升级周期（分钟）")]
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
        public string Backup { get; set; }
        public bool? IsDisable { get; set; }
        public List<Ycp> Ycps { get; set; } = new List<Ycp>();
        public List<Yxp> Yxps { get; set; } = new List<Yxp>();
        public List<SetParm> SetParms { get; set; } = new List<SetParm>();

        public Equip Clone()
        {
            return (Equip) MemberwiseClone();
        }
    }
}