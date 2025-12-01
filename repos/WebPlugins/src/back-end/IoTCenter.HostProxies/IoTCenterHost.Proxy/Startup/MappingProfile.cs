// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using IoTCenterCore.AutoMapper;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Proto;

namespace IoTCenterHost.Proxy.StartUp
{
    public partial class MappingProfile : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<IoTCenterHost.Core.Abstraction.WcfZCItem, FirstGetRealZCItemReply.Types.WcfZCItem>().ReverseMap();
            expression.CreateMap<GrpcMessageLevel, AddMessageRequest.Types.MessageLevel>().ReverseMap();
            expression.CreateMap<GrpcEquipState, IoTCenterHost.Proto.EquipState>().ReverseMap();
            expression.CreateMap<IoTCenterHost.Core.Abstraction.WcfZCItem, EquipSetInfo>().ReverseMap();
            expression.CreateMap<IoTCenterHost.Proto.ChangedEquip, GrpcChangedEquip>().ReverseMap();
            expression.CreateMap<EquipSetInfo, IoTCenterHost.Core.Abstraction.IotModels.EquipSetInfo>().ReverseMap();
            expression.CreateMap<BaseResult, ResponseModel>().ReverseMap();
            expression.CreateMap<GwClientType, IoTCenterHost.Proto.ClientType>().ReverseMap();
            expression.CreateMap<YcItemResponse, GrpcYcItem>().ReverseMap();
        }
    }
}
