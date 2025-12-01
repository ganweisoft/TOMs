// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Models
{
    public class UpdateSceneModel
    {
        public int EquipNo { get; set; }

        public int SetNo { get; set; }

        public string SetNm { get; set; }

        public List<ListValue> list { get; set; }
    }
}
