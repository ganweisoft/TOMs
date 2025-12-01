//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.AppServices;
using IoTCenterHost.AppServices.Domain.DO.Equip;
using IoTCenterHost.AppServices.Domain.DO.Equip.Service;
using IoTCenterHost.AppServices.Domain.DO.GWServiceToken;
using IoTCenterHost.AppServices.Domain.DO.RemoteStatus;
using IoTCenterHost.AppServices.Domain.DO.RemoteValue;
using IoTCenterHost.AppServices.Domain.DO.Role;
using IoTCenterHost.AppServices.Domain.DO.User;
using IoTCenterHost.AppServices.Domain.VO.MemoryData.Curve;
using IoTCenterHost.AppServices.Domain.VO.Message;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.AppServices
{
    public static class InjectExtension
    {
        public static IServiceCollection InjectRepositoryService(this IServiceCollection services)
        {
            services.AddScoped<IRemoteValueRepository, RemoteValueRepository>();
            services.AddScoped<IRemoteStatusRepository, RemoteStatusRepository>();
            services.AddScoped<IEquipRepository, EquipRepository>();
            services.AddScoped<ICommandRepository, CommandRepositoryImpl>();
            services.AddScoped<IMessageService, MessageServiceImpl>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICurveDomainService, CurveDomainServiceImpl>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IGWServiceTokenRepository, GWServiceTokenRepository>();
            return services;
        }
        public static IServiceCollection InjectAppService(this IServiceCollection services)
        {
            services.AddScoped<IAccountAppService, AccountAppServiceImpl>();
            services.AddScoped<IAlarmEventServerAppService, AlarmEventAppServiceImpl>();
            services.AddScoped<ICommandAppService, CommandAppServiceImpl>();
            services.AddScoped<ICurveServerAppService, CurveAppServiceImpl>();
            services.AddScoped<IEquipAlarmAppService, EquipAlarmAppServiceImpl>();
            services.AddScoped<IEquipBaseServerService, EquipBaseAppServiceImpl>();
            services.AddScoped<IYCServerAppService, YCAppServiceImpl>();
            services.AddScoped<IYXServerAppService, YXAppServiceImpl>();
            services.AddScoped<IUserAppServerService, UserAppServiceImpl>();
            return services;
        }
        public static IServiceCollection InjectSingletonService(this IServiceCollection services)
        {
            services.AddSingleton<IMemoryCacheService, MemoryCacheServiceImpl>();
            services.AddSingleton<IotRealTimeDataService, IotRealTimeDataServiceImpl>();
            return services;
        }
    }
}
