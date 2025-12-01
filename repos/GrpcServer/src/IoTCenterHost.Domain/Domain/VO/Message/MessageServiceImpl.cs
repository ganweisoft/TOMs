//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;

namespace IoTCenterHost.AppServices.Domain.VO.Message
{
    public class MessageServiceImpl : IMessageService
    {
        public void ConfirmedRealTimeEventItem(RealTimeEventItem item)
        {
            MessageService.AddConfirmedMessage(item);
        }

        public void DeleteDebugInfo(int iEquipNo)
        {
            MessageService.DeleteDebugInfo(iEquipNo);
        }


        public void AddMessage(MessageLevel level, string msgstr, int equipno, bool CanRepeat = true)
        {
            MessageService.AddMessage(level, msgstr, equipno, CanRepeat);
        }
    }
}
