//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.AppServices;
using IoTCenterHost.AppServices.Interfaces.Token;

namespace IoTCenterHost.AppServices.Interfaces
{
    public interface IotRealTimeDataService
    {
        void CreateRealDataItem();
        void RemoveRealData(string connectId);


        RealTimeDataItem GetIotRealTimeData(string connectId);

        RealTimeDataItem GetRealTimeDataItem();
        RealTimeDataItem GetIotRealTimeData(string connectId, LoginUser loginUser);
    }
}
