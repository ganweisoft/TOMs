//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;

namespace IoTCenterHost.Core.Abstraction
{
    public class YxpList
    {
        [JsonProperty(PropertyName = "sta_n")]
        public int StaN { get; set; }

        [JsonProperty(PropertyName = "equip_no")]
        public int EquipNo { get; set; }

        [JsonProperty(PropertyName = "yx_no")]
        public int YxNo { get; set; }

        [JsonProperty(PropertyName = "yx_nm")]
        public string YxNm { get; set; }

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

    }
}
