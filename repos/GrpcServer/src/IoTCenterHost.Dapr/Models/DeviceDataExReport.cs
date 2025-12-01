//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace IoTCenterHost.Dapr.Models
{
    public class DeviceDataExReport
    {
        public int DataType { get; set; }
        public string Time { get; set; }
        public string Flow { get; set; }
        public List<DataItem> DataItems { get; set; }

        public class DataItem
        {
            public int DeviceId { get; set; }
            public Dictionary<int, object> Attribute { get; set; } = new();
        }
    }
}