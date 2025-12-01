// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using IoTCenter.Data.Model;
using IoTCenterCore.AutoMapper;
using Ganweisoft.IoTCenter.Module.EquipConfig;
using System;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class EquipConfigAutoMapperConfig : IAutoMapperConfig
{
    public void Config(IMapperConfigurationExpression expression)
    {
        expression.CreateMap<IotYxp, Yxp>();
        expression.CreateMap<IotYcp, Ycp>();
        expression.CreateMap<IotEquip, Equip>();
        expression.CreateMap<IotSetParm, SetParm>();

        expression.CreateMap<Yxp, IotYxp>();
        expression.CreateMap<Ycp, IotYcp>();
        expression.CreateMap<Equip, IotEquip>();
        expression.CreateMap<SetParm, IotSetParm>();

        expression.CreateMap<SetDataModel, IotSetParm>().ForMember(d => d.StaN,
                            opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));

        expression.CreateMap<YcpDataModel, IotYcp>().ForMember(d => d.StaN,
                opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));
        
        expression.CreateMap<YxpDataModel, IotYxp>().ForMember(d => d.StaN,
            opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));

        expression.CreateMap<EquipDataModel, IotEquip>().ForMember(d => d.StaN,
                opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));
        
        expression.CreateMap<Equip, EquipDataModel>();
        
        expression.CreateMap<IotEquip, EquipDataModel>();

        expression.CreateMap<YxpDataModel, Yxp>().ForMember(d => d.StaN,
            opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));

        expression.CreateMap<YcpDataModel, Ycp>().ForMember(d => d.StaN,
            opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));

        expression.CreateMap<EquipDataModel, Equip>().ForMember(d => d.StaN,
            opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));

        expression.CreateMap<SetDataModel, SetParm>().ForMember(d => d.StaN,
            opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo));

        expression.CreateMap<BatchYcpModel, Ycp>()
                       .ForMember(d => d.StaN, opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo))
                       .ForMember(d => d.CurveRcd, opt => opt.MapFrom(src => src.CurveRcd != 0))
                       .ForMember(d => d.SafeBgn, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(d => d.SafeEnd, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(d => d.AlarmScheme, opt => opt.MapFrom(src => Convert.ToInt32($"{src.ShowAlarm}{src.RecordAlarm}{src.Smslarm}{src.EmailAlarm}", 2)));

        expression.CreateMap<Ycp, BatchYcpModel>();

        expression.CreateMap<BatchYxpModel, Yxp>()
                       .ForMember(d => d.StaN, opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo))
                       .ForMember(d => d.CurveRcd, opt => opt.MapFrom(src => src.CurveRcd != 0))
                       .ForMember(d => d.SafeBgn, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(d => d.SafeEnd, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(d => d.AlarmScheme, opt => opt.MapFrom(src => Convert.ToInt32($"{src.ShowAlarm}{src.RecordAlarm}{src.Smslarm}{src.EmailAlarm}", 2)));

        expression.CreateMap<Yxp, BatchYxpModel>();

        expression.CreateMap<BatchSetModel, SetParm>()
                       .ForMember(d => d.StaN, opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo))
                       .ForMember(d => d.Record, opt => opt.MapFrom(src => src.Record != 0))
                       .ForMember(d => d.EnableVoice, opt => opt.MapFrom(src => src.EnableVoice != 0))
                       .ForMember(d => d.QrEquipNo, opt => opt.MapFrom(src => src.QrEquipNo ?? 0));

        expression.CreateMap<SetParm, BatchSetModel>();

        expression.CreateMap<BatchYcpModel, IotYcp>()
                       .ForMember(d => d.StaN, opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo))
                       .ForMember(d => d.CurveRcd, opt => opt.MapFrom(src => src.CurveRcd != 0))
                        .ForMember(d => d.AlarmScheme, opt => opt.MapFrom(src => Convert.ToInt32($"{src.ShowAlarm}{src.RecordAlarm}{src.Smslarm}{src.EmailAlarm}", 2)));

        expression.CreateMap<BatchYxpModel, IotYxp>()
                       .ForMember(d => d.StaN, opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo))
                       .ForMember(d => d.CurveRcd, opt => opt.MapFrom(src => src.CurveRcd != 0))
                       .ForMember(d => d.AlarmScheme, opt => opt.MapFrom(src => Convert.ToInt32($"{src.ShowAlarm}{src.RecordAlarm}{src.Smslarm}{src.EmailAlarm}", 2)));
        expression.CreateMap<IotYxp, BatchYxpModel>();

        expression.CreateMap<BatchSetModel, IotSetParm>()
                       .ForMember(d => d.StaN, opt => opt.MapFrom(src => src.StaNo <= 0 ? 1 : src.StaNo))
                       .ForMember(d => d.Record, opt => opt.MapFrom(src => src.Record != 0))
                       .ForMember(d => d.EnableVoice, opt => opt.MapFrom(src => src.EnableVoice != 0));
    }
}
