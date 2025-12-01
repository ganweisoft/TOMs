// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.Interfaces.Services;
using IoTCenterHost.Proto;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using static IoTCenterHost.Proto.Greeter;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class GreetServiceImpl : IGreetService
    {
        private readonly GreeterClient _greeterClient;
        private readonly IConfiguration _configuration;
        private readonly int _waitTime = 10;

        public GreetServiceImpl(GreeterClient greeterClient, IConfiguration configuration)
        {
            _greeterClient = greeterClient;
            _configuration = configuration;
        }

        public async Task<bool> GreetAsync()
        {
            var tryTime = 0;
            var tryConnectTime = int.Parse(_configuration["HostSetting:TryConnectTime"]);
            while (tryTime <= tryConnectTime)
            {
                tryTime += 1;
                try
                {
                    var reply = await _greeterClient.SayHelloExAsync(new HelloRequest { Name = "WebServer" });
                    if (!string.IsNullOrEmpty(reply.Message))
                    {
                        var result = reply.Message.FromJson<HelloReplyResponse>();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(tryTime <= tryConnectTime
                        ? $"连接网关出现错误:{ex},{_waitTime}秒进行第{tryTime}次重试。"
                        : $"连接网关出现错误:{ex},已超出最大次数{tryConnectTime}。");
                }

                await Task.Delay(TimeSpan.FromSeconds(_waitTime));
            }

            return false;
        }

        public async Task<(bool, string)> GreetExAsync()
        {
            var tryTime = 0;
            var tryConnectTime = int.Parse(_configuration["HostSetting:TryConnectTime"]);
            while (tryTime <= tryConnectTime)
            {
                tryTime += 1;
                try
                {
                    var reply = await _greeterClient.SayHelloExAsync(new HelloRequest { Name = "WebServer" });
                    if (!string.IsNullOrEmpty(reply.Message))
                    {
                        return (true, reply.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(tryTime <= tryConnectTime
                        ? $"连接网关出现错误:{ex},{_waitTime}秒进行第{tryTime}次重试。"
                        : $"连接网关出现错误:{ex},已超出最大次数{tryConnectTime}。");
                }

                await Task.Delay(TimeSpan.FromSeconds(_waitTime));
            }

            return (false, string.Empty);
        }
    }



    public class HelloReplyResponse
    {
        public string ConnectId { get; set; }
        public string MqIdentity { get; set; }
        public int MqPort { get; set; }
        public string MqTopic { get; set; }
        public string DesKey { get; set; }
        public string PluginsPath { get; set; }
        public string SafetyLevel { get; set; }
        public string SingleAppStart { get; set; }
        public string DbConnectionsJson { get; set; }
        public DateTime HostStartTime { get; set; }
        public string RedisOptionJson { get; set; }
        public string AllowOrigins { get; set; }
        public string WebApiOptionJson { get; set; }
        public string AppSettingJson { get; set; }
    }
}
