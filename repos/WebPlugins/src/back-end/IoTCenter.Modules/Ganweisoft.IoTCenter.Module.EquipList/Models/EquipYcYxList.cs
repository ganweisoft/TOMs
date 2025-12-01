// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganweisoft.IoTCenter.Module.EquipList
{
    public class EquipYcYxList<T>
    {
        public int Count { get; set; }
        public int TotalEquipCount => ListNos.Distinct().Count();
        public List<T> List { get; set; } = new List<T>();
        public int[] ListNos { get; set; } = Array.Empty<int>();
    }
}
