//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenterHost.Core.Abstraction.IotModels
{
    public class Pagination
    {
        public string WhereCause { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public bool IsAsc { get; set; }

    }
    public class PaginationData
    {
        public string Data { get; set; }
        public int Total { get; set; }
    }

    public class RealTimeGroupCount
    {
        public int Level { get; set; }

        public int Total { get; set; }
    }

    public class RealTimePageModel
    {

        public string EventType { get; set; }

        public string EventName { get; set; }

        public bool? Confirmed { get; set; }

        public string Guid { get; set; }
    }


    public class RealTimeFilterPageModel : RealTimePageModel
    {

        public int[] Equips { get; set; }
        public int[] Level { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string MaxRecordId { get; set; }

        public string LastRecordId { get; set; }
    }
}
