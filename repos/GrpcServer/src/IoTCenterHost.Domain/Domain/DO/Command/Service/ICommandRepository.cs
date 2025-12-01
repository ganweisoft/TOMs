//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.AppServices.Domain.DO.Equip.Service
{
    public interface ICommandRepository
    {
        void ResetEquipmentLinkage();
        bool HaveSet(int EquipNo);
        string GetSetListStr(int iEquipNo);
    }
}
