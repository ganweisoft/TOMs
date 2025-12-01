// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using Grpc.Health.V1;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static IoTCenterHost.Proto.Greeter;
using static IoTCenterHost.Proto.SystemManage;
using static IoTCenterHost.Proto.Tool;

namespace IoTCenterHost.Proxy.StartUp
{
    public static class GrpcClientConfiguration
    {
        private const string ServerTag = "servertag";

        public static IServiceCollection RegistGrpcClient(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            RegisterGrpcClientWithAuth<IotService.IotServiceClient>(services, configuration);
            RegisterGrpcClient<GreeterClient>(services, configuration);
            RegisterGrpcClientWithAuth<IotQueryService.IotQueryServiceClient>(services, configuration);
            RegisterGrpcClientWithAuth<ToolClient>(services, configuration);
            RegisterGrpcClientWithAuth<SystemManageClient>(services, configuration);
            RegisterGrpcClientWithAuth<Health.HealthClient>(services, configuration);
            RegisterGrpcClientWithAuth<IotCallbackService.IotCallbackServiceClient>(services, configuration);
            RegisterGrpcClientWithAuth<iotsubgatewayContract.iotsubgatewayContractClient>(services, configuration);

            return services;
        }

        private static void RegisterGrpcClient<TClient>(IServiceCollection services, IConfiguration config)
            where TClient : class
        {
            services.AddGrpcClient<TClient>(options =>
            {
                SetConnectionConfiguration(config, options);
            });
        }

        private static void RegisterGrpcClientWithAuth<TClient>(IServiceCollection services, IConfiguration config)
            where TClient : class
        {
            services.AddGrpcClient<TClient>(options =>
            {
                SetConnectionConfiguration(config, options);
            })
            .AddCallCredentials((context, metadata, provider) =>
            {
                AddCredentials(context, metadata, provider);
                return Task.CompletedTask;
            })
            .ConfigureChannel(channel =>
            {
                channel.UnsafeUseInsecureChannelCallCredentials = true;
            });
        }

        private static void SetConnectionConfiguration(IConfiguration config, Grpc.Net.ClientFactory.GrpcClientFactoryOptions options)
        {
            options.Address = new Uri(config["HostSetting:Address"]);
            options.ChannelOptionsActions.Add(channel =>
            {
                channel.MaxReceiveMessageSize = int.MaxValue;
                channel.HttpHandler = new HttpClientHandler
                {
                    MaxConnectionsPerServer = int.MaxValue
                };
            });
        }

        private static void AddCredentials(AuthInterceptorContext context, Metadata metadata, IServiceProvider provider)
        {
            var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;

            if (httpContext == null)
                return;

            var connectStatusProvider = httpContext.RequestServices?.GetService<IConnectStatusProvider>()
                                      ?? provider.GetRequiredService<IConnectStatusProvider>();

            var token = connectStatusProvider?.Token;
            var tag = httpContext.User?.FindFirst(d => d.Type == ServerTag)?.Value;

            if (!string.IsNullOrEmpty(token))
                metadata.Add("Authorization", $"Bearer {token}");
            else if (!string.IsNullOrEmpty(tag))
                metadata.Add("Authorization", $"Bearer {tag}");

            var ip = httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded)
                     ? forwarded.ToString().Split(',')[0].Trim()
                     : httpContext.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(ip))
                metadata.Add("ClientRealIp", ip);

            var port = httpContext.Connection.RemotePort;
            metadata.Add("ClientRealPort", port.ToString());
        }
    }
}
