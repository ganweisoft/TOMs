// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.Interfaces.AppServices;
using IoTCenterHost.Core.Abstraction.Interfaces.Services;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterHost.Proxy.Connect;
using IoTCenterHost.Proxy.Core;
using IoTCenterHost.Proxy.Gateway;
using IoTCenterHost.Proxy.Proxies;
using IoTCenterHost.Proxy.ServiceImpl;
using IoTCenterHost.Proxy.StartUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.Proxy
{
    public static class IotCenterExtension
    {
        public static IServiceCollection RegistIotHostServices(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceCollection = services
                .AddScoped<IoTCenterAppService, IoTCenterAppServiceImpl>()
                .AddScoped<ICommandAppService, CommandAppServiceImpl>()
                .AddScoped<ICurveClientAppService, CurveAppServiceImpl>()
                .AddScoped<IEquipAlarmAppService, EquipAlarmAppServiceImpl>()
                .AddScoped<IConnectService, ConnectServiceImpl>()
                .AddScoped<IotCenterHostService, AlarmCenterServiceClientImpl>()
                .AddScoped<IGreetService, GreetServiceImpl>()
                .AddScoped<IYCClientAppService, YCAppServiceImpl>()
                .AddScoped<IYXClientAppService, YXAppServiceImpl>()
                .AddScoped<IotCenterHostCallbackService, IoTCallbackServiceImpl>()
                .AddScoped<IEquipBaseClientAppService, EquipBaseAppServiceImpl>()
                .AddScoped<IRoleAppService, RoleManageAppServiceImpl>()
                .AddScoped<IUserAppService, UserManageAppServiceImpl>()
                .AddSingleton<IEquipManager, EquipManager>()
                .AddScoped<IToolService, ToolServiceImplV2>()
                .AddSingleton<IConnectPoolManager, ConnectPoolManager>()
               .RegistGrpcClient(configuration)
                .AddScoped<IAlarmEventClientAppService, AlarmEventAppServiceImpl>()
                .AddScoped<IEquipBaseAppService, EquipBaseAppServiceImpl>()
                .AddScoped<IConnectStatusProvider, HttpConnectStatusProvider>()
                .AddScoped<IoTGateway>()
                .AddSingleton<EquipGatewayExchanger>()
                .AddSingleton<IoTGatewayManager>()
            .AddScoped<IoTSubGatewayService, IoTSubGatewayServiceImpl>()
            .AddScoped<IoTServiceClientFactory>();
            return serviceCollection;
        }
    }
}
