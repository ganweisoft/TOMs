//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenterHost.Core.Abstraction.BaseModels
{
    public class GrpcGetKeyModel
    {
        public int equipno { get; set; }
        public int ycyxno { get; set; }
        public int GWEventType { get; set; }
        public DateTime time { get; set; }
    }

    public class GrpcGetEventInfo
    {
        public DateTime bgn { get; set; }
        public DateTime end { get; set; }
        public int stano { get; set; }
        public string eqpno { get; set; }
        public int ycyxno { get; set; }
        public int GWEventType { get; set; }

        public int pageSize { get; set; }

        public override string ToString()
        {
            return $"{bgn}{end}{stano}{eqpno}{ycyxno}{GWEventType}{pageSize}";
        }
    }

    public struct GrpcGwEventInfo
    {
        public long id { get; set; }
        public DateTime datetime { get; set; }
        public string gwevent { get; set; }
    }

    public class GrpcAddEventInfo
    {
        public DateTime GWDateTime { get; set; }
        public int equipno { get; set; }
        public int ycyxno { get; set; }
        public int GWEventType { get; set; }
        public string GWEventJson { get; set; }
    }
}
