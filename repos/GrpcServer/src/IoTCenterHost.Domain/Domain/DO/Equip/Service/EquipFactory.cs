//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.Entity;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.Core.Extension;

namespace IoTCenterHost.AppServices.Domain.DO.Equip
{
    public class EquipFactory : IEquipFactory
    {
        public EquipEntity BuildEntity(EquipPo equip)
        {
            var equipEntity = equip.Map<EquipEntity>();
            equipEntity.CommStatus = equip.Map<CommStatus>();
            equipEntity.DriverInfo = equip.Map<DriverInfo>();
            return equipEntity;

        }
        public EquipEntity BuildEntityFromTask(Task<EquipPo> equip)
        {
            var equipEntity = equip.Result.Map<EquipEntity>();
            equipEntity.CommStatus = equip.Map<CommStatus>();
            equipEntity.DriverInfo = equip.Map<DriverInfo>();
            return equipEntity;
        }
    }
}
