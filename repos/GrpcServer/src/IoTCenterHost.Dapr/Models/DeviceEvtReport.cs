//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Dapr.Models
{
    public class DeviceEvtReport
    {
        public string Time { get; set; }
        public string Flow { get; set; }
        public List<EquipEvent> EventItems { get; set; } = new ();

        public class EquipEvent
        {
            public int DeviceId { get; set; }
            public List<EquipEventItem> EquipEvents { get; set; } = new ();
        }

        public class EquipEventItem
        {
            /// <summary>
            /// 显示到实时快照的内容
            /// </summary>
            public string Msg { get; set; }

            /// <summary>
            /// 联动传入的内容，如果为空就传入msg
            /// </summary>
            public string Msg4Linkage { get; set; }
            public int Level { get; set; }
            public DateTime OccurDateTime { get; set; }
            public int EquipNo { get; set; }
        }
    }
}