// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models
{
    public class GetEquipLinkModel : QueryRequest
    {

        public string EquipName { get; set; }

        public int[] IequipNos { get; set; }

        public string IycyxTypes { get; set; }

        public long? MinDelay { get; set; }

        public long? MaxDelay { get; set; }

        public string OequipNos { get; set; }

        public string OsetNos { get; set; }

        public List<EquipSetList> EquipSetLists { get; set; } = new List<EquipSetList>();

    }

    public class EquipSetList
    {
        public int EquipNo { get; set; }
        public List<int> SetNos { get; set; } = new List<int>();
    }
}
