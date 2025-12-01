// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models
{
    public class EqulpLinkSence
    {
        [JsonProperty(PropertyName = "sta_n")]
        public int StaNo { get; set; }
        [JsonProperty(PropertyName = "equip_no")]
        public int EquipNo { get; set; }
        [JsonProperty(PropertyName = "equip_nm")]
        public string EquipNm { get; set; }
        [JsonProperty(PropertyName = "set_no")]
        public int SetNo { get; set; }
        [JsonProperty(PropertyName = "set_nm")]
        public string SetNm { get; set; }
        [JsonProperty(PropertyName = "set_type")]
        public string SetType { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        public List<ListValue> List { get; set; }
    }

    public class ListValue
    {
        public string SceneType { get; set; }
        public string EquipNo { get; set; }
        public string EquipNm { get; set; }
        public string SetNo { get; set; }
        public string SetNm { get; set; }
        public string SetType { get; set; }
        public string Value { get; set; }
        public string TimeValue { get; set; }
    }
}
