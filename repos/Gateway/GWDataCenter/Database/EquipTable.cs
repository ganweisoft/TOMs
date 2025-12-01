﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GWDataCenter.Database
{
    [Table("Equip")]
    public class EquipTableRow
    {
        [Required]
        [Display(Name = "站点号")]
        public int sta_n { get; set; }
        [Required]
        [Display(Name = "设备号")]
        public int equip_no { get; set; }
        [Required]
        [Display(Name = "设备名称")]
        public string equip_nm { get; set; }
        [Display(Name = "设备细节")]
        public string equip_detail { get; set; }
        [Display(Name = "处理周期")]
        public int acc_cyc { get; set; }
        [Display(Name = "关联页面")]
        public string related_pic { get; set; }
        [Display(Name = "处理意见")]
        public string proc_advice { get; set; }
        [Display(Name = "通讯失败提示")]
        public string out_of_contact { get; set; }
        [Display(Name = "通讯恢复提示")]
        public string contacted { get; set; }
        [Display(Name = "声音文件")]
        public string event_wav { get; set; }
        [Display(Name = "驱动模块")]
        public string communication_drv { get; set; }
        [Display(Name = "通讯端口")]
        public string local_addr { get; set; }
        [Display(Name = "设备地址")]
        public string equip_addr { get; set; }
        [Display(Name = "通讯参数")]
        public string communication_param { get; set; }
        [Display(Name = "时间参数")]
        public string communication_time_param { get; set; }
        [Display(Name = "模板设备")]
        public int raw_equip_no { get; set; }
        [Display(Name = "关联表")]
        public string tabname { get; set; }
        [Display(Name = "报警方式")]
        public int alarm_scheme { get; set; }
        [Display(Name = "属性")]
        public int attrib { get; set; }
        [Display(Name = "站点IP")]
        public string sta_IP { get; set; }
        [Display(Name = "报警升级周期")]
        public int? AlarmRiseCycle { get; set; }
        [Display(Name = "保留字段1")]
        public string Reserve1 { get; set; }
        [Display(Name = "保留字段2")]
        public string Reserve2 { get; set; }
        [Display(Name = "保留字段3")]
        public string Reserve3 { get; set; }
        [Display(Name = "关联视频")]
        public string related_video { get; set; }
        [Display(Name = "关联资产")]
        public string ZiChanID { get; set; }
        [Display(Name = "关联预案")]
        public string PlanNo { get; set; }
        [Display(Name = "安全时段")]
        public string SafeTime { get; set; }
        [Display(Name = "备份")]
        public string backup { get; set; }
    }
}
