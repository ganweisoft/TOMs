//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.Equip;
using IoTCenterHost.AppServices.Domain.Entity;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.ProxyModels;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices.StartUp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateEquipPoMapper();
            CreateUserPoMapper();
            CreateIoTCenterMapper();
            CreateGrpcResultMapper();
        }
        private void CreateEquipPoMapper()
        {
            CreateMap<EquipPo, DriverInfo>().ReverseMap();
            CreateMap<EquipPo, EquipEntity>().ReverseMap();
            CreateMap<EquipPo, CommStatus>().ReverseMap();
            CreateMap<Task<EquipPo>, Task<DriverInfo>>().ReverseMap();
            CreateMap<Task<EquipPo>, Task<EquipEntity>>().ReverseMap();
            CreateMap<Task<EquipPo>, Task<CommStatus>>().ReverseMap();
        }
        private void CreateUserPoMapper()
        {
            CreateMap<Domain.DO.User.UserEntity, GWUserVO>().ReverseMap();
            CreateMap<Task<Domain.DO.User.UserEntity>, Task<GWUserVO>>().ReverseMap();
            CreateMap<GWDataCenter.Database.GWUserTableRow, GWUserVO>().ReverseMap();
        }
        private void CreateIoTCenterMapper()
        {
            CreateMap<WcfRealTimeEventItem, RealTimeEventItem>()
            .ForMember(t => t.Related_video, opts => opts.MapFrom(a => a.Related_pic))
            .ForMember(t => t.PlanNo, opts => opts.MapFrom(a => a.PlanNo))
            .ForMember(t => t.ZiChanID, opts => opts.MapFrom(a => a.ZiChanID))
            .ReverseMap();
        }

        private void CreateGrpcResultMapper()
        {
            CreateMap<WcfZCItem, FirstGetRealZCItemReply.Types.WcfZCItem>().ReverseMap();
            CreateMap<WcfZCItem, Core.Abstraction.IotModels.EquipSetInfo>().ReverseMap();
            CreateMap<IoTCenterHost.Proto.ChangedEquip, GWDataCenter.ChangedEquip>().ReverseMap();
            CreateMap<WcfRealTimeEventItem, RealTimeEventItemProto>().ReverseMap();
            CreateMap<Core.Abstraction.IotModels.EquipSetInfo, IoTCenterHost.Core.Abstraction.IotModels.EquipSetInfo>().ReverseMap();
            CreateMap<BaseResult, ResponseModel>().ReverseMap();
            CreateMap<ProxyEquipItem, GrpcEquipItem>().ReverseMap();
            CreateMap<ProxyYcItem, GrpcYcItem>().ReverseMap();
            CreateMap<ProxyYxItem, GrpcYxItem>().ReverseMap();
            CreateMap<MessageLevel, GrpcMessageLevel>().ReverseMap();
            CreateMap<GWDataCenter.EquipState, GrpcEquipState>().ReverseMap();
            CreateMap<GrpcEquipState, Proto.EquipState>().ReverseMap();

            CreateMap<YcItemResponse, ProxyYcItem>().ReverseMap();
        }

    }
}
