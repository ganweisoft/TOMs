//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.AppServices.Domain.DO.Equip
{
    public class DriverInfo
    {
        public string communication_drv { get; set; }
        public string local_addr { get; set; }
        public string equip_addr { get; set; }
        public string communication_param { get; set; }
        public string communication_time_param { get; set; }
        public int raw_equip_no { get; set; }
    }
}
