// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public partial class IotEquip
    {
        public int StaN { get; set; }
        public int EquipNo { get; set; }
        public string EquipNm { get; set; }
        public string EquipDetail { get; set; }
        public int AccCyc { get; set; }
        public string RelatedPic { get; set; }
        public string ProcAdvice { get; set; }
        public string OutOfContact { get; set; }
        public string Contacted { get; set; }
        public string EventWav { get; set; }
        public string CommunicationDrv { get; set; }
        public string LocalAddr { get; set; }
        public string EquipAddr { get; set; }
        public string CommunicationParam { get; set; }
        public string CommunicationTimeParam { get; set; }
        public int RawEquipNo { get; set; }
        public string Tabname { get; set; }
        public int AlarmScheme { get; set; }
        public int Attrib { get; set; }
        public string StaIp { get; set; }
        public int? AlarmRiseCycle { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public string RelatedVideo { get; set; }
        public string ZiChanId { get; set; }
        public string PlanNo { get; set; }
        public string SafeTime { get; set; }
        public string Backup { get; set; }
        public string ThingModelJson { get; set; }

        public List<IotYcp> IotYcps { get; set; } = new List<IotYcp>();
        public List<IotYxp> IotYxps { get; set; } = new List<IotYxp>();
        public List<IotSetParm> IotSetParms { get; set; } = new List<IotSetParm>();

        public int? EquipConnType { get; set; }
    }

    public enum ProductConnType
    {
        直连设备 = 0,
        子系统 = 1,
        虚拟设备 = 2
    }
}
