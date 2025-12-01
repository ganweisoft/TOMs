//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.AppServices.Domain.PO
{
    public class GWZiChanRecord
    {
        public int ID { get; set; }
        public string ZiChanID { get; set; }
        public DateTime? WeiHuDate { get; set; }
        public string WeiHuName { get; set; }
        public string WeiHuRecord { get; set; }
        public string ItemAddMan { get; set; }
        public DateTime? ItemAddDate { get; set; }
        public string Pictures { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
