// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition
{
    public class GetEquipYcYxpModel
    {
        public int[] EquipNos { get; set; }
    }

    public class GetEquipYcYxpResponse
    {
        public IEnumerable<GetEquipYcYxpResponseItem> Items { get; set; }
    }

    public class GetEquipYcYxpResponseItem
    {
        public int StaNo { get; set; }

        public int EquipNo { get; set; }

        public int YcyxNo { get; set; }

        public string YcyxName { get; set; }

        public string YcyxType { get; set; }
    }
}