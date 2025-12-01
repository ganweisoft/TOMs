// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList
{
    internal class GetYcpEquipModel
    {
        public int StaN { get; set; }
        public int EquipNo { get; set; }
        public int YcNo { get; set; }
        public string YcNm { get; set; }
        public string ProcAdvice { get; set; }
        public string RelatedPic { get; set; }
        public string RelatedVideo { get; set; }
        public string ZiChanID { get; set; }
        public string ZiChanName { get; set; }
        public string PlanNo { get; set; }
        public string State { get; set; } = "true";
        public string Value { get; set; } = "";
        public string Unit { get; set; }
    }
}
