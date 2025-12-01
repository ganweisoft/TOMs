//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;

namespace IoTCenterHost.Core.Abstraction
{
    public class YcpList
    {
        [JsonProperty(PropertyName = "sta_n")]
        public int StaN { get; set; }

        [JsonProperty(PropertyName = "equip_no")]
        public int EquipNo { get; set; }

        [JsonProperty(PropertyName = "yc_no")]
        public int YcNo { get; set; }

        [JsonProperty(PropertyName = "yc_nm")]
        public string YcNm { get; set; }

        [JsonProperty(PropertyName = "proc_advice")]
        public string ProcAdvice { get; set; }

        [JsonProperty(PropertyName = "related_pic")]
        public string RelatedPic { get; set; }

        [JsonProperty(PropertyName = "related_video")]
        public string RelatedVideo { get; set; }

        public string ZiChanID { get; set; }

        public string PlanNo { get; set; }

        public bool State { get; set; }

        public string Value { get; set; }

        public string Unit { get; set; }

    }
}
