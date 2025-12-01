// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models
{
    public class TEquipAndOEquiepListResponse
    {
        public IEnumerable<TOEquip> IList { get; set; }
        public IEnumerable<TOEquip> OList { get; set; }
    }

    public class TOEquip
    {
        public int EquipNo { get; set; }
        public string EquipNm { get; set; }
        public string EquipType { get; set; }
    }
}
