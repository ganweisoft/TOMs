// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using IoTCenter.Utilities;
using IoTCenterHost.Proxy.Core;
using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace IoTCenterHost.Proxy.Proxies
{
    public class IoTServiceClientFactory
    {
        private IConnectStatusProvider _connectStatusProvider;
        private readonly IoTCenter.Utilities.IMemoryCacheService _memoryCacheService;
        private readonly ILoggingService _loggingService;
        private ConcurrentDictionary<string, ClientBase> _sopedClient = new ();
        public IoTServiceClientFactory(IConnectStatusProvider connectStatusProvider,
            IoTCenter.Utilities.IMemoryCacheService memoryCacheService,
            ILoggingService loggingService)
        {
            _connectStatusProvider = connectStatusProvider;
            _memoryCacheService = memoryCacheService;
            _loggingService = loggingService;
        }
        public TClient CreateContractClient<TClient>(string gatewayAddr) where TClient : ClientBase
        {
            if (_sopedClient.ContainsKey(gatewayAddr))
            {
                return _sopedClient[gatewayAddr] as TClient;
            }

            Type type = typeof(TClient);
            try
            {
                var client = type.GetConstructor(new Type[1] { typeof(GrpcChannel) })
                    .Invoke(new object[1] { (ChannelBase)CreateChannel(gatewayAddr) }) as TClient;

                _loggingService.Debug($"{gatewayAddr}获取动态服务连接上下文成功");
                _sopedClient.AddOrUpdate(gatewayAddr, client, (_, v) => v);
                return client;
            }
            catch (Exception ex)
            {
                _loggingService.Error($"建立服务连接失败，请检查服务可用性.{ex}");
                return null;
            }
        }



        public GrpcChannel CreateChannel(string gatewayAddr)
        {
            GrpcChannel channel = null;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            try
            {
                var defaultMethodConfig = new MethodConfig
                {
                    Names = { MethodName.Default },
                    RetryPolicy = new RetryPolicy
                    {
                        MaxAttempts = 5,
                        InitialBackoff = TimeSpan.FromSeconds(1),
                        MaxBackoff = TimeSpan.FromSeconds(5),
                        BackoffMultiplier = 1.5,
                        RetryableStatusCodes = { StatusCode.Unavailable, StatusCode.Unknown }
                    }
                };
                if (!string.IsNullOrEmpty(_connectStatusProvider.Token))
                {
                    var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
                    {
                        metadata.Add("Authorization", $"Bearer {_connectStatusProvider.Token}");
                    });
                    channel = GrpcChannel.ForAddress(gatewayAddr, new GrpcChannelOptions
                    {
                        Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credentials),
                        UnsafeUseInsecureChannelCallCredentials = true,
                        MaxReceiveMessageSize = int.MaxValue,
                        MaxSendMessageSize = int.MaxValue,
                        ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
                    });
                }
                else
                {
                    channel = GrpcChannel.ForAddress(gatewayAddr, new GrpcChannelOptions
                    {
                        HttpHandler = handler,
                        MaxReceiveMessageSize = int.MaxValue,
                        MaxSendMessageSize = int.MaxValue,
                    });
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error($"建立服务连接失败，请检查服务可用性.{ex}");
                return null;
            }
            return channel;
        }
    }
}
