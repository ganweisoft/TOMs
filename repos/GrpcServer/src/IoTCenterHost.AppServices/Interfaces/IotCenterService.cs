//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;

namespace IoTCenterHost.AppServices.Interfaces
{
    public interface IotCenterService
    {
        void StartService();
        void StopService();
        void RebootService();

        bool Confirm2NormalState(int iEqpNo, string sYcYxType, int iYcYxNo);
        string GetVersionInfo();
        void AddConfirmedMessage(RealTimeEventItem ConfirmRealTimeEventItem);
    }
}
