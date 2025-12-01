//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class EventModel
    {
        public int Sta_n { get; set; }
        public string gwEvent { get; set; }
        public string time { get; set; }
        public string confirmname { get; set; }
        public string confirmtime { get; set; }
        public string confirmremark { get; set; }
        public string GUID { get; set; }
    }
}
