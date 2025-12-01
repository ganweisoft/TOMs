// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTCenter.Data.Model
{
    public partial class IoTdevice
    {
        [Key]
        public string DeviceId { get; set; }
        public string GatewayId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int EquipNo { get; set; }
        public string DeviceType { get; set; }
        public string DeviceModel { get; set; }
        public string SwVersion { get; set; }
        public string FwVersion { get; set; }
        public string HwVersion { get; set; }
        public string ProtocolType { get; set; }
        public string SigVersion { get; set; }
        public string SerialNumber { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Height { get; set; }
        public string Mac { get; set; }
        public string ManufacturerName { get; set; }
        public string AreaName { get; set; }
        public string BuildName { get; set; }
        public string UnitName { get; set; }
        public string SystemName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string SceneParam { get; set; }
        public string VideoParam { get; set; }
        public string OtherData { get; set; }
    }
}
