// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipLink.Models
{
    public class EquipLinkListResponse
    {
        public int Id { get; set; }
        public int IequipNo { get; set; }
        public string IequipNm { get; set; }
        public int IycyxNo { get; set; }
        public string YcYxName { get; set; }
        public string IycyxType { get; set; }
        public int Delay { get; set; }
        public int OequipNo { get; set; }
        public string OequipNm { get; set; }
        public int OsetNo { get; set; }
        public string SetNm { get; set; }
        public string SetType { get; set; }
        public string Value { get; set; }
        public string ProcDesc { get; set; }

        public bool Enable { get; set; }
    }
}
