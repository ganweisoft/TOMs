//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices
{
    [Authorize]
    public class GreeterService : Greeter.GreeterBase
    {
        public const string HostStartTimeMemKey = "GWHost_ApplicationStarted_Time";
        private readonly IMemoryCacheService _memoryCache;
        private readonly IHostConfigurationAppService _hostConfiguration;
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(IHostConfigurationAppService hostConfiguration,
            IMemoryCacheService memoryCache, ILogger<GreeterService> logger)
        {
            _memoryCache = memoryCache;
            _hostConfiguration = hostConfiguration;
            _logger = logger;
        }
        [AllowAnonymous]
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {

            return Task.FromResult(new HelloReply
            {
                Message = Guid.NewGuid().ToString("N")
            });
        }
        [AllowAnonymous]
        public override Task<HelloReply> SayHelloEx(HelloRequest request, ServerCallContext context)
        {
            var propXml = _hostConfiguration.GetPropertiesXmlModel();
            var hostStartTime = _memoryCache.Get<DateTime>(HostStartTimeMemKey);
            var result = Task.FromResult(new HelloReply
            {
                Message = new HelloReplyResponse
                {
                    ConnectId = Guid.NewGuid().ToString("N"),//此次内部的方法涉及订阅设备事件。
                    PluginsPath = propXml.PluginsPath,
                    SingleAppStart = propXml.SingleAppStart,
                    HostStartTime = hostStartTime,
                    DbConnectionsJson = new
                    {
                        DbConn = propXml.DbConnections
                    }.ToJson(),
                    AllowOrigins = propXml.AllowOrigins.ToJson(),
                    RedisOptionJson = propXml.RedisOption.ToJson(),
                    WebApiOptionJson = propXml.WebApiOption.ToJson(),
                    AppSettingJson = new
                    {
                        WebApi = propXml.WebApiOption,
                        propXml.AllowOrigins,
                        HostServer = new
                        {
                            propXml.PluginsPath,
                            propXml.SingleAppStart,
                            propXml.StorageFile,
                            propXml.DebugEquipNos,
                        }
                    }.ToJson()
                }.ToJson()
            });
            return result;
        }
    }
    public class HelloReplyResponse
    {
        public string ConnectId { get; set; }
        public string PluginsPath { get; set; }
        public string SafetyLevel { get; set; }
        public string SingleAppStart { get; set; }
        public string DbConnectionsJson { get; set; }
        public string RedisOptionJson { get; set; }
        public string AllowOrigins { get; set; }
        public DateTime HostStartTime { get; set; }
        public string WebApiOptionJson { get; set; }
        public string AppSettingJson { get; set; }
    }

}
