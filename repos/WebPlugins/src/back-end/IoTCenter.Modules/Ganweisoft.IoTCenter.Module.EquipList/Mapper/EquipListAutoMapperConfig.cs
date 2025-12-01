// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using IoTCenter.Data.Model;
using IoTCenterCore.AutoMapper;

namespace Ganweisoft.IoTCenter.Module.EquipList
{
    public class EquipListAutoMapperConfig : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<Equip, AbnormalDeviceExportModel>();
            expression.CreateMap<AbnormalDeviceExportModel, Equip>();

            expression.CreateMap<Yxp, AbnormalDeviceYxpExportModel>();
            expression.CreateMap<AbnormalDeviceYxpExportModel, Yxp>();

            expression.CreateMap<SetParm, AbnormalDeviceSetParmExportModel>();
            expression.CreateMap<AbnormalDeviceSetParmExportModel, SetParm>();
        }
    }
}
