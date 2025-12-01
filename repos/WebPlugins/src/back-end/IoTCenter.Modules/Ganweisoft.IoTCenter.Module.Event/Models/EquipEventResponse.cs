// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using System;

namespace Ganweisoft.IoTCenter.Module.Event;

public class EquipEventResponse
{
    [JsonProperty("equip_no")]
    public int EquipNo { get; set; }
    [JsonProperty("equip_nm")]
    public string EquipNm { get; set; }
    public string Event { get; set; }
    public DateTime Time { get; set; }
    public string Confirmname { get; set; }
    public DateTime? Confirmtime { get; set; }
    public string ConfirmRemark { get; set; }
    public string AlarmLevel { get; set; }
    [JsonProperty("ycyx_no")]
    public string YcyxNo { get; set; }
    [JsonProperty("ycyx_type")]
    public string YcyxType { get; set; }
    [JsonProperty("related_pic")]
    public string RelatedPic { get; set; }
    [JsonProperty("related_video")]
    public string RelatedVideo { get; set; }
    public string ZiChanID { get; set; }
    public string PlanNo { get; set; }
}
