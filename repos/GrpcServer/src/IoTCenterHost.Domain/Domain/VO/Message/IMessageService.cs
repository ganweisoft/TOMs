//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;

namespace IoTCenterHost.AppServices.Domain.VO.Message
{
    public interface IMessageService
    {

        void ConfirmedRealTimeEventItem(RealTimeEventItem item);
        void DeleteDebugInfo(int iEquipNo);
        void AddMessage(MessageLevel level, string msgstr, int equipno, bool CanRepeat = true);
    }
}
